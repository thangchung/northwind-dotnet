namespace SalePayment.Domain;

public class EmployeeInfo : EntityBase
{
    private EmployeeInfo() { }

    public Guid EmployeeId { get; private set; }
}
