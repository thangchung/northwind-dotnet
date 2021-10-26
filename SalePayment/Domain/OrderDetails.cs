namespace SalePayment.Domain;

public class OrderDetails : EntityBase
{
    private OrderDetails() { }

    public Guid OrderId { get; private set; } // don't link back to order, because its in one aggregation
    public Guid ProductInfoId { get; private set; }
    public ProductInfo ProductInfo { get; set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public float Discount { get; private set; } // 0.1 = 10% discount
}
