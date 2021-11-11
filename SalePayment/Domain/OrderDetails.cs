namespace SalePayment.Domain;

public class OrderDetails : EntityBase
{
    private OrderDetails() { }

    public OrderDetails(Guid orderId, Guid productId, decimal unitPrice, int quantity, float discount)
    {
        OrderId = orderId;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
        ProductInfoId = NewGuid();
        ProductInfo = new ProductInfo(productId);
    }

    public Guid OrderId { get; private set; } // don't link back to order, because its in one aggregation
    public Guid ProductInfoId { get; private set; }
    public ProductInfo ProductInfo { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public float Discount { get; private set; } // 0.1 = 10% discount
}
