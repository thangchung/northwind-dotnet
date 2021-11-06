using SalePayment.Data;
using SalePayment.Domain;

namespace SalePayment.Consumers.Cdc;

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
        await _dbContext.Set<ProductInfo>().AddAsync(
            new ProductInfo(@event.Id.ConvertTo<Guid>()), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ProductUpdated @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<ProductInfo>().FirstOrDefaultAsync(
            x=>x.ProductId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            // update something
            // ...

            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task Handle(ProductDeleted @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<ProductInfo>().FirstOrDefaultAsync(
            x=>x.ProductId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
