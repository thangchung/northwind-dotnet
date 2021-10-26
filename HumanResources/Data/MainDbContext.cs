using HumanResources.Domain;

namespace HumanResources.Data;

public class MainDbContext : AppDbContextBase
{
    private const string Schema = "human_resources";

    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = default!;
    public DbSet<Region> Regions { get; set; } = default!;
    public DbSet<Customer> Customers { get; set; } = default!;
    public DbSet<Supplier> Suppliers { get; set; } = default!;
    public DbSet<Shipper> Shippers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // employee
        modelBuilder.Entity<Employee>().ToTable("employees", Schema);
        modelBuilder.Entity<Employee>().HasKey(x => x.Id);
        modelBuilder.Entity<Employee>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Employee>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Employee>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Employee>().Ignore(x => x.DomainEvents);

        // regions
        modelBuilder.Entity<Region>().ToTable("regions", Schema);
        modelBuilder.Entity<Region>().HasKey(x => x.Id);
        modelBuilder.Entity<Region>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Region>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Region>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Region>().Ignore(x => x.DomainEvents);

        // territories
        modelBuilder.Entity<Territory>().ToTable("territories", Schema);
        modelBuilder.Entity<Territory>().HasKey(x => x.Id);
        modelBuilder.Entity<Territory>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Territory>().Property(x => x.Created);

        modelBuilder.Entity<Territory>().HasIndex(x => x.Id).IsUnique();

        // customers
        modelBuilder.Entity<Customer>().ToTable("customers", Schema);
        modelBuilder.Entity<Customer>().HasKey(x => x.Id);
        modelBuilder.Entity<Customer>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Customer>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Customer>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Customer>().Ignore(x => x.DomainEvents);

        // suppliers
        modelBuilder.Entity<Supplier>().ToTable("suppliers", Schema);
        modelBuilder.Entity<Supplier>().HasKey(x => x.Id);
        modelBuilder.Entity<Supplier>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Supplier>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Supplier>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Supplier>().Ignore(x => x.DomainEvents);

        // shippers
        modelBuilder.Entity<Shipper>().ToTable("shippers", Schema);
        modelBuilder.Entity<Shipper>().HasKey(x => x.Id);
        modelBuilder.Entity<Shipper>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Shipper>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Shipper>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Shipper>().Ignore(x => x.DomainEvents);

        // relationships
        modelBuilder.Entity<Employee>()
            .OwnsOne(x => x.AddressInfo);

        // ref https://docs.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key#join-entity-type-configuration
        modelBuilder.Entity<Employee>()
            .HasMany(x => x.Territories)
            .WithMany(x => x.Employees)
            .UsingEntity(x => x.ToTable("employee_territories"));

        modelBuilder.Entity<Employee>()
            .HasOne(x => x.ReportsTo);

        modelBuilder.Entity<Region>()
            .HasMany(x => x.Territories)
            .WithOne(x => x.Region);

        modelBuilder.Entity<Customer>()
            .OwnsOne(x => x.AddressInfo);

        modelBuilder.Entity<Supplier>()
            .OwnsOne(x => x.AddressInfo);
    }
}

