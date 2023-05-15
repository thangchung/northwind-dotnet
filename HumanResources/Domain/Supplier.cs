namespace HumanResources.Domain;

public class Supplier : EntityRootBase
{
    private Supplier() { }

    public Supplier(string companyName)
    {
        CompanyName = companyName;
    }

    public string CompanyName { get; private set; } = default!;
    public string? ContactName { get; private set; } = default!;
    public string? ContactTitle { get; private set; } = default!;
    public AddressInfo? AddressInfo { get; set; }
    public string? HomePage { get; private set; } = default!;

    public Supplier ChangeCompanyName(string companyName)
    {
        CompanyName = companyName;
        return this;
    }
}
