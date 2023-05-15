namespace HumanResources.Domain;

public class Shipper : EntityRootBase
{
    private Shipper() {}

    public Shipper(string companyName)
    {
        CompanyName = companyName;
    }

    public string CompanyName { get; private set; } = default!;
    public string? Phone { get; private set; } = default!;
}
