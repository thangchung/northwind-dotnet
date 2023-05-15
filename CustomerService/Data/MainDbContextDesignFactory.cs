using CustomerService.Data;

namespace HumanResources.Data;

public class MainDbContextDesignFactory : DbContextDesignFactoryBase<MainDbContext>
{
    public MainDbContextDesignFactory() : base("northwind_db")
    {
    }
}
