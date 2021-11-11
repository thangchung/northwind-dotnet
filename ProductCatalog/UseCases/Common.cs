namespace ProductCatalog.UseCases;

public record struct CategoryDto(Guid Id, string Name);

public record struct ProductDto(Guid Id, string Name, bool Discontinued)
{
    public CategoryDto? Category { get; set; } = null;

    public ProductDto AssignCategory(CategoryDto categoryDto)
    {
        Category = categoryDto;
        return this;
    }
};
