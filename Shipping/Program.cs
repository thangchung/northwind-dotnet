using Shipping;
using Shipping.Domain;
using Shipping.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddCustomMediatR(new[] { typeof(ShippingOrder) })
    .AddCustomValidators(new[] { typeof(ShippingOrder) })
    .AddPersistence("northwind_db", builder.Configuration)
    .AddSchemeRegistry(builder.Configuration)
    .AddCdCConsumers()
    .AddCustomMassTransit(builder.Configuration)
    .AddSwaggerGen()
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

app.MapPost("/api/v1/shipment/{orderId}/pick",
    async (Guid orderId, PickShipmentCommand model, ISender sender) =>
        await sender.Send(model with {OrderId = orderId}));

app.MapPost("/api/v1/shipment/{orderId}/delivery",
    async (Guid orderId, DeliverShipmentCommand model, ISender sender) =>
        await sender.Send(model with {OrderId = orderId}));

app.MapFallback(() => Results.Redirect("/swagger"));

app.Run();
