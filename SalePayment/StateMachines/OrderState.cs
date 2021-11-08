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
