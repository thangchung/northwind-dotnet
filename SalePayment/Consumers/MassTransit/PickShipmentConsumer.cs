using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;
using Northwind.IntegrationEvents.ViewModels;

namespace SalePayment.Consumers.MassTransit;

public class PickShipmentConsumerDefinition : ConsumerDefinition<PickShipmentConsumer>
{
    private readonly IServiceProvider _serviceProvider;

    public PickShipmentConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ConcurrentMessageLimit = 20;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<PickShipmentConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        endpointConfigurator.UseServiceScope(_serviceProvider);
    }
}

public class PickShipmentConsumer : IConsumer<PickShipmentByShipper>
{
    private readonly ITopicProducer<ShipmentDispatched> _shipmentDispatchedTopicProducer;
    private readonly ITopicProducer<ShipmentDispatchedFailed> _shipmentDispatchedFailedTopicProducer;
    private readonly ITopicProducer<ShipmentCancelled> _shipmentCancelledTopicProducer;

    public PickShipmentConsumer(
        ITopicProducer<ShipmentDispatched> shipmentDispatchedTopicProducer,
        ITopicProducer<ShipmentDispatchedFailed> shipmentDispatchedFailedTopicProducer,
        ITopicProducer<ShipmentCancelled> shipmentCancelledTopicProducer)
    {
        _shipmentDispatchedTopicProducer = shipmentDispatchedTopicProducer;
        _shipmentDispatchedFailedTopicProducer = shipmentDispatchedFailedTopicProducer;
        _shipmentCancelledTopicProducer = shipmentCancelledTopicProducer;
    }

    public async Task Consume(ConsumeContext<PickShipmentByShipper> context)
    {
        // for testing only
        if (context.Message.BeFailedAt == "Dispatched")
        {
            await _shipmentDispatchedFailedTopicProducer.Produce(new {context.Message.OrderId});
            await _shipmentCancelledTopicProducer.Produce(new {context.Message.OrderId});

            if (context.RequestId != null)
                await context.RespondAsync<ShipmentDispatchedFailed>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    Reason = $"Test cannot dispatch the packages: {context.Message.OrderId}"
                });

            return;
        }

        await _shipmentDispatchedTopicProducer.Produce(new {context.Message.OrderId});

        if (context.RequestId != null)
            await context.RespondAsync<ShipmentDispatched>(new
            {
                InVar.Timestamp,
                context.Message.OrderId
            });
    }
}
