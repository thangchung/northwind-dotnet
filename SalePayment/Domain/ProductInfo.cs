namespace SalePayment.Domain;

public class ProductInfo : EntityBase
{
    private ProductInfo() { }

    public ProductInfo(Guid productId)
    {
        ProductId = productId;
    }

    public Guid ProductId { get; private set; }
}
