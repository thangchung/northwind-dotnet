using SalePayment.Data;
using SalePayment.Domain;

namespace SalePayment.Consumers.Cdc;

public class CustomerCdCConsumer :
    INotificationHandler<CustomerCreated>,
    INotificationHandler<CustomerUpdated>,
    INotificationHandler<CustomerDeleted>
{
    private readonly MainDbContext _dbContext;

    public CustomerCdCConsumer(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(CustomerCreated @event, CancellationToken cancellationToken)
    {
        await _dbContext.Set<CustomerInfo>().AddAsync(
            new CustomerInfo(@event.Id.ConvertTo<Guid>()), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(CustomerUpdated @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<CustomerInfo>().FirstOrDefaultAsync(
            x=>x.CustomerId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            // update something
            // ...

            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task Handle(CustomerDeleted @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<CustomerInfo>().FirstOrDefaultAsync(
            x=>x.CustomerId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
