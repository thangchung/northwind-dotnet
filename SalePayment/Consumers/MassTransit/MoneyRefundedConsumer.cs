using Grpc.Net.ClientFactory;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace SalePayment.Consumers.MassTransit;

public class MoneyRefundedConsumer : IConsumer<MoneyRefunded>
{
    private readonly ITopicProducer<OrderCancelled> _orderCancelledTopicProducer;
    private readonly GrpcClientFactory _grpcClientFactory;
    private readonly ILogger<MoneyRefundedConsumer> _logger;

    public MoneyRefundedConsumer(ITopicProducer<OrderCancelled> orderCancelledTopicProducer,
        GrpcClientFactory grpcClientFactory,
        ILogger<MoneyRefundedConsumer> logger)
    {
        _orderCancelledTopicProducer = orderCancelledTopicProducer;
        _grpcClientFactory = grpcClientFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<MoneyRefunded> context)
    {
        _logger.LogInformation("{OrderStateMachine} Refund money for order={OrderId}",
            $"OrderStateMachine[{context.Message.OrderId}]",
            context.Message.OrderId);

        // todo: compensation data
        // todo: submit claim money to payment gateway based on transaction_id


        // send audit logs
        var auditorClient = _grpcClientFactory.CreateClient<Auditor.AuditorClient>("Auditor");
        await auditorClient.SubmitAuditAsync(new SubmitAuditRequest
        {
            Actor = "[order-state-machine]",
            Event = nameof(MoneyRefunded),
            Status = "Refunded",
            AuditedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
            CorrelateId = context.CorrelationId.ToString()
        });

        await _orderCancelledTopicProducer.Produce(new
        {
            context.Message.OrderId,
            Reason = $"Refund money for order=${context.Message.OrderId} of customer xxx on TransactionId xxx"
        });

        await Task.CompletedTask;
    }
}
