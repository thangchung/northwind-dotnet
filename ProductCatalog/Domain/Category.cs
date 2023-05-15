namespace ProductCatalog.Domain;

public class Category : EntityRootBase
{
    private Category() { }

    public Category(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; } = default!;
    public byte[]? Picture { get; private set; } = default!;
}
