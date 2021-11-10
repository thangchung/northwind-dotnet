using CustomerService.Data;
using CustomerService.Domain;
using HumanResources.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomMediatR(new[] { typeof(CustomerDemographic) });
builder.Services.AddCustomValidators(new[] { typeof(CustomerDemographic) });

builder.Services.AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("northwind_db"),
        options => options.UseModel(CustomerService.MainDbContextModel.Instance),
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaConsumer(o =>
{
    o.Topic = "customers_cdc_events";
    o.GroupId = "customers_cdc_events_group2";
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
