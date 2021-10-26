namespace ProductCatalog.Domain;

public class SupplierInfo : EntityBase
{
    private SupplierInfo() { }

    public Guid SupplierId { get; private set; }
}
