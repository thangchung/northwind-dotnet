using Confluent.Kafka;
using MassTransit;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;
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
    mt.AddConsumer<RequestOrderConsumer>(typeof(RequestOrderConsumerDefinition));
    mt.AddConsumer<ProcessPaymentConsumer>(typeof(ProcessPaymentConsumerDefinition));

    mt.SetKebabCaseEndpointNameFormatter();

    mt.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    mt.AddRider(rider =>
    {
        rider.AddConsumer<CompensatePaymentFailProcessedConsumer>();

        rider.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateMachineDefinition>()
            .MongoDbRepository(r =>
            {
                r.Connection = "mongodb://127.0.0.1";
                r.DatabaseName = "orders";
            });

        rider.AddProducer<OrderRequested>(nameof(OrderRequested));
        rider.AddProducer<PaymentProcessed>(nameof(PaymentProcessed));
        rider.AddProducer<PaymentProcessedFailed>(nameof(PaymentProcessedFailed));

        rider.AddProducer<CompensatePaymentFailProcessed>(nameof(CompensatePaymentFailProcessed));

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");

            k.TopicEndpoint<Null, OrderRequested>(nameof(OrderRequested), "Orders", c =>
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

            k.TopicEndpoint<Null, CompensatePaymentFailProcessed>(nameof(CompensatePaymentFailProcessed), "Orders", c =>
            {
                c.AutoOffsetReset = AutoOffsetReset.Latest;
                c.CreateIfMissing(t => t.NumPartitions = 1);
                c.ConfigureConsumer<CompensatePaymentFailProcessedConsumer>(context);
            });
        });
    });

    mt.AddRequestClient<RequestOrder>();
    mt.AddRequestClient<ProcessPayment>();
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

app.MapGet("/v1/api/order/{id}/status", async (Guid id, IRequestClient<CheckOrder> checkOrderClient) =>
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

app.MapPost("/v1/api/order", async (RequestOrder model, IRequestClient<RequestOrder> requestOrderRequestClient) =>
{
    var (accepted, rejected) = await requestOrderRequestClient.GetResponse<OrderValidated, OrderValidatedFailed>(new
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

        return Results.Problem("Order was not accepted");
    }

    {
        var response = await rejected;

        return Results.BadRequest(response.Message);
    }
});

app.MapPost("/v1/api/payment", async (ProcessPayment model, IRequestClient<ProcessPayment> processPaymentRequestClient) =>
{
    var (accepted, rejected) = await processPaymentRequestClient.GetResponse<PaymentProcessed, PaymentProcessedFailed>(new
    {
        model.OrderId,
        model.Description,
        InVar.Timestamp
    });

    if (accepted.IsCompletedSuccessfully)
    {
        var response = await accepted;

        return Results.Ok(response);
    }

    if (accepted.IsCompleted)
    {
        await accepted;

        return Results.Problem("Payment was not accepted");
    }

    {
        var response = await rejected;

        return Results.BadRequest(response.Message);
    }
});

app.Run();
