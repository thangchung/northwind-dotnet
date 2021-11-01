using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Data;
using ProductCatalog.Domain;
using ProductCatalog.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomMediatR(new[] { typeof(Product) });
builder.Services.AddCustomValidators(new[] { typeof(Product) });

builder.Services.AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("postgres"),
        options => options.UseModel(ProductCatalog.MainDbContextModel.Instance),
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();

builder.Services.AddKafkaConsumer(o =>
{
    o.Topic = "supplier_cdc_events";
    o.GroupId = "supplier_cdc_events_group";
    o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
    {
        ISpecificRecord? result = null;
        if (eventFullName == typeof(SupplierCreated).FullName)
        {
            result = await bytes.DeserializeAsync<SupplierCreated>(schemaRegistryClient);
        }
        else if (eventFullName == typeof(SupplierUpdated).FullName)
        {
            result = await bytes.DeserializeAsync<SupplierUpdated>(schemaRegistryClient);
        }
        else if (eventFullName == typeof(SupplierDeleted).FullName)
        {
            result = await bytes.DeserializeAsync<SupplierDeleted>(schemaRegistryClient);
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
await app.DoSeedData(app.Logger);

app.MapGet("/api/v1/products", async (
    [FromHeader(Name = "x-query")] string xQuery,
    HttpContext httpContext, ISender sender) =>
{
    var queryModel = httpContext.SafeGetListQuery<GetProducts.Query, ListResultModel<ProductDto>>(xQuery);
    var result = await sender.Send(queryModel);
    return Results.Ok(result);
});

app.MapPost("/api/v1/products", async (MutateProduct.CreateCommand command, ISender sender) =>
    Results.Ok(await sender.Send(command)));

app.MapPut("/api/v1/products/{id}", async (Guid id, MutateProduct.UpdateCommand command, ISender sender) =>
{
    command.Id = id;
    return Results.Ok(await sender.Send(command));
});

app.MapDelete("/api/v1/products/{id}", async (Guid id, ISender sender) =>
    Results.Ok(await sender.Send(new MutateProduct.DeleteCommand(id))));

app.Run();
