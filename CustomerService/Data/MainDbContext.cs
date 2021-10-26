using CustomerService.Domain;

namespace CustomerService.Data;

public class MainDbContext : AppDbContextBase
{
    private const string Schema = "customer_service";

    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<CustomerDemographic> CustomerDemographics { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // customer demographic
        modelBuilder.Entity<CustomerDemographic>().ToTable("customer_demographics", Schema);
        modelBuilder.Entity<CustomerDemographic>().HasKey(x => x.Id);
        modelBuilder.Entity<CustomerDemographic>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<CustomerDemographic>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<CustomerDemographic>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<CustomerDemographic>().Ignore(x => x.DomainEvents);

        // customer info
        modelBuilder.Entity<CustomerInfo>().ToTable("customers_info", Schema);
        modelBuilder.Entity<CustomerInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<CustomerInfo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<CustomerInfo>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<CustomerInfo>().HasIndex(x => x.Id).IsUnique();

        // customer customer demographic
        modelBuilder.Entity<CustomerCustomerDemo>().ToTable("customer_customer_demo", Schema);
        modelBuilder.Entity<CustomerCustomerDemo>().HasKey(x => x.Id);
        modelBuilder.Entity<CustomerCustomerDemo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<CustomerCustomerDemo>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<CustomerCustomerDemo>().HasIndex(x => x.Id).IsUnique();

        // relationships
        modelBuilder.Entity<CustomerDemographic>()
            .HasMany(x => x.CustomerCustomerDemos);
    }
}

