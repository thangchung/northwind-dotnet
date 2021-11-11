using Grpc.Net.ClientFactory;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace SalePayment.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly GrpcClientFactory _grpcClientFactory;

    public OrderStateMachine(GrpcClientFactory grpcClientFactory)
    {
        _grpcClientFactory = grpcClientFactory;

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
                    Console.WriteLine($"[State Machine] init the state-machine for order={context.Instance.CorrelationId}.");
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
                    Console.WriteLine($"[State Machine] validate ok for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(OrderValidated), context.Instance.CorrelationId);
                })
                .TransitionTo(PaymentProcessedState),
            When(OrderValidatedFailed)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] validate failed for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(OrderValidatedFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(OrderCancelledState));

        During(PaymentProcessedState,
            Ignore(OrderValidated),
            When(PaymentProcessed)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] payment ok for order={context.Instance.CorrelationId}.");
                    //Console.WriteLine("Send notification for free-shipper or shipper hub.");
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
                    Console.WriteLine($"[State Machine] payment failed for order={context.Instance.CorrelationId}.");
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
                    Console.WriteLine($"[State Machine] order confirmed ok for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(ShipmentPrepared), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentPreparedState));

        /* start shipping part */
        During(ShipmentPreparedState,
            Ignore(ShipmentPrepared),
            When(ShipmentDispatched)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] shipment dispatched ok for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentDispatchedState),
            When(ShipmentDispatchedFailed)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] shipment dispatched failed for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(ShipmentDispatchedFailed), context.Instance.CorrelationId);
                })
                .TransitionTo(ShipmentCancelledState));

        During(ShipmentDispatchedState,
            Ignore(ShipmentDispatched),
            When(ShipmentDelivered)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] shipment delivered ok for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .TransitionTo(OrderCompletedState),
            When(ShipmentDeliveredFailed)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] shipment delivered failed for order={context.Instance.CorrelationId}.");
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
                    Console.WriteLine($"[State Machine] shipment cancelled for order={context.Instance.CorrelationId}.");
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
                    Console.WriteLine($"[State Machine] order cancelled for order={context.Instance.CorrelationId}.");
                    await SendAuditLogs(nameof(OrderCancelled), context.Instance.CorrelationId);
                })
                .Finalize());

        During(OrderCompletedState,
            Ignore(ShipmentDelivered),
            When(OrderCompleted)
                .ThenAsync(async context =>
                {
                    Console.WriteLine($"[State Machine] order completed for order={context.Instance.CorrelationId}.");
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
