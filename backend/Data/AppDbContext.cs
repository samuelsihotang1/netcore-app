using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Shipment> Shipments => Set<Shipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =========================
            // Users
            // =========================
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique();

                // Default timestamp (SQL Server)
                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // =========================
            // Products
            // =========================
            modelBuilder.Entity<Product>(e =>
            {
                e.HasIndex(x => x.Sku).IsUnique();

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                e.Property(x => x.Price).HasPrecision(14, 2);
                e.Property(x => x.Stock).HasDefaultValue(0);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Product_Stock_NonNegative", "[Stock] >= 0");
                    tb.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
                });
            });

            // =========================
            // Orders
            // =========================
            modelBuilder.Entity<Order>(e =>
            {
                e.HasOne(x => x.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                e.Property(x => x.Subtotal).HasPrecision(14, 2);
                e.Property(x => x.ShippingCost).HasPrecision(14, 2);
                e.Property(x => x.GrandTotal).HasPrecision(14, 2);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Order_Status",
                        "[Status] IN ('draft','paid','completed','cancelled')");
                    tb.HasCheckConstraint("CK_Order_Subtotal_NonNegative", "[Subtotal] >= 0");
                    tb.HasCheckConstraint("CK_Order_ShippingCost_NonNegative", "[ShippingCost] >= 0");
                    tb.HasCheckConstraint("CK_Order_GrandTotal_NonNegative", "[GrandTotal] >= 0");
                });
            });

            // =========================
            // OrderItems
            // =========================
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.HasOne(x => x.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.UnitPrice).HasPrecision(14, 2);
                e.Property(x => x.LineTotal).HasPrecision(14, 2);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_OrderItem_Qty_Positive", "[Qty] > 0");
                    tb.HasCheckConstraint("CK_OrderItem_UnitPrice_NonNegative", "[UnitPrice] >= 0");
                    tb.HasCheckConstraint("CK_OrderItem_LineTotal_NonNegative", "[LineTotal] >= 0");
                });
            });

            // =========================
            // Shipments
            // =========================
            modelBuilder.Entity<Shipment>(e =>
            {
                e.HasOne(x => x.Order)
                    .WithMany(o => o.Shipments)
                    .HasForeignKey(x => x.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Shipment_Status",
                        "[Status] IN ('in_transit','delivered','failed')");
                });
            });
        }
    }
}
