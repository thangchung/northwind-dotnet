using Dapper;
using ProductCatalog.Data;

namespace ProductCatalog.UseCases;

public class GetProductView : IQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public record ProductViewModel(Guid ProductId, string ProductName, string CategoryName, string SupplierName)
    {
        public ProductViewModel() : this(default!, default!, default!, default!) { }
    }

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
                @"SELECT product_id ProductId, product_name ProductName, category_name CategoryName, supplier_name SupplierName
                    FROM product_catalog.product_views"
                );

            return Results.Ok(ResultModel<List<ProductViewModel>>.Create(results.ToList()));
        }
    }
}
