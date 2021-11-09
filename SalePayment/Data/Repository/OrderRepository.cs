using SalePayment.Domain;
using SalePayment.UseCases;

namespace SalePayment.Data.Repository;

public interface IOrderRepository
{
    Task<Order> AddAsync(SubmitOrder.Command orderCommand, CancellationToken cancellationToken = default);
}

internal class OrderRepository : RepositoryBase<MainDbContext, Order>, IOrderRepository
{
    public OrderRepository(MainDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Order> AddAsync(SubmitOrder.Command orderCommand, CancellationToken cancellationToken = default)
    {
        var customer = await DbContext.Set<CustomerInfo>()
            .FirstOrDefaultAsync(x => x.CustomerId == orderCommand.CustomerId, cancellationToken: cancellationToken);

        if (customer is null)
        {
            throw new ArgumentException("Customer couldn't find.");
        }

        // todo: hard-code for employee because we don't have auth just yet
        var employeeId = NewGuid("a257f1e6-6dff-4848-ac75-eaf40e708584");
        var employee = await DbContext.Set<EmployeeInfo>()
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId, cancellationToken: cancellationToken);

        if (employee is null)
        {
            throw new ArgumentException("Customer couldn't find.");
        }

        var order = new Order(orderCommand.OrderDate, customer.Id);
        order.ExpectShipDate(orderCommand.RequiredDate);

        foreach (var (productId, unitPrice, quantity) in orderCommand.Details)
        {
            order.InsertOrderDetail(order.Id, productId, unitPrice, quantity);
        }

        await DbSet.AddAsync(order, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}
