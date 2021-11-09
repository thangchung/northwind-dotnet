using HumanResources.Data;

namespace HumanResources;

public static class Extensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString("postgres"),
                options => options.UseModel(HumanResources.MainDbContextModel.Instance),
                svc => svc.AddRepository(typeof(Repository<>)))
            .AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }
}
