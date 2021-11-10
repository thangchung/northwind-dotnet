using SalePayment;
using SalePayment.Data;
using SalePayment.Domain;
using SalePayment.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddCustomMediatR(new[] { typeof(Order) })
    .AddCustomValidators(new[] { typeof(Order) })
    .AddPersistence(builder.Configuration)
    .AddSwaggerGen()
    .AddCdCConsumers()
    .AddMassTransit()
    .AddGrpcClients(builder.Configuration)
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

app.MapPost("/api/v1/order",
    async ([FromBody] SubmitOrder.Command model, ISender sender) => await sender.Send(model));

app.MapPost("/api/v1/payment",
    async (ProcessPayment.Command model, ISender sender) => await sender.Send(model));

app.MapPost("/api/v1/shipment/{orderId}/pick",
    async (Guid orderId, PickShipment.Command model, ISender sender) =>
    {
        model.OrderId = orderId;
        return await sender.Send(model);
    });

app.MapPost("/api/v1/shipment/{orderId}/delivery",
    async (Guid orderId, DeliverShipment.Command model, ISender sender) =>
    {
        model.OrderId = orderId;
        return await sender.Send(model);
    });

app.MapGet("/api/v1/order-state-machine",
    (ISender sender) => sender.Send(new GetOrderStateMachine.Query()));

app.Run();
