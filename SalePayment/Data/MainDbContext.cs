using SalePayment.Domain;
using SalePayment.Domain.Outbox;

namespace SalePayment.Data;

public class MainDbContext : AppDbContextBase
{
    private const string Schema = "sale_payment";

    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // order
        modelBuilder.Entity<Order>().ToTable("orders", Schema);
        modelBuilder.Entity<Order>().HasKey(x => x.Id);
        modelBuilder.Entity<Order>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<Order>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<Order>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<Order>().Ignore(x => x.DomainEvents);

        // order detail
        modelBuilder.Entity<OrderDetails>().ToTable("order_details", Schema);
        modelBuilder.Entity<OrderDetails>().HasKey(x => x.Id);
        modelBuilder.Entity<OrderDetails>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<OrderDetails>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<OrderDetails>().HasIndex(x => x.Id).IsUnique();

        // customer info
        modelBuilder.Entity<CustomerInfo>().ToTable("customers_info", Schema);
        modelBuilder.Entity<CustomerInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<CustomerInfo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<CustomerInfo>().Property(x => x.Created);

        modelBuilder.Entity<CustomerInfo>().HasIndex(x => x.Id).IsUnique();

        // employee info
        modelBuilder.Entity<EmployeeInfo>().ToTable("employees_info", Schema);
        modelBuilder.Entity<EmployeeInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<EmployeeInfo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<EmployeeInfo>().Property(x => x.Created);

        modelBuilder.Entity<EmployeeInfo>().HasIndex(x => x.Id).IsUnique();

        // product info
        modelBuilder.Entity<ProductInfo>().ToTable("products_info", Schema);
        modelBuilder.Entity<ProductInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<ProductInfo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<ProductInfo>().Property(x => x.Created);

        modelBuilder.Entity<ProductInfo>().HasIndex(x => x.Id).IsUnique();

        // relationships
        modelBuilder.Entity<Order>()
            .HasMany(x => x.OrderDetails);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.CustomerInfo);

        modelBuilder.Entity<Order>()
            .HasOne(x => x.EmployeeInfo);

        modelBuilder.Entity<OrderDetails>()
            .HasOne(x => x.ProductInfo);

        // outbox
        modelBuilder.Entity<OrderOutbox>().ToTable("order_outboxes", Schema);
        modelBuilder.Entity<OrderOutbox>().HasKey(x => x.Id);
        modelBuilder.Entity<OrderOutbox>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<OrderOutbox>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<OrderOutbox>().Ignore(x => x.Updated);
        modelBuilder.Entity<OrderOutbox>().Ignore(x => x.DomainEvents);
    }
}

