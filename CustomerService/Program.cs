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
        builder.Configuration.GetConnectionString("postgres"),
        options => options.UseModel(CustomerService.MainDbContextModel.Instance),
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

builder.Services.AddSchemeRedistry();

builder.Services.AddKafkaConsumer(o =>
{
    o.Topic = "customer_service_events";
    o.GroupId = "customer_service_events_group";
    /*o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
    {
        return result;
    };*/
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
