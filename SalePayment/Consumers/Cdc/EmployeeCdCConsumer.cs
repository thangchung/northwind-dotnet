using SalePayment.Data;
using SalePayment.Domain;

namespace SalePayment.Consumers.Cdc;

public class EmployeeCdCConsumer :
    INotificationHandler<EmployeeCreated>,
    INotificationHandler<EmployeeUpdated>,
    INotificationHandler<EmployeeDeleted>
{
    private readonly MainDbContext _dbContext;

    public EmployeeCdCConsumer(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(EmployeeCreated @event, CancellationToken cancellationToken)
    {
        await _dbContext.Set<EmployeeInfo>().AddAsync(
            new EmployeeInfo(@event.Id.ConvertTo<Guid>()), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(EmployeeUpdated @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<EmployeeInfo>().FirstOrDefaultAsync(
            x=>x.EmployeeId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            // update something
            // ...

            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task Handle(EmployeeDeleted @event, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Set<EmployeeInfo>().FirstOrDefaultAsync(
            x=>x.EmployeeId == @event.Id.ConvertTo<Guid>(), cancellationToken: cancellationToken);
        if (entity is not null)
        {
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
