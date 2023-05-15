namespace SalePayment.Domain;

public class EmployeeInfo : EntityBase
{
    private EmployeeInfo() { }

    public EmployeeInfo(Guid employeeId)
    {
        EmployeeId = employeeId;
    }

    public Guid EmployeeId { get; private set; }
}
