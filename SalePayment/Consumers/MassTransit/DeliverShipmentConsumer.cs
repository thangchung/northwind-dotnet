using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;
using Northwind.IntegrationEvents.ViewModels;

namespace SalePayment.Consumers.MassTransit;

public class DeliverShipmentConsumerDefinition : ConsumerDefinition<DeliverShipmentConsumer>
{
    private readonly IServiceProvider _serviceProvider;

    public DeliverShipmentConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ConcurrentMessageLimit = 20;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<DeliverShipmentConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        endpointConfigurator.UseServiceScope(_serviceProvider);
    }
}

public class DeliverShipmentConsumer : IConsumer<DeliverShipmentForCustomer>
{
    private readonly ITopicProducer<ShipmentDelivered> _shipmentDeliveredTopicProducer;
    private readonly ITopicProducer<ShipmentDeliveredFailed> _shipmentDeliveredFailedTopicProducer;
    private readonly ITopicProducer<OrderCompleted> _orderCompletedTopicProducer;
    private readonly ITopicProducer<ShipmentCancelled> _shipmentCancelledTopicProducer;

    public DeliverShipmentConsumer(
        ITopicProducer<ShipmentDelivered> shipmentDeliveredTopicProducer,
        ITopicProducer<ShipmentDeliveredFailed> shipmentDeliveredFailedTopicProducer,
        ITopicProducer<OrderCompleted> orderCompletedTopicProducer,
        ITopicProducer<ShipmentCancelled> shipmentCancelledTopicProducer)
    {
        _shipmentDeliveredTopicProducer = shipmentDeliveredTopicProducer;
        _shipmentDeliveredFailedTopicProducer = shipmentDeliveredFailedTopicProducer;
        _orderCompletedTopicProducer = orderCompletedTopicProducer;
        _shipmentCancelledTopicProducer = shipmentCancelledTopicProducer;
    }

    public async Task Consume(ConsumeContext<DeliverShipmentForCustomer> context)
    {
        // for testing only
        if (context.Message.BeFailedAt == "Delivered")
        {
            await _shipmentDeliveredFailedTopicProducer.Produce(new {context.Message.OrderId});
            await _shipmentCancelledTopicProducer.Produce(new {context.Message.OrderId});

            if (context.RequestId != null)
                await context.RespondAsync<ShipmentDeliveredFailed>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    Reason = $"Test cannot deliver the packages: {context.Message.OrderId}"
                });

            return;
        }

        await _shipmentDeliveredTopicProducer.Produce(new {context.Message.OrderId});
        await _orderCompletedTopicProducer.Produce(new {context.Message.OrderId});

        if (context.RequestId != null)
            await context.RespondAsync<ShipmentDelivered>(new
            {
                InVar.Timestamp,
                context.Message.OrderId
            });
    }
}
