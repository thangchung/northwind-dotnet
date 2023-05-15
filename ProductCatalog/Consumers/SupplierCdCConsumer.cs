using ProductCatalog.Data;
using ProductCatalog.Domain;

namespace ProductCatalog.Consumers;

public class SupplierCdCConsumer :
    INotificationHandler<SupplierCreated>,
    INotificationHandler<SupplierUpdated>,
    INotificationHandler<SupplierDeleted>
{
    private readonly MainDbContext _dbContext;

    public SupplierCdCConsumer(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(SupplierCreated @event, CancellationToken cancellationToken)
    {
        await _dbContext.Set<SupplierInfo>().AddAsync(
            new SupplierInfo(@event.Id.ConvertTo<Guid>()), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(SupplierUpdated @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<SupplierInfo>().FirstOrDefaultAsync(
            x=>x.SupplierId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            // update something
            // ...

            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task Handle(SupplierDeleted @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<SupplierInfo>().FirstOrDefaultAsync(
            x=>x.SupplierId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
