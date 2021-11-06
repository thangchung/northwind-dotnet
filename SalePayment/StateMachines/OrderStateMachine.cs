using Automatonymous;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;
using Northwind.IntegrationEvents.Contracts;

namespace SalePayment.StateMachines;

public class OrderState :
    SagaStateMachineInstance,
    ISagaVersion
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }

    public string CurrentState { get; set; } = default!;
    public string FaultReason { get; set; }

    public Guid OrderId { get; set; }

    public Guid CustomerId { get; set; }
    public Guid? EmployeeId { get; set;}
    public DateTime OrderDate { get; set;}
    public DateTime? RequiredDate { get; set; }

    public decimal TotalMoney { get; set; }
    public string TransactionId { get; set; }

    public DateTime? Updated { get; set; }
}

public class OrderStateMachineDefinition : SagaDefinition<OrderState>
{
    public OrderStateMachineDefinition()
    {
        ConcurrentMessageLimit = 12;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 5000, 10000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        Event(() => OrderRequested, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentProcessed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => PaymentProcessedFailed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderConfirmed, x =>
            x.CorrelateById(m => m.Message.OrderId));

        /*Event(() => ShipmentPrepared, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDispatched, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => ShipmentDelivered, x =>
            x.CorrelateById(m => m.Message.OrderId));

        Event(() => OrderCompleted, x =>
            x.CorrelateById(m => m.Message.OrderId));*/

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
            When(OrderRequested)
                .Then(context =>
                {
                    context.Instance.Updated = DateTime.UtcNow;
                })
                .TransitionTo(PaymentProcessedState));

        During(PaymentProcessedState,
            Ignore(OrderRequested),
            When(PaymentProcessed)
                .TransitionTo(OrderConfirmedState),
            When(PaymentProcessedFailed) // compensation when payment check failed flow
                .Then(context =>
                {
                    context.Instance.FaultReason = "Failed to processing payment.";
                    Console.WriteLine("Failed to processing payment.");
                })
                .Produce(context => context.Init<CompensatePaymentFailProcessed>(new
                {
                    OrderId = context.Instance.CorrelationId
                }))
                .TransitionTo(OrderFailedState));

        DuringAny(
            When(OrderStatusRequested)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {
                    OrderId = x.Instance.CorrelationId,
                    State = x.Instance.CurrentState
                }))
        );
    }

    public State PaymentProcessedState { get; private set; }
    public State OrderConfirmedState { get; private set; }
    /*public State ShipmentPreparedState { get; private set; }
    public State ShipmentDispatchedState { get; private set; }
    public State ShipmentDeliveredState { get; private set; }
    public State OrderCompletedState { get; private set; }*/
    public State OrderFailedState { get; private set; }

    public Event<OrderRequested> OrderRequested { get; private set; }
    public Event<PaymentProcessed> PaymentProcessed { get; private set; }
    public Event<PaymentProcessedFailed> PaymentProcessedFailed { get; private set; }
    public Event<OrderConfirmed> OrderConfirmed { get; private set; }
    /*public Event<ShipmentPrepared> ShipmentPrepared { get; private set; }
    public Event<ShipmentDispatched> ShipmentDispatched { get; private set; }
    public Event<ShipmentDelivered> ShipmentDelivered { get; private set; }
    public Event<OrderCompleted> OrderCompleted { get; private set; }*/
    public Event<CheckOrder> OrderStatusRequested { get; private set; }
}
