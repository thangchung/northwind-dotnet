namespace HumanResources.Domain;

public class AddressInfo : ValueObject
{
    public AddressInfo(string? address, string? city, string? region, string? postalCode, string? country,
        string? phone, string? fax)
    {
        Address = address;
        City = city;
        Region = region;
        PostalCode = postalCode;
        Country = country;
        Phone = phone;
        Fax = fax;
    }

    public string? Address { get; private set; } = default!;
    public string? City { get; private set; } = default!;
    public string? Region { get; private set; } = default!;
    public string? PostalCode { get; private set; } = default!;
    public string? Country { get; private set; } = default!;
    public string? Phone { get; private set; } = default!;
    public string? Fax { get; private set; } = default!;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return $"{Address}{City}{Region}{PostalCode}{Country}-{Phone}-{Fax}";
    }
}
