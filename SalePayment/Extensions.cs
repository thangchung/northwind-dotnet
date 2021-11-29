using System.Net;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;
using SalePayment.Consumers.MassTransit;
using SalePayment.Data;
using SalePayment.Data.Repository;
using SalePayment.StateMachines;

namespace SalePayment;

public static class Extensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connStringKey, IConfiguration config)
    {
        services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString(connStringKey),
                options =>
                {
                    options.UseModel(SalePayment.MainDbContextModel.Instance);
                },
                svc => svc.AddRepository(typeof(Repository<>)))
            .AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    private class Http3Handler : DelegatingHandler
    {
        public Http3Handler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = HttpVersion.Version30;
            request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
            return base.SendAsync(request, cancellationToken);
        }
    }

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddGrpcClient<Auditor.AuditorClient>("Auditor", o =>
            {
                o.Address = new Uri(config.GetValue<string>("AuditorGrpcUrl"));
            })
            .ConfigureChannel(options =>
            {
                options.HttpHandler = new Http3Handler(new HttpClientHandler());
            })
            .EnableCallContextPropagation(o => o.SuppressContextNotFoundErrors = true);

        return services;
    }

    public static IServiceCollection AddCdCConsumers(this IServiceCollection services)
    {
        services.AddKafkaConsumer(o =>
        {
            o.Topic = "employee_cdc_events";
            o.GroupId = "employee_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(EmployeeCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<EmployeeCreated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(EmployeeUpdated).FullName)
                {
                    result = await bytes.DeserializeAsync<EmployeeUpdated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(EmployeeDeleted).FullName)
                {
                    result = await bytes.DeserializeAsync<EmployeeDeleted>(schemaRegistryClient);
                }

                return result;
            };
        });

        services.AddKafkaConsumer(o =>
        {
            o.Topic = "customer_cdc_events";
            o.GroupId = "customer_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(CustomerCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<CustomerCreated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(CustomerUpdated).FullName)
                {
                    result = await bytes.DeserializeAsync<CustomerUpdated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(CustomerDeleted).FullName)
                {
                    result = await bytes.DeserializeAsync<CustomerDeleted>(schemaRegistryClient);
                }

                return result;
            };
        });

        services.AddKafkaConsumer(o =>
        {
            o.Topic = "product_cdc_events";
            o.GroupId = "product_cdc_events_sale_payment_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(ProductCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<ProductCreated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ProductUpdated).FullName)
                {
                    result = await bytes.DeserializeAsync<ProductUpdated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ProductDeleted).FullName)
                {
                    result = await bytes.DeserializeAsync<ProductDeleted>(schemaRegistryClient);
                }

                return result;
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(mt =>
        {
            mt.SetKebabCaseEndpointNameFormatter();

            mt.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            mt.AddRider(rider =>
            {
                rider.AddConsumer<MoneyRefundedConsumer>();
                rider.AddConsumer<MakeOrderValidatedConsumer>();
                rider.AddConsumer<OrderConfirmedConsumer>();

                rider.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateMachineDefinition>()
                    /*.InMemoryRepository();*/
                    .MongoDbRepository(r =>
                    {
                        r.Connection = config.GetValue("MassTransit:Sagas:MongoDbUrl","mongodb://127.0.0.1");
                        r.DatabaseName = "orders";
                    });

                rider.AddProducer<OrderSubmitted>(nameof(OrderSubmitted));
                rider.AddProducer<OrderValidated>(nameof(OrderValidated));
                rider.AddProducer<OrderValidatedFailed>(nameof(OrderValidatedFailed));
                rider.AddProducer<OrderCancelled>(nameof(OrderCancelled));
                rider.AddProducer<PaymentProcessed>(nameof(PaymentProcessed));
                rider.AddProducer<PaymentProcessedFailed>(nameof(PaymentProcessedFailed));
                rider.AddProducer<ShipmentPrepared>(nameof(ShipmentPrepared));

                rider.AddProducer<OrderConfirmed>(nameof(OrderConfirmed));
                rider.AddProducer<MoneyRefunded>(nameof(MoneyRefunded));
                rider.AddProducer<MakeOrderValidated>(nameof(MakeOrderValidated));

                rider.AddProducer<OrderCompleted>(nameof(OrderCompleted));

                rider.UsingKafka((context, k) =>
                {
                    k.Host(config.GetValue("Kafka:BootstrapServers", "localhost:9092"));

                    k.TopicEndpoint<Null, OrderSubmitted>(nameof(OrderSubmitted), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, OrderValidated>(nameof(OrderValidated), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, OrderValidatedFailed>(nameof(OrderValidatedFailed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, OrderCancelled>(nameof(OrderCancelled), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, PaymentProcessed>(nameof(PaymentProcessed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, PaymentProcessedFailed>(nameof(PaymentProcessedFailed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentPrepared>(nameof(ShipmentPrepared), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    /*k.TopicEndpoint<Null, ShipmentDispatched>(nameof(ShipmentDispatched), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDispatchedFailed>(nameof(ShipmentDispatchedFailed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDelivered>(nameof(ShipmentDelivered), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDeliveredFailed>(nameof(ShipmentDeliveredFailed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentCancelled>(nameof(ShipmentCancelled), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });*/

                    k.TopicEndpoint<Null, OrderCompleted>(nameof(OrderCompleted), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<OrderState>(context);
                    });

                    k.TopicEndpoint<Null, MoneyRefunded>(nameof(MoneyRefunded), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureConsumer<MoneyRefundedConsumer>(context);
                    });

                    k.TopicEndpoint<Null, MakeOrderValidated>(nameof(MakeOrderValidated), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureConsumer<MakeOrderValidatedConsumer>(context);
                    });

                    k.TopicEndpoint<Null, OrderConfirmed>(nameof(OrderConfirmed), "Orders", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Latest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureConsumer<OrderConfirmedConsumer>(context);
                    });
                });
            });

            //mt.AddRequestClient<CheckOrder>();
        });

        services.AddMassTransitHostedService(true);

        return services;
    }
}
