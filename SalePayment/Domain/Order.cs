namespace SalePayment.Domain;

public class Order : EntityRootBase
{
    private Order() {}

    public Order(DateTime orderDate, Guid customerInfoId)
    {
        OrderDate = orderDate.ToDateTime();
        CustomerInfoId = customerInfoId;
    }

    public Guid CustomerInfoId { get; private set; }
    public CustomerInfo CustomerInfo { get; private set; } = null!;
    public Guid? EmployeeInfoId { get; private set; }
    public EmployeeInfo? EmployeeInfo { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? RequiredDate { get; private set; }
    public List<OrderDetails> OrderDetails { get; private set; } = new();

    public Order ExpectShipDate(DateTime? requiredDate)
    {
        if (requiredDate.HasValue)
        {
            RequiredDate = requiredDate.Value.ToDateTime();
        }

        return this;
    }

    public Order InsertOrderDetail(Guid orderId, Guid productId, decimal unitPrice, int quantity, float discount = 0.1f)
    {
        OrderDetails.Add(new OrderDetails(orderId, productId, unitPrice, quantity, discount));
        return this;
    }
}
