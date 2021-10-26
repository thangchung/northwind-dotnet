using Shipping.Domain;

namespace Shipping.Data;

public class MainDbContext : AppDbContextBase
{
    private const string Schema = "shipping";

    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ShippingOrder> ShippingOrders { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(Consts.UuidGenerator);

        // shipping order
        modelBuilder.Entity<ShippingOrder>().ToTable("shipping_orders", Schema);
        modelBuilder.Entity<ShippingOrder>().HasKey(x => x.Id);
        modelBuilder.Entity<ShippingOrder>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<ShippingOrder>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<ShippingOrder>().HasIndex(x => x.Id).IsUnique();
        modelBuilder.Entity<ShippingOrder>().Ignore(x => x.DomainEvents);

        // state
        modelBuilder.Entity<State>().ToTable("states", Schema);
        modelBuilder.Entity<State>().HasKey(x => x.Id);
        modelBuilder.Entity<State>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<State>().Property(x => x.Created).HasDefaultValueSql(Consts.DateAlgorithm);

        modelBuilder.Entity<State>().HasIndex(x => x.Id).IsUnique();

        // shipper info
        modelBuilder.Entity<ShipperInfo>().ToTable("shippers_info", Schema);
        modelBuilder.Entity<ShipperInfo>().HasKey(x => x.Id);
        modelBuilder.Entity<ShipperInfo>().Property(x => x.Id).HasColumnType("uuid")
            .HasDefaultValueSql(Consts.UuidAlgorithm);

        modelBuilder.Entity<ShipperInfo>().Property(x => x.Created);

        modelBuilder.Entity<ShipperInfo>().HasIndex(x => x.Id).IsUnique();

        // relationships
        modelBuilder.Entity<ShippingOrder>()
            .HasOne(x => x.ShipperInfo);
    }
}

