using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<long>, long>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Shipment> Shipments => Set<Shipment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(e =>
            {
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Product>(e =>
            {
                e.HasIndex(x => x.Sku).IsUnique();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Price).HasPrecision(14, 2);
                e.Property(x => x.Stock).HasDefaultValue(0);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Product_Stock_NonNegative", "[Stock] >= 0");
                    tb.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
                });
            });

            modelBuilder.Entity<Order>(e =>
            {
                e.HasOne(x => x.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.Product)
                    .WithMany()
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.UnitPrice).HasPrecision(14, 2);
                e.Property(x => x.Subtotal).HasPrecision(14, 2);
                e.Property(x => x.ShippingCost).HasPrecision(14, 2);
                e.Property(x => x.GrandTotal).HasPrecision(14, 2);

                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Order_Status",
                        "[Status] IN ('draft','paid','completed','cancelled')");
                    tb.HasCheckConstraint("CK_Order_Qty_Positive", "[Qty] > 0");
                    tb.HasCheckConstraint("CK_Order_UnitPrice_NonNegative", "[UnitPrice] >= 0");
                    tb.HasCheckConstraint("CK_Order_Subtotal_NonNegative", "[Subtotal] >= 0");
                    tb.HasCheckConstraint("CK_Order_ShippingCost_NonNegative", "[ShippingCost] >= 0");
                    tb.HasCheckConstraint("CK_Order_GrandTotal_NonNegative", "[GrandTotal] >= 0");
                });
            });

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
