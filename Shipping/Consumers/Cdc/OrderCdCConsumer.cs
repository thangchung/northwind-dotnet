using Shipping.Data;
using Shipping.Domain;

namespace Shipping.Consumers.Cdc;

public class OrderCdCConsumer :
    INotificationHandler<OrderCreated>
{
    private readonly MainDbContext _dbContext;

    public OrderCdCConsumer(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(OrderCreated @event, CancellationToken cancellationToken)
    {
        await _dbContext.Set<ShippingOrder>().AddAsync(
            new ShippingOrder(@event.Id.ConvertTo<Guid>()), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
