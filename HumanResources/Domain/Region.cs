namespace HumanResources.Domain;

public class Region : EntityRootBase
{
    private Region() { }

    public string Description { get; private set; } = default!;

    public List<Territory> Territories { get; private set; } = new();
}
