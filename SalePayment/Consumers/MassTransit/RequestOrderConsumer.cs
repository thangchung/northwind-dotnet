using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.KafkaIntegration;
using Northwind.IntegrationEvents.Contracts;

namespace SalePayment.Consumers.MassTransit;

public class RequestOrderConsumerDefinition : ConsumerDefinition<RequestOrderConsumer>
{
    private readonly IServiceProvider _serviceProvider;

    public RequestOrderConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ConcurrentMessageLimit = 20;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<RequestOrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        endpointConfigurator.UseServiceScope(_serviceProvider);
    }
}

public class RequestOrderConsumer : IConsumer<RequestOrder>
{
    private readonly ITopicProducer<OrderRequested> _orderRequestedTopicProducer;
    private readonly ILogger<RequestOrderConsumer> _logger;

    public RequestOrderConsumer(ITopicProducer<OrderRequested> orderRequestedTopicProducer,
        ILogger<RequestOrderConsumer> logger)
    {
        _orderRequestedTopicProducer = orderRequestedTopicProducer;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RequestOrder> context)
    {
        _logger?.Log(LogLevel.Debug, "RequestOrderConsumer: {CustomerId}", context.Message.CustomerId);

        // todo:
        // validation customer_id, employee_id using gRPC
        // check order date should greater than today
        // ...

        // for testing only
        if (context.Message.OrderId.Equals(new Guid("04134712-6546-4f55-a932-594c64d4910c")))
        {
            if (context.RequestId != null)
                await context.RespondAsync<OrderValidatedFailed>(new
                {
                    InVar.Timestamp,
                    context.Message.OrderId,
                    context.Message.CustomerId,
                    Reason = $"Test Customer cannot submit orders: {context.Message.CustomerId}"
                });

            return;
        }

        await _orderRequestedTopicProducer.Produce(new
        {
            context.Message.OrderId,
            context.Message.EmployeeId,
            context.Message.CustomerId,
            context.Message.OrderDate,
            context.Message.RequiredDate
        });

        if (context.RequestId != null)
            await context.RespondAsync<OrderValidated>(new {InVar.Timestamp, context.Message.OrderId});
    }
}
