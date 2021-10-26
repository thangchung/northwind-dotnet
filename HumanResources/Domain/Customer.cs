namespace HumanResources.Domain;

public class Customer : EntityRootBase
{
    private Customer() { }

    public string CompanyName { get; private set; } = default!;
    public string? ContactName { get; private set; } = default!;
    public string? ContactTitle { get; private set; } = default!;
    public AddressInfo? AddressInfo { get; set; }
}
