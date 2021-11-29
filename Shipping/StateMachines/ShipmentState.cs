namespace Shipping.StateMachines;

public class ShipmentState :
    SagaStateMachineInstance,
    ISagaVersion
{
    [BsonId]
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
    public string CurrentState { get; set; } = default!;
    public string FaultReason { get; set; } = null!;

    public string TransactionId { get; set; } = null!;
    public DateTime? Updated { get; set; }
}
