namespace SalePayment.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
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
                .Then(context =>
                {
                    context.Instance.Updated = DateTime.UtcNow;
                    Console.WriteLine($"[State Machine] init the state-machine for order={context.Instance.CorrelationId}.");
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
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] validate ok for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(PaymentProcessedState),
            When(OrderValidatedFailed)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] validate failed for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(OrderCancelledState));

        During(PaymentProcessedState,
            /*Ignore(OrderValidated), Ignore(OrderValidatedFailed),*/
            When(PaymentProcessed)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] payment ok for order={context.Instance.CorrelationId}.");
                    //Console.WriteLine("Send notification for free-shipper or shipper hub.");
                })
                .Produce(context => context.Init<OrderConfirmed>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(OrderConfirmedState),
            When(PaymentProcessedFailed)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] payment failed for order={context.Instance.CorrelationId}.");
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId //, context.Instance.TransactionId
                }))
                .TransitionTo(OrderCancelledState));

        During(OrderConfirmedState,
            Ignore(PaymentProcessed),
            When(ShipmentPrepared)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] order confirmed ok for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(ShipmentPreparedState));

        /* start shipping part */
        During(ShipmentPreparedState,
            Ignore(OrderValidated), Ignore(PaymentProcessed), Ignore(ShipmentPrepared),
            When(ShipmentDispatched)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] shipment dispatched ok for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(ShipmentDispatchedState),
            When(ShipmentDispatchedFailed)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] shipment dispatched failed for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(ShipmentCancelledState));

        During(ShipmentDispatchedState,
            Ignore(ShipmentDispatched),
            When(ShipmentDelivered)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] shipment delivered ok for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(OrderCompletedState),
            When(ShipmentDeliveredFailed)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] shipment delivered failed for order={context.Instance.CorrelationId}.");
                })
                .TransitionTo(ShipmentCancelledState));

        During(ShipmentCancelledState,
            Ignore(ShipmentDispatchedFailed),
            Ignore(ShipmentDeliveredFailed),
            When(ShipmentCancelled)
                .Then(context =>
                {
                    Console.WriteLine($"[State Machine] shipment cancelled for order={context.Instance.CorrelationId}.");
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .TransitionTo(OrderCancelledState));
        /* start shipping part */

        During(OrderCancelledState,
            When(OrderCancelled)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[State Machine] order cancelled for order={context.Instance.CorrelationId}.");
                })
                .Finalize());

        During(OrderCompletedState,
            When(OrderCompleted)
                .Then(context =>
                {
                    Console.WriteLine(
                        $"[State Machine] order completed for order={context.Instance.CorrelationId}.");
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

    public State OrderSubmittedState { get; private set; }
    public State PaymentProcessedState { get; private set; }
    public State OrderConfirmedState { get; private set; }
    public State ShipmentPreparedState { get; private set; }

    public State ShipmentDispatchedState { get; private set; }
    public State ShipmentCancelledState { get; private set; }
    public State OrderCompletedState { get; private set; }
    public State OrderCancelledState { get; private set; }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderValidated> OrderValidated { get; private set; }
    public Event<OrderValidatedFailed> OrderValidatedFailed { get; private set; }
    public Event<PaymentProcessed> PaymentProcessed { get; private set; }
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; }
    public Event<OrderConfirmed> OrderConfirmed { get; private set; }
    public Event<ShipmentPrepared> ShipmentPrepared { get; private set; }

    public Event<ShipmentDispatched> ShipmentDispatched { get; private set; }
    public Event<ShipmentDispatchedFailed> ShipmentDispatchedFailed { get; private set; }
    public Event<ShipmentDelivered> ShipmentDelivered { get; private set; }
    public Event<ShipmentDeliveredFailed> ShipmentDeliveredFailed { get; private set; }
    public Event<ShipmentCancelled> ShipmentCancelled { get; private set; }

    public Event<OrderCompleted> OrderCompleted { get; private set; }
    public Event<OrderCancelled> OrderCancelled { get; private set; }
    public Event<CheckOrder> OrderStatusRequested { get; private set; }
}
