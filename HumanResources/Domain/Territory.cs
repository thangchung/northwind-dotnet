namespace HumanResources.Domain;

public class Territory : EntityBase
{
    private Territory() { }

    public string Description { get; private set; } = default!;

    public Guid RegionId { get; private set; }
    public Region Region { get; private set; }

    public List<Employee> Employees { get; private set; } = new();
}
