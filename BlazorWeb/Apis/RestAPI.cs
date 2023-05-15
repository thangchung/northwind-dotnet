using System.ComponentModel.DataAnnotations;
using RestEase;

namespace BlazorWeb.Apis;

public interface IAppApi
{
    [Get("product-api/v1/products/{id}")]
    Task<ResultDto<ProductVM>> GetProductAsync([Path] Guid id);

    [Get("product-api/v1/product-view/{page}/{pageSize}")]
    Task<ResultDto<List<ProductVM>>> GetProductsAsync([Path] int page, [Path] int pageSize);

    [Post("product-api/v1/products")]
    Task<ResultDto<ProductDto>> CreateProductAsync([Body] CreateProductModel model);

    [Put("product-api/v1/products/{id}")]
    Task<ResultDto<ProductDto>> EditProductAsync([Path] Guid id, [Body] CreateProductModel model);

    [Delete("product-api/v1/products/{id}")]
    Task<ResultDto<bool>> DeleteProductAsync([Path] Guid id);

    [Get("product-api/v1/categories")]
    Task<ResultDto<List<CategoryDto>>> GetCategoriesAsync();
}

public class ProductVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? SupplierId { get; set; }
    public string SupplierName { get; set; } = default!;
    public long ItemCount { get; set; }
}

public class CreateProductModel
{
    public Guid? Id { get; set; } // if not null, then this is edit form model
    [Required] public string Name { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? SupplierId { get; set; }
    public string? SupplierName { get; set; }
}

public record struct ProductDto(Guid Id, string Name, string? CategoryName, string? SupplierName);
public record struct CategoryDto(Guid Id, string Name);

public record ResultDto<T>(T Data, bool IsError = false, string ErrorMessage = default!) where T : notnull;
