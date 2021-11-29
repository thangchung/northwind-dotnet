using Grpc.Net.ClientFactory;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace SalePayment.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly GrpcClientFactory _grpcClientFactory;

    public OrderStateMachine(GrpcClientFactory grpcClientFactory, ILoggerFactory loggerFactory)
    {
        _grpcClientFactory = grpcClientFactory;
        var logger = loggerFactory.CreateLogger(nameof(OrderStateMachine));

        Event(() => OrderSubmitted, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderValidated, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderValidatedFailed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentProcessed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentProcessed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentProcessedFailed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderConfirmed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderCompleted, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderCancelled, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderStatusRequested, x =>
        {
            x.CorrelateById(m => m.Message.OrderId);
            x.OnMissingInstance(m => m.ExecuteAsync(async context =>
            {
                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<OrderNotFound>(new {context.Message.OrderId});
                }
            }));
        });

        InstanceState(x => x.CurrentState);

        Initially(
            When(OrderSubmitted)
                .ThenAsync(async context =>
                {
                    context.Instance.Updated = DateTime.UtcNow;

                    logger.LogInformation("{OrderStateMachine} Init the state-machine for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs("StateMachine.Started", context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MakeOrderValidated>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(Submitted)
        );

        During(Submitted,
            Ignore(OrderSubmitted),
            When(OrderValidated)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Validation okay for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderValidated), context.Instance.CorrelationId);
                })
                .TransitionTo(Processed),
            When(OrderValidatedFailed)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Validation failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderValidatedFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(Cancelled));

        During(Processed,
            Ignore(OrderSubmitted),
            Ignore(OrderValidated),
            When(PaymentProcessed)
                .ThenAsync(async context =>
                {
                    //Console.WriteLine("Send notification for free-shipper or shipper hub.");
                    logger.LogInformation("{OrderStateMachine} Payment okay for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(PaymentProcessed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<OrderConfirmed>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(Confirmed),
            When(PaymentProcessedFailed)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Payment failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(PaymentProcessedFailed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId
                }))
                .TransitionTo(Cancelled));

        During(Confirmed,
            Ignore(PaymentProcessed),
            When(OrderCompleted)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Order completed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderCompleted), context.Instance.CorrelationId);
                })
                .Finalize());

        During(Cancelled,
            Ignore(OrderValidatedFailed), Ignore(PaymentProcessedFailed),
            When(OrderCancelled)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Order cancelled for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderCancelled), context.Instance.CorrelationId);
                })
                .Finalize());

        SetCompletedWhenFinalized();
    }

    public State Submitted { get; private set; } = null!;
    public State Processed { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State Cancelled { get; private set; } = null!;

    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;
    public Event<OrderValidated> OrderValidated { get; private set; } = null!;
    public Event<OrderValidatedFailed> OrderValidatedFailed { get; private set; } = null!;
    public Event<PaymentProcessed> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; } = null!;
    public Event<OrderConfirmed> OrderConfirmed { get; private set; } = null!;
    public Event<OrderCompleted> OrderCompleted { get; private set; } = null!;
    public Event<OrderCancelled> OrderCancelled { get; private set; } = null!;
    public Event<CheckOrder> OrderStatusRequested { get; private set; } = null!;

    private async Task SendAuditLogs(string eventName, Guid correlationId, string description = "")
    {
        // only for demo; in reality, we should use pub-sub for more resiliency
        var auditorClient = _grpcClientFactory.CreateClient<Auditor.AuditorClient>("Auditor");
        await auditorClient.SubmitAuditAsync(new SubmitAuditRequest
        {
            Actor = $"[{nameof(OrderStateMachine)}]",
            Event = eventName,
            Status = eventName,
            AuditedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
            CorrelateId = correlationId.ToString(),
            Description = description
        });
    }
}
