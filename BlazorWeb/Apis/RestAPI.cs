using RestEase;

namespace BlazorWeb.Apis;

public interface IAppApi
{
    [Get("product-api/v1/product-view/{page}/{pageSize}")]
    Task<ResultDto<List<ProductVM>>> GetProductsAsync([Path] int page, [Path] int pageSize);
}

public class ProductVM
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string SupplierName { get; set; }
}
public record ResultDto<T>(T Data, bool IsError = false, string ErrorMessage = default!) where T : notnull;
