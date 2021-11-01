namespace ProductCatalog.Domain;

public class SupplierInfo : EntityBase
{
    private SupplierInfo() { }

    public SupplierInfo(Guid supplierId)
    {
        SupplierId = supplierId;
    }

    public Guid SupplierId { get; private set; }
}
