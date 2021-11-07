using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;
using Northwind.IntegrationEvents.ViewModels;
using SalePayment.Consumers.MassTransit;
using SalePayment.Data;
using SalePayment.Domain;
using SalePayment.StateMachines;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomMediatR(new[] { typeof(Order) });
builder.Services.AddCustomValidators(new[] { typeof(Order) });

builder.Services.AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("postgres"),
        options => options.UseModel(SalePayment.MainDbContextModel.Instance),
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaConsumer(o =>
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

builder.Services.AddKafkaConsumer(o =>
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

builder.Services.AddKafkaConsumer(o =>
{
    o.Topic = "product_cdc_events";
    o.GroupId = "product_cdc_events_group";
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

// Masstransit registration
builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));
    mt.AddConsumer<ProcessPaymentConsumer>(typeof(ProcessPaymentConsumerDefinition));
    mt.AddConsumer<PickShipmentConsumer>(typeof(PickShipmentConsumerDefinition));
    mt.AddConsumer<DeliverShipmentConsumer>(typeof(DeliverShipmentConsumerDefinition));

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
                r.Connection = "mongodb://127.0.0.1";
                r.DatabaseName = "orders";
            });

        rider.AddProducer<OrderSubmitted>(nameof(OrderSubmitted));
        rider.AddProducer<OrderValidated>(nameof(OrderValidated));
        rider.AddProducer<OrderValidatedFailed>(nameof(OrderValidatedFailed));
        rider.AddProducer<OrderCancelled>(nameof(OrderCancelled));
        rider.AddProducer<PaymentProcessed>(nameof(PaymentProcessed));
        rider.AddProducer<PaymentProcessedFailed>(nameof(PaymentProcessedFailed));
        rider.AddProducer<ShipmentPrepared>(nameof(ShipmentPrepared));

        rider.AddProducer<ShipmentDispatched>(nameof(ShipmentDispatched));
        rider.AddProducer<ShipmentDispatchedFailed>(nameof(ShipmentDispatchedFailed));
        rider.AddProducer<ShipmentDelivered>(nameof(ShipmentDelivered));
        rider.AddProducer<ShipmentDeliveredFailed>(nameof(ShipmentDeliveredFailed));
        rider.AddProducer<ShipmentCancelled>(nameof(ShipmentCancelled));

        rider.AddProducer<OrderCompleted>(nameof(OrderCompleted));

        rider.AddProducer<OrderConfirmed>(nameof(OrderConfirmed));
        rider.AddProducer<MoneyRefunded>(nameof(MoneyRefunded));
        rider.AddProducer<MakeOrderValidated>(nameof(MakeOrderValidated));

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");

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

            k.TopicEndpoint<Null, ShipmentDispatched>(nameof(ShipmentDispatched), "Orders", c =>
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
            });

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

    mt.AddRequestClient<RequestOrder>();
    mt.AddRequestClient<ProcessPayment>();
    mt.AddRequestClient<PickShipmentByShipper>();
    mt.AddRequestClient<DeliverShipmentForCustomer>();
    mt.AddRequestClient<CheckOrder>();
});

builder.Services.AddMassTransitHostedService(true);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
    .ExcludeFromDescription();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCustomCors();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

await app.DoDbMigrationAsync(app.Logger);

app.MapGet("/v1/api/order/{id}/status",
    async (Guid id, IRequestClient<CheckOrder> checkOrderClient) =>
    {
        var (status, notFound) =
            await checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new {OrderId = id});

        if (status.IsCompletedSuccessfully)
        {
            var response = await status;
            return Results.Ok(response.Message);
        }
        else
        {
            var response = await notFound;
            return Results.NotFound(response.Message);
        }
    });

app.MapPost("/v1/api/order",
    async (RequestOrder model, IRequestClient<RequestOrder> requestOrderRequestClient) =>
    {
        var (accepted, rejected) =
            await requestOrderRequestClient.GetResponse<OrderValidated, OrderValidatedFailed>(new
            {
                model.OrderId,
                InVar.Timestamp,
                model.CustomerId,
                model.EmployeeId,
                model.OrderDate,
                model.RequiredDate
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;

            return Results.Ok(response);
        }

        if (accepted.IsCompleted)
        {
            await accepted;

            return Results.Problem("Order was not accepted.");
        }

        {
            var response = await rejected;

            return Results.BadRequest(response.Message);
        }
    });

app.MapPost("/v1/api/payment",
    async (ProcessPayment model, IRequestClient<ProcessPayment> processPaymentRequestClient) =>
    {
        var (accepted, rejected) =
            await processPaymentRequestClient.GetResponse<PaymentProcessed, PaymentProcessedFailed>(new
            {
                model.OrderId, model.Description, InVar.Timestamp
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;

            return Results.Ok(response);
        }

        if (accepted.IsCompleted)
        {
            await accepted;

            return Results.Problem("Payment was not accepted.");
        }

        {
            var response = await rejected;

            return Results.BadRequest(response.Message);
        }
    });

app.MapPost("/v1/api/shipment/{orderId}/pick",
    async (Guid orderId, PickShipmentByShipper model, IRequestClient<PickShipmentByShipper> pickShipmentRequestClient) =>
    {
        model.OrderId = orderId;

        var (accepted, rejected) =
            await pickShipmentRequestClient.GetResponse<ShipmentDispatched, ShipmentDispatchedFailed>(new
            {
                model.OrderId, InVar.Timestamp, model.TransactionId, model.ShipperId, model.BeFailedAt
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;

            return Results.Ok(response);
        }

        if (accepted.IsCompleted)
        {
            await accepted;

            return Results.Problem("Shipment was not accepted.");
        }

        {
            var response = await rejected;

            return Results.BadRequest(response.Message);
        }
    });

app.MapPost("/v1/api/shipment/{orderId}/delivery",
    async (Guid orderId, DeliverShipmentForCustomer model, IRequestClient<DeliverShipmentForCustomer> deliverShipmentRequestClient) =>
    {
        model.OrderId = orderId;

        var (accepted, rejected) =
            await deliverShipmentRequestClient.GetResponse<ShipmentDelivered, ShipmentDeliveredFailed>(new
            {
                model.OrderId, InVar.Timestamp, model.TransactionId, model.ShipperId, model.CustomerId, model.BeFailedAt
            });

        if (accepted.IsCompletedSuccessfully)
        {
            var response = await accepted;

            return Results.Ok(response);
        }

        if (accepted.IsCompleted)
        {
            await accepted;

            return Results.Problem("Shipment was not accepted.");
        }

        {
            var response = await rejected;

            return Results.BadRequest(response.Message);
        }
    });

app.Run();
