using Shipping.Data;
using Shipping.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomMediatR(new[] { typeof(ShippingOrder) });
builder.Services.AddCustomValidators(new[] { typeof(ShippingOrder) });

builder.Services.AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("postgres"),
        options => options.UseModel(Shipping.MainDbContextModel.Instance),
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

builder.Services.AddSchemeRedistry();

builder.Services.AddKafkaConsumer(o =>
{
    o.Topic = "shippers_cdc_events";
    o.GroupId = "shippers_cdc_events_group";
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

builder.Services.AddKafkaConsumer(o =>
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

app.Run();
