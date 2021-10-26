namespace SalePayment.Domain;

public class ProductInfo : EntityBase
{
    private ProductInfo() { }

    public Guid ProductId { get; private set; }
}
