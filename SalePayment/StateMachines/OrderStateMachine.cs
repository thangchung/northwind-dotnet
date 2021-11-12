using Grpc.Net.ClientFactory;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace SalePayment.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly GrpcClientFactory _grpcClientFactory;
    private readonly ILogger _logger;

    public OrderStateMachine(GrpcClientFactory grpcClientFactory, ILoggerFactory logger)
    {
        _grpcClientFactory = grpcClientFactory;
        _logger = logger.CreateLogger(nameof(OrderStateMachine));

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

        Event(() => ShipmentPrepared, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDispatched, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDispatchedFailed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDelivered, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDeliveredFailed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentCancelled, x =>
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

                    _logger.LogInformation("{OrderStateMachine} Init the state-machine for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs("StateMachine.Started", context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MakeOrderValidated>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(OrderSubmittedState)
        );

        During(OrderSubmittedState,
            Ignore(OrderSubmitted),
            When(OrderValidated)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Validation okay for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderValidated), context.Instance.CorrelationId);
                })
                .TransitionTo(PaymentProcessedState),
            When(OrderValidatedFailed)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Validation failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderValidatedFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(OrderCancelledState));

        During(PaymentProcessedState,
            Ignore(OrderSubmitted),
            Ignore(OrderValidated),
            When(PaymentProcessed)
                .ThenAsync(async context =>
                {
                    //Console.WriteLine("Send notification for free-shipper or shipper hub.");
                    _logger.LogInformation("{OrderStateMachine} Payment okay for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(PaymentProcessed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<OrderConfirmed>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(OrderConfirmedState),
            When(PaymentProcessedFailed)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Payment failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(PaymentProcessedFailed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId //, context.Instance.TransactionId
                }))
                .TransitionTo(OrderCancelledState));

        During(OrderConfirmedState,
            Ignore(PaymentProcessed),
            When(ShipmentPrepared)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Order confirmed ok for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentPrepared), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentPreparedState));

        /* start shipping part */
        During(ShipmentPreparedState,
            Ignore(ShipmentPrepared),
            When(ShipmentDispatched)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Shipment dispatched ok for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentDispatchedState),
            When(ShipmentDispatchedFailed)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Shipment dispatched failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatchedFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentCancelledState));

        During(ShipmentDispatchedState,
            Ignore(ShipmentDispatched),
            When(ShipmentDelivered)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Shipment delivered ok for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .TransitionTo(OrderCompletedState),
            When(ShipmentDeliveredFailed)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Shipment delivered failed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDeliveredFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentCancelledState));

        During(ShipmentCancelledState,
            Ignore(OrderValidatedFailed),
            Ignore(ShipmentDispatchedFailed),
            Ignore(ShipmentDeliveredFailed),
            When(ShipmentCancelled)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Shipment cancelled for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentCancelled), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(OrderCancelledState));
        /* start shipping part */

        During(OrderCancelledState,
            Ignore(PaymentProcessedFailed),
            Ignore(ShipmentCancelled),
            When(OrderCancelled)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Order cancelled for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderCancelled), context.Instance.CorrelationId);
                })
                .Finalize());

        During(OrderCompletedState,
            Ignore(ShipmentDelivered),
            When(OrderCompleted)
                .ThenAsync(async context =>
                {
                    _logger.LogInformation("{OrderStateMachine} Order completed for order={OrderId}",
                        $"OrderStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(OrderCompleted), context.Instance.CorrelationId);
                })
                .Finalize());

        DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState
                }))
        );
    }

    public State OrderSubmittedState { get; private set; } = null!;
    public State PaymentProcessedState { get; private set; } = null!;
    public State OrderConfirmedState { get; private set; } = null!;
    public State ShipmentPreparedState { get; private set; } = null!;

    public State ShipmentDispatchedState { get; private set; } = null!;
    public State ShipmentCancelledState { get; private set; } = null!;
    public State OrderCompletedState { get; private set; } = null!;
    public State OrderCancelledState { get; private set; } = null!;

    public Event<OrderSubmitted> OrderSubmitted { get; private set; } = null!;
    public Event<OrderValidated> OrderValidated { get; private set; } = null!;
    public Event<OrderValidatedFailed> OrderValidatedFailed { get; private set; } = null!;
    public Event<PaymentProcessed> PaymentProcessed { get; private set; } = null!;
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; } = null!;
    public Event<OrderConfirmed> OrderConfirmed { get; private set; } = null!;
    public Event<ShipmentPrepared> ShipmentPrepared { get; private set; } = null!;

    public Event<ShipmentDispatched> ShipmentDispatched { get; private set; } = null!;
    public Event<ShipmentDispatchedFailed> ShipmentDispatchedFailed { get; private set; } = null!;
    public Event<ShipmentDelivered> ShipmentDelivered { get; private set; } = null!;
    public Event<ShipmentDeliveredFailed> ShipmentDeliveredFailed { get; private set; } = null!;
    public Event<ShipmentCancelled> ShipmentCancelled { get; private set; } = null!;

    public Event<OrderCompleted> OrderCompleted { get; private set; } = null!;
    public Event<OrderCancelled> OrderCancelled { get; private set; } = null!;
    public Event<CheckOrder> OrderStatusRequested { get; private set; } = null!;

    private async Task SendAuditLogs(string eventName, Guid correlationId, string description = "")
    {
        // only for demo; in reality, we should use pub-sub for more resiliency
        var auditorClient = _grpcClientFactory.CreateClient<Auditor.AuditorClient>("Auditor");
        await auditorClient.SubmitAuditAsync(new SubmitAuditRequest
        {
            Actor = "[order-state-machine]",
            Event = eventName,
            Status = eventName,
            AuditedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
            CorrelateId = correlationId.ToString(),
            Description = description
        });
    }
}
