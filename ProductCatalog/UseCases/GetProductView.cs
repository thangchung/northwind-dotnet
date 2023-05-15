using Dapper;
using ProductCatalog.Data;

namespace ProductCatalog.UseCases;

public class GetProductView : IQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public record struct ProductViewModel(Guid Id, string Name, string CategoryName, string SupplierName, long ItemCount);

    internal class Validator : AbstractValidator<GetProductView>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("PageSize should at least greater than or equal to 1.");
        }
    }

    internal class Handler : IRequestHandler<GetProductView, IResult>
    {
        private readonly MainDbContext _mainDbContext;

        public Handler(MainDbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        public async Task<IResult> Handle(GetProductView request, CancellationToken cancellationToken)
        {
            await using var conn = _mainDbContext.Database.GetDbConnection();
            await conn.OpenAsync(cancellationToken);
            var results = await conn.QueryAsync<ProductViewModel>(
                @"SELECT product_id ""Id"", product_name ""Name"", category_name CategoryName, supplier_name SupplierName, count(*) OVER() AS ItemCount
                    FROM product_catalog.product_views LIMIT @PageSize OFFSET ((@Page - 1) * @PageSize)",
                new {request.PageSize, request.Page}
            );

            return Results.Ok(ResultModel<List<ProductViewModel>>.Create(results.ToList()));
        }
    }
}
