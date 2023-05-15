using SalePayment;
using SalePayment.Data;
using SalePayment.Domain;
using SalePayment.UseCases;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilog("SalePayment");

builder.Services
    .AddCustomCors()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddCustomMediatR(new[] {typeof(Order)})
    .AddCustomValidators(new[] {typeof(Order)})
    .AddPersistence("northwind_db", builder.Configuration)
    .AddSwaggerGen()
    .AddSchemeRegistry(builder.Configuration)
    .AddCdCConsumers()
    .AddCustomMassTransit(builder.Configuration)
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



app.MapPost("/api/v1/order",
    async (SubmitOrderCommand model, ISender sender) => await sender.Send(model));

app.MapPost("/api/v1/payment",
    async (ProcessPaymentCommand model, ISender sender) => await sender.Send(model));

app.MapGet("/api/v1/order-state-machine",
    (ISender sender) => sender.Send(new GetOrderStateMachineQuery()));

app.MapFallback(() => Results.Redirect("/swagger"));

await WithSeriLog(async () =>
{
    await app.DoDbMigrationAsync(app.Logger);
    await app.DoSeedData(app.Logger);

    app.Run();
});
