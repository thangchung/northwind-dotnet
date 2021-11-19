using ProductCatalog.Data;
using ProductCatalog.Views;

namespace ProductCatalog.Consumers;

// listen to yourself pattern
public class ProductCdCConsumer :
    INotificationHandler<ProductCreated>,
    INotificationHandler<ProductUpdated>,
    INotificationHandler<ProductDeleted>
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

    public async Task Handle(ProductUpdated @event, CancellationToken cancellationToken)
    {
        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == @event.ProductId.ConvertTo<Guid>(), cancellationToken: cancellationToken);

        if (existed is not null)
        {
            existed.ProductName = @event.ProductName;
            existed.CategoryId = @event.CategoryId.ConvertTo<Guid>();
            existed.CategoryName = @event.CategoryName;
            existed.SupplierId = @event.SupplierId.ConvertTo<Guid>();
            existed.SupplierName = @event.SupplierName;
            _dbContext.Set<ProductView>().Update(existed);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task Handle(ProductDeleted @event, CancellationToken cancellationToken)
    {
        var existed = await _dbContext.Set<ProductView>()
            .FirstOrDefaultAsync(x => x.ProductId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);

        if (existed is not null)
        {
            _dbContext.Set<ProductView>().Remove(existed);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
