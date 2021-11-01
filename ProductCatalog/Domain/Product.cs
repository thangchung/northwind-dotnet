namespace ProductCatalog.Domain;

public class Product : EntityRootBase
{
    private Product()
    {
    }

    public Product(string name, bool discontinued)
    {
        Name = name;
        Discontinued = discontinued;
    }

    public string Name { get; set; } = default!;
    public Guid? SupplierInfoId { get; private set; }
    public SupplierInfo? SupplierInfo { get; set; } = null!;
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; } = null!;
    public string? QuantityPerUnit { get; private set; } = default!;
    public decimal? UnitPrice { get; private set; } = default!;
    public int? UnitsInStock { get; private set; } = default!;
    public int? UnitsOnOrder { get; private set; } = default!;
    public int? ReorderLevel { get; private set; } = default!;
    public bool Discontinued { get; private set; } = default!;
}
