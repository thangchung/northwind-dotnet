using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;

namespace SalePayment.Consumers.MassTransit;

public class ProcessPaymentConsumerDefinition : ConsumerDefinition<ProcessPaymentConsumer>
{
    private readonly IServiceProvider _serviceProvider;

    public ProcessPaymentConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ConcurrentMessageLimit = 20;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ProcessPaymentConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        endpointConfigurator.UseServiceScope(_serviceProvider);
    }
}

public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
{
    private readonly ITopicProducer<PaymentProcessed> _paymentProcessedTopicProducer;
    private readonly ITopicProducer<PaymentProcessedFailed> _paymentProcessedFailedTopicProducer;

    public ProcessPaymentConsumer(ITopicProducer<PaymentProcessed> paymentProcessedTopicProducer,
        ITopicProducer<PaymentProcessedFailed> paymentProcessedFailedTopicProducer)
    {
        _paymentProcessedTopicProducer = paymentProcessedTopicProducer;
        _paymentProcessedFailedTopicProducer = paymentProcessedFailedTopicProducer;
    }

    public async Task Consume(ConsumeContext<ProcessPayment> context)
    {
        // for testing only
        if (context.Message.Description == "PaymentFailed")
        {
            await _paymentProcessedFailedTopicProducer.Produce(new {context.Message.OrderId});

            if (context.RequestId != null)
                await context.RespondAsync<PaymentProcessedFailed>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    Reason = $"Test cannot processing payment: {context.Message.OrderId}"
                });

            return;
        }

        await _paymentProcessedTopicProducer.Produce(new {context.Message.OrderId});

        // todo: send notification to shipper hub (list of order need to ship)
        // todo: in there some of shipper will pickup the ship order to ship
        // ...

        if (context.RequestId != null)
            await context.RespondAsync<PaymentProcessed>(new
            {
                InVar.Timestamp,
                context.Message.OrderId
            });
    }
}
