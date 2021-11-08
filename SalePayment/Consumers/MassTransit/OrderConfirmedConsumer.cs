namespace SalePayment.Consumers.MassTransit;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmed>
{
    private readonly ITopicProducer<ShipmentPrepared> _shipmentPreparedTopicProducer;
    private readonly ILogger<OrderConfirmedConsumer> _logger;

    public OrderConfirmedConsumer(ITopicProducer<ShipmentPrepared> shipmentPreparedTopicProducer,
        ILogger<OrderConfirmedConsumer> logger)
    {
        _shipmentPreparedTopicProducer = shipmentPreparedTopicProducer;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderConfirmed> context)
    {
        _logger.LogInformation(
            $"Send notification for free-shippers or shipper hub to pick the order=${context.Message.OrderId}.");

        // todo: send notification for all free-shippers or to shipper hub, then waiting for them to pick the shipment order
        // Shipper will take action whether to pick or not at via /v1/api/shipment/{order-id}/pick

        await _shipmentPreparedTopicProducer.Produce(new
        {
            context.Message.TransactionId, context.Message.OrderId
        });

        await Task.CompletedTask;
    }
}
