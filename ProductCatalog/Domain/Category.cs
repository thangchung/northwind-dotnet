namespace ProductCatalog.Domain;

public class Category : EntityRootBase
{
    private Category() { }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; } = default!;
    public byte[]? Picture { get; private set; } = default!;
}
