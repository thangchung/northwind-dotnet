namespace ProductCatalog.UseCases;

public class Common
{
}

public record CategoryDto(Guid Id, string Name);

public record ProductDto(Guid Id, string Name, bool Discontinued)
{
    public CategoryDto? Category { get; set; }

    public ProductDto AssignCategory(CategoryDto categoryDto)
    {
        Category = categoryDto;
        return this;
    }
};
