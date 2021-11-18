using ProductCatalog.Data;
using ProductCatalog.Views;

namespace ProductCatalog.Consumers;

// listen to yourself pattern
public class ProductCdCConsumer :
    INotificationHandler<ProductCreated>
{
    private readonly MainDbContext _dbContext;

    public ProductCdCConsumer(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(ProductCreated @event, CancellationToken cancellationToken)
    {
        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == @event.ProductId.ConvertTo<Guid>(), cancellationToken: cancellationToken);

        if (existed is null)
        {
            var productView = new ProductView
            {
                ProductId = @event.ProductId.ConvertTo<Guid>(),
                ProductName = @event.ProductName,
                CategoryId = @event.CategoryId.ConvertTo<Guid>(),
                CategoryName = @event.CategoryName,
                SupplierId = @event.SupplierId.ConvertTo<Guid>(),
                SupplierName = @event.SupplierName
            };
            await _dbContext.Set<ProductView>().AddAsync(productView, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
