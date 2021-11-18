namespace ProductCatalog.Views;

public class ProductView
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
}
