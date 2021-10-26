namespace HumanResources.Domain;

public class Supplier : EntityRootBase
{
    private Supplier() { }

    public string CompanyName { get; private set; } = default!;
    public string? ContactName { get; private set; } = default!;
    public string? ContactTitle { get; private set; } = default!;
    public AddressInfo? AddressInfo { get; set; }
    public string? HomePage { get; private set; } = default!;
}
