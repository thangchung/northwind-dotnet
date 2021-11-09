namespace HumanResources.Domain;

public class Employee : EntityRootBase
{
    private Employee() {}

    public Employee(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string LastName { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string? Title { get; private set; } = default!;
    public string? TitleOfCourtesy { get; private set; } = default!;
    public DateTime? BirthDate { get; private set; }
    public DateTime? HireDate { get; private set; }
    public AddressInfo? AddressInfo { get; set; }
    public string? Extension { get; private set; } = default!;
    public byte[]? Photo { get; private set; }
    public string? Notes { get; private set; } = default!;
    public Guid? ReportsToId { get; private set; }
    public Employee? ReportsTo { get; private set; }
    public string? PhotoPath { get; private set; } = default!;

    public List<Territory> Territories { get; private set; } = new();
}
