using SalePayment;
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

app.MapPost("/v1/api/order",
    async (SubmitOrder.Command model, ISender sender) => await sender.Send(model));

app.MapPost("/v1/api/payment",
    async (ProcessPayment.Command model, ISender sender) => await sender.Send(model));

app.MapPost("/v1/api/shipment/{orderId}/pick",
    async (Guid orderId, PickShipment.Command model, ISender sender) =>
    {
        model.OrderId = orderId;
        return await sender.Send(model);
    });

app.MapPost("/v1/api/shipment/{orderId}/delivery",
    async (Guid orderId, DeliverShipment.Command model, ISender sender) =>
    {
        model.OrderId = orderId;
        return await sender.Send(model);
    });

app.MapGet("/v1/api/order-state-machine",
    (ISender sender) => sender.Send(new GetOrderStateMachine.Query()));

app.Run();
