using HumanResources;
using HumanResources.Data;
using HumanResources.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddCustomMediatR(new[] { typeof(Employee) })
    .AddCustomValidators(new[] { typeof(Employee) })
    .AddPersistence(builder.Configuration)
    .AddSwaggerGen()
    .AddSchemeRedistry(builder.Configuration.GetValue<string>("Kafka:SchemaRegistryUrl"))
    .AddDaprClient();

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
await app.DoSeedData(app.Logger);

app.Run();
