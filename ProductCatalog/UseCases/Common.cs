namespace ProductCatalog.UseCases;

public record struct CategoryDto(Guid Id, string Name);

public record ProductDto(Guid Id, string Name, bool Discontinued)
{
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    public ProductDto AssignCategoryName(Guid? categoryId, string? categoryName)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        return this;
    }

    public ProductDto AssignSupplierName(Guid? supplierId, string? supplierName)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        return this;
    }
};
