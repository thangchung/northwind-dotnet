namespace SalePayment.Consumers.MassTransit;

public class MakeOrderValidatedConsumer : IConsumer<MakeOrderValidated>
{
    private readonly ITopicProducer<OrderValidated> _orderValidatedTopicProducer;
    private readonly ITopicProducer<OrderValidatedFailed> _orderValidatedFailedTopicProducer;
    private readonly ILogger<MakeOrderValidatedConsumer> _logger;

    public MakeOrderValidatedConsumer(
        ITopicProducer<OrderValidated> orderValidatedTopicProducer,
        ITopicProducer<OrderValidatedFailed> orderValidatedFailedTopicProducer,
        ILogger<MakeOrderValidatedConsumer> logger)
    {
        _orderValidatedTopicProducer = orderValidatedTopicProducer;
        _orderValidatedFailedTopicProducer = orderValidatedFailedTopicProducer;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MakeOrderValidated> context)
    {
        _logger.LogInformation(
            "{OrderStateMachine} Validate the order={OrderId}",
            $"OrderStateMachine[{context.Message.OrderId}]",
            context.Message.OrderId);

        await _orderValidatedTopicProducer.Produce(new
        {
            context.Message.OrderId,
        });

        await Task.CompletedTask;
    }
}
