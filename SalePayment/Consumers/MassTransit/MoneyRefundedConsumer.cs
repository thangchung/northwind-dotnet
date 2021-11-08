namespace SalePayment.Consumers.MassTransit;

public class MoneyRefundedConsumer : IConsumer<MoneyRefunded>
{
    private readonly ITopicProducer<OrderCancelled> _orderCancelledTopicProducer;
    private readonly ILogger<MoneyRefundedConsumer> _logger;

    public MoneyRefundedConsumer(ITopicProducer<OrderCancelled> orderCancelledTopicProducer,
        ILogger<MoneyRefundedConsumer> logger)
    {
        _orderCancelledTopicProducer = orderCancelledTopicProducer;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MoneyRefunded> context)
    {
        _logger.LogInformation(
            $"Refund money for order=${context.Message.OrderId} of customer xxx on TransactionId xxx");

        // todo: compensation data
        // todo: submit claim money to payment gateway based on transaction_id

        await _orderCancelledTopicProducer.Produce(new
        {
            context.Message.OrderId,
            Reason = $"Refund money for order=${context.Message.OrderId} of customer xxx on TransactionId xxx"
        });

        await Task.CompletedTask;
    }
}
