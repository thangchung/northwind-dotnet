using HumanResources.Domain;
using HumanResources.UseCases;

namespace HumanResources.Data;

public static class Extensions
{
    public static async Task DoSeedData(this IApplicationBuilder app, ILogger logger)
    {
        var scope = app.ApplicationServices.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<MainDbContext>();
        var sender = scope.ServiceProvider.GetService<ISender>();

        if (await dbContext?.Set<Supplier>().CountAsync()! <= 0)
        {
            _ = await sender?.Send(new MutateSupplier.CreateCommand { CompanyName = "Company A" })!;
            _ = await sender?.Send(new MutateSupplier.CreateCommand { CompanyName = "Company B" })!;
            _ = await sender?.Send(new MutateSupplier.CreateCommand { CompanyName = "Company C" })!;
        }

        if (await dbContext?.Set<Customer>().CountAsync()! <= 0)
        {
            _ = await sender?.Send(new MutateCustomer.CreateCommand { CompanyName = "Customer Company A" })!;
            _ = await sender?.Send(new MutateCustomer.CreateCommand { CompanyName = "Customer Company B" })!;
            _ = await sender?.Send(new MutateCustomer.CreateCommand { CompanyName = "Customer Company C" })!;
        }

        if (await dbContext?.Set<Employee>().CountAsync()! <= 0)
        {
            _ = await sender?.Send(new MutateEmployee.CreateCommand { FirstName = "Nguyen Van", LastName = "A"})!;
        }

        if (await dbContext?.Set<Shipper>().CountAsync()! <= 0)
        {
            _ = await sender?.Send(new MutateShipper.CreateCommand { CompanyName = "Shipper Company A"})!;
            _ = await sender?.Send(new MutateShipper.CreateCommand { CompanyName = "Shipper Company B"})!;
            _ = await sender?.Send(new MutateShipper.CreateCommand { CompanyName = "Shipper Company C"})!;
            _ = await sender?.Send(new MutateShipper.CreateCommand { CompanyName = "Shipper Company D"})!;
            _ = await sender?.Send(new MutateShipper.CreateCommand { CompanyName = "Shipper Company E"})!;
        }

        await dbContext.SaveChangesAsync();
    }
}
