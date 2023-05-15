using Serilog;
using Shipping;
using Shipping.Domain;
using Shipping.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog("Shipping");

builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddCustomMediatR(new[] {typeof(ShippingOrder)})
    .AddCustomValidators(new[] {typeof(ShippingOrder)})
    .AddPersistence("northwind_db", builder.Configuration)
    .AddSchemeRegistry(builder.Configuration)
    .AddCdCConsumers()
    .AddCustomMassTransit(builder.Configuration)
    .AddSwaggerGen()
    .AddGrpcClients(builder.Configuration)
    .AddDaprClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseSerilogRequestLogging();

app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
    .ExcludeFromDescription();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCustomCors();
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/v1/shipment/{orderId}/pick",
    async (Guid orderId, PickShipmentCommand model, ISender sender) =>
        await sender.Send(model with {OrderId = orderId}));

app.MapPost("/api/v1/shipment/{orderId}/delivery",
    async (Guid orderId, DeliverShipmentCommand model, ISender sender) =>
        await sender.Send(model with {OrderId = orderId}));

app.MapGet("/api/v1/order-state-machine",
    (ISender sender) => sender.Send(new GetShipmentStateMachineQuery()));

app.MapFallback(() => Results.Redirect("/swagger"));

await WithSeriLog(async () =>
{
    await app.DoDbMigrationAsync(app.Logger);

    app.Run();
});
