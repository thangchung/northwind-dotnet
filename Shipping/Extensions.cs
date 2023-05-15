using System.Net;
using Grpc.Core;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;
using Shipping.Data;
using Shipping.StateMachines;

namespace Shipping;

public static class Extensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connStringKey,
        IConfiguration config)
    {
        services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString(connStringKey),
                options => options.UseModel(Shipping.MainDbContextModel.Instance),
                svc => svc.AddRepository(typeof(Repository<>)))
            .AddDatabaseDeveloperPageExceptionFilter();

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
                // gRPC - http3
                // options.HttpHandler = new Http3Handler(new HttpClientHandler());

                var httpHandler = new HttpClientHandler();
                httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                options.HttpHandler = httpHandler;

                options.Credentials = ChannelCredentials.Insecure;
            })
            .EnableCallContextPropagation(o => o.SuppressContextNotFoundErrors = true);

        return services;
    }

    public static IServiceCollection AddCdCConsumers(this IServiceCollection services)
    {
        services.AddKafkaConsumer(o =>
        {
            o.Topic = "shipper_cdc_events";
            o.GroupId = "shipper_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(ShipperCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperCreated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ShipperUpdated).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperUpdated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ShipperDeleted).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperDeleted>(schemaRegistryClient);
                }

                return result;
            };
        });

        services.AddKafkaConsumer(o =>
        {
            o.Topic = "order_cdc_events";
            o.GroupId = "order_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(OrderCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<OrderCreated>(schemaRegistryClient);
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
                rider.AddSagaStateMachine<ShipmentStateMachine, ShipmentState, ShipmentStateMachineDefinition>()
                    /*.InMemoryRepository();*/
                    .MongoDbRepository(r =>
                    {
                        r.Connection = config.GetValue("MassTransit:Sagas:MongoDbUrl","mongodb://127.0.0.1");
                        r.DatabaseName = "shipments";
                    });

                rider.AddProducer<ShipmentDispatched>(nameof(ShipmentDispatched));
                rider.AddProducer<ShipmentDispatchedFailed>(nameof(ShipmentDispatchedFailed));
                rider.AddProducer<ShipmentDelivered>(nameof(ShipmentDelivered));
                rider.AddProducer<ShipmentDeliveredFailed>(nameof(ShipmentDeliveredFailed));
                rider.AddProducer<ShipmentCancelled>(nameof(ShipmentCancelled));

                rider.AddProducer<OrderCompleted>(nameof(OrderCompleted));
                rider.AddProducer<MoneyRefunded>(nameof(MoneyRefunded));

                rider.UsingKafka((context, k) =>
                {
                    k.Host(config.GetValue("Kafka:BootstrapServers", "localhost:9092"));

                    k.TopicEndpoint<Null, ShipmentPrepared>(nameof(ShipmentPrepared), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDispatched>(nameof(ShipmentDispatched), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDispatchedFailed>(nameof(ShipmentDispatchedFailed), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDelivered>(nameof(ShipmentDelivered), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentDeliveredFailed>(nameof(ShipmentDeliveredFailed), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });

                    k.TopicEndpoint<Null, ShipmentCancelled>(nameof(ShipmentCancelled), "Shipments", c =>
                    {
                        c.AutoOffsetReset = AutoOffsetReset.Earliest;
                        c.CreateIfMissing(t => t.NumPartitions = 1);
                        c.ConfigureSaga<ShipmentState>(context);
                    });
                });
            });
        });

        services.AddMassTransitHostedService(true);

        return services;
    }
}
