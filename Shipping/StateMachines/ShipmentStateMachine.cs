using Grpc.Net.ClientFactory;
using Northwind.IntegrationEvents.Protobuf.Audit.V1;

namespace Shipping.StateMachines;

public class ShipmentStateMachine : MassTransitStateMachine<ShipmentState>
{
    private readonly GrpcClientFactory _grpcClientFactory;

    public ShipmentStateMachine(GrpcClientFactory grpcClientFactory, ILoggerFactory loggerFactory)
    {
        _grpcClientFactory = grpcClientFactory;
        var logger = loggerFactory.CreateLogger(nameof(ShipmentStateMachine));

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

        InstanceState(x => x.CurrentState);

        Initially(
            When(ShipmentPrepared)
                .ThenAsync(async context =>
                {
                    context.Instance.Updated = DateTime.UtcNow;

                    logger.LogInformation("{OrderStateMachine} Init the state-machine for order={OrderId}",
                        $"ShipmentStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs("StateMachine.Started", context.Instance.CorrelationId);
                })
                .TransitionTo(Prepared)
        );

        During(Prepared,
            Ignore(ShipmentPrepared),
            When(ShipmentDispatched)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Shipment dispatched ok for order={OrderId}",
                        $"ShipmentStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .TransitionTo(Dispatched),
            When(ShipmentDispatchedFailed)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Shipment dispatched failed for order={OrderId}",
                        $"ShipmentStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatchedFailed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .Finalize());

        During(Dispatched,
            Ignore(ShipmentDispatched),
            When(ShipmentDelivered)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Shipment delivered ok for order={OrderId}",
                        $"ShipmentStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDispatched), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<OrderCompleted>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .Finalize(),
            When(ShipmentDeliveredFailed)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("{OrderStateMachine} Shipment delivered failed for order={OrderId}",
                        $"ShipmentStateMachine[{context.Instance.CorrelationId}]",
                        context.Instance.CorrelationId);

                    await SendAuditLogs(nameof(ShipmentDeliveredFailed), context.Instance.CorrelationId);
                })
                .Produce(context => context.Init<MoneyRefunded>(new
                {
                    OrderId = context.Instance.CorrelationId, context.Instance.TransactionId
                }))
                .Finalize());

        SetCompletedWhenFinalized();
    }

    // states
    public State Prepared { get; private set; } = null!;
    public State Dispatched { get; private set; } = null!;

    // events
    public Event<ShipmentPrepared> ShipmentPrepared { get; private set; } = null!;
    public Event<ShipmentDispatched> ShipmentDispatched { get; private set; } = null!;
    public Event<ShipmentDispatchedFailed> ShipmentDispatchedFailed { get; private set; } = null!;
    public Event<ShipmentDelivered> ShipmentDelivered { get; private set; } = null!;
    public Event<ShipmentDeliveredFailed> ShipmentDeliveredFailed { get; private set; } = null!;
    public Event<ShipmentCancelled> ShipmentCancelled { get; private set; } = null!;

    private async Task SendAuditLogs(string eventName, Guid correlationId, string description = "")
    {
        // only for demo; in reality, we should use pub-sub for more resiliency
        var auditorClient = _grpcClientFactory.CreateClient<Auditor.AuditorClient>("Auditor");
        await auditorClient.SubmitAuditAsync(new SubmitAuditRequest
        {
            Actor = $"[{nameof(ShipmentStateMachine)}]",
            Event = eventName,
            Status = eventName,
            AuditedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
            CorrelateId = correlationId.ToString(),
            Description = description
        });
    }
}
