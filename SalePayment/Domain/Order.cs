namespace SalePayment.Domain;

public class Order : EntityRootBase
{
    private Order() {}

    public Guid CustomerInfoId { get; private set; }
    public CustomerInfo CustomerInfo { get; private set; }
    public Guid? EmployeeInfoId { get; private set; }
    public EmployeeInfo? EmployeeInfo { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public List<OrderDetails> OrderDetails { get; private set; } = new();
}
