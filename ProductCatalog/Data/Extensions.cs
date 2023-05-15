using ProductCatalog.Domain;
using ProductCatalog.UseCases;

namespace ProductCatalog.Data;

public static class Extensions
{
    public static async Task DoSeedData(this IApplicationBuilder app, ILogger logger)
    {
        var scope = app.ApplicationServices.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetService<MainDbContext>();
        //var sender = scope.ServiceProvider.GetService<ISender>();

        /*if (await dbContext?.Set<Product>().CountAsync()! <= 0)
        {
            _ = await sender?.Send(new MutateProduct.CreateCommand { Name = "test product" })!;
        }*/

        if (await dbContext?.Set<Category>().CountAsync()! <= 0)
        {
            await dbContext.Set<Category>().AddAsync(new Category("Category 1"));
            await dbContext.Set<Category>().AddAsync(new Category("Category 2"));
            await dbContext.Set<Category>().AddAsync(new Category("Category 3"));
            await dbContext.SaveChangesAsync();
        }
    }
}
