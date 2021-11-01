using SalePayment.Data;
using SalePayment.Domain;

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

builder.Services.AddSchemeRedistry();

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
