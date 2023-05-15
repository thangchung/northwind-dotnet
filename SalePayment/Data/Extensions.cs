using SalePayment.Domain;

namespace SalePayment.Data;

public static class Extensions
{
    public static async Task DoSeedData(this IApplicationBuilder app, ILogger logger)
    {
        var scope = app.ApplicationServices.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<MainDbContext>();
        // var sender = scope.ServiceProvider.GetService<ISender>();

        // seed data for product info
        if (await dbContext?.Set<ProductInfo>().CountAsync()! <= 0)
        {
            dbContext?.Set<ProductInfo>().AddRangeAsync(
                new ProductInfo(NewGuid("88a6d251-d2ba-420a-822b-8c68ec9ce4eb")),
                new ProductInfo(NewGuid("881897ec-d7be-40e0-974b-656eae8452b1")));
        }

        // seed data for customer info
        if (await dbContext?.Set<CustomerInfo>().CountAsync()! <= 0)
        {
            dbContext?.Set<CustomerInfo>().AddRangeAsync(
                new CustomerInfo(NewGuid("50f51e5f-36c6-4582-b428-ac15d70d9012")));
        }

        // seed data for employee info
        if (await dbContext?.Set<EmployeeInfo>().CountAsync()! <= 0)
        {
            dbContext?.Set<EmployeeInfo>().AddRangeAsync(
                new EmployeeInfo(NewGuid("a257f1e6-6dff-4848-ac75-eaf40e708584")));
        }

        await dbContext?.SaveChangesAsync()!;
    }
}
