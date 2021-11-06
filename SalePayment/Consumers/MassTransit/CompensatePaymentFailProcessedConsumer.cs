using MassTransit;
using Northwind.IntegrationEvents.Contracts;

namespace SalePayment.Consumers.MassTransit;

public class CompensatePaymentFailProcessedConsumer : IConsumer<CompensatePaymentFailProcessed>
{
    public async Task Consume(ConsumeContext<CompensatePaymentFailProcessed> context)
    {
        // todo: compensation data
        Console.WriteLine($"Compensation data for order=${context.Message.OrderId}");

        await Task.CompletedTask;
    }
}
