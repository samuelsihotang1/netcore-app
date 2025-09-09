using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class DbSeeder
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();

            await db.Database.MigrateAsync();

            // ===== Roles (opsional) =====
            const string roleUser = "User";
            if (!await roleManager.Roles.AnyAsync())
            {
                await roleManager.CreateAsync(new IdentityRole<long>(roleUser));
            }

            // ===== Users =====
            var u1 = await EnsureUserAsync(userManager, "Andi User", "user1@shop.test", "P@ssw0rd!");
            var u2 = await EnsureUserAsync(userManager, "Budi User", "user2@shop.test", "P@ssw0rd!");
            var admin = await EnsureUserAsync(userManager, "Admin", "admin@shop.test", "P@ssw0rd!");
            await EnsureUserInRoleAsync(userManager, u1, roleUser);
            await EnsureUserInRoleAsync(userManager, u2, roleUser);
            await EnsureUserInRoleAsync(userManager, admin, roleUser);

            if (await db.Products.AnyAsync() && await db.Orders.AnyAsync()) return;

            // ===== Products =====
            var pKeyboard = await EnsureProductAsync(db, "SKU-001", "Mechanical Keyboard", 250000m, stock: 50, weight: 800, isActive: true);
            var pMouse    = await EnsureProductAsync(db, "SKU-002", "Gaming Mouse",       150000m, stock: 5,  weight: 120, isActive: true);
            var pHeadset  = await EnsureProductAsync(db, "SKU-003", "USB Headset",        200000m, stock: 10, weight: 250, isActive: true);
            var pOld      = await EnsureProductAsync(db, "SKU-004", "Old Product",        99000m,  stock: 20, weight: 300, isActive: false); // tidak aktif (uji filter)
            var pLow      = await EnsureProductAsync(db, "SKU-005", "Very Low Stock",     50000m,  stock: 1,  weight: 50,  isActive: true);  // stok kecil (uji gagal beli)

            // ===== Orders untuk u1 (menguji semua kondisi) =====
            // O1: DRAFT (tanpa shipment) — bisa diubah ke PAID untuk uji auto-buat shipment
            await EnsureOrderAsync(db, u1.Id, pKeyboard, status: "draft", qty: 1, shipping: 15000m, makeShipment: false);

            // O2: PAID + shipment IN_TRANSIT — uji halaman 4/5, dan perubahan delivery
            await EnsureOrderAsync(db, u1.Id, pMouse, status: "paid", qty: 2, shipping: 12000m, makeShipment: true, shipmentStatus: "in_transit");

            // O3: CANCELLED (tanpa shipment) — uji aturan "jika cancelled, tidak usah buat delivery"
            await EnsureOrderAsync(db, u1.Id, pHeadset, status: "cancelled", qty: 1, shipping: 18000m, makeShipment: false);

            // O4: COMPLETED — shipment DELIVERED (order otomatis completed)
            await EnsureOrderAsync(db, u1.Id, pKeyboard, status: "completed", qty: 1, shipping: 10000m, makeShipment: true, shipmentStatus: "delivered");

            // O5: PAID + shipment FAILED — uji skenario gagal kirim
            await EnsureOrderAsync(db, u1.Id, pHeadset, status: "paid", qty: 1, shipping: 15000m, makeShipment: true, shipmentStatus: "failed");

            // O6: DRAFT untuk produk stok kecil — uji nanti saat di-PAID akan gagal jika qty > stok
            await EnsureOrderAsync(db, u1.Id, pLow, status: "draft", qty: 2, shipping: 8000m, makeShipment: false); // stok 1, qty 2 → insufisien bila di-PAID

            // ===== Orders untuk u2 (variasi tambahan) =====
            await EnsureOrderAsync(db, u2.Id, pMouse, status: "paid", qty: 1, shipping: 9000m, makeShipment: true, shipmentStatus: "in_transit");
            await EnsureOrderAsync(db, u2.Id, pKeyboard, status: "draft", qty: 3, shipping: 15000m, makeShipment: false);

            await db.SaveChangesAsync();
        }

        // ================== Helpers ==================

        private static async Task<AppUser> EnsureUserAsync(UserManager<AppUser> userManager, string name, string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return user;

            user = new AppUser
            {
                Name = name,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to create seed user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return user;
        }

        private static async Task EnsureUserInRoleAsync(UserManager<AppUser> userManager, AppUser user, string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var res = await userManager.AddToRoleAsync(user, role);
                if (!res.Succeeded)
                    throw new InvalidOperationException("Failed to add user to role: " + string.Join(", ", res.Errors.Select(e => e.Description)));
            }
        }

        private static async Task<Product> EnsureProductAsync(AppDbContext db, string sku, string name, decimal price, int stock, int? weight, bool isActive)
        {
            var p = await db.Products.FirstOrDefaultAsync(x => x.Sku == sku);
            if (p != null) return p;

            p = new Product
            {
                Sku = sku,
                Name = name,
                Price = price,
                Stock = stock,
                WeightGram = weight,
                IsActive = isActive
            };
            db.Products.Add(p);
            await db.SaveChangesAsync();
            return p;
        }

        /// <summary>
        /// Membuat order sesuai status & shipment.
        /// - Jika status "paid" / "completed" dan makeShipment=true → kurangi stok produk.
        /// - Shipment status: in_transit | delivered | failed.
        /// - Jika delivered → set order.Status = completed (konsisten dgn aturan API).
        /// </summary>
        private static async Task EnsureOrderAsync(
            AppDbContext db,
            long userId,
            Product product,
            string status,
            int qty,
            decimal shipping,
            bool makeShipment,
            string? shipmentStatus = null)
        {
            var exists = await db.Orders.AnyAsync(o =>
                o.UserId == userId && o.ProductId == product.Id && o.Status == status && o.Qty == qty);
            if (exists) return;

            var order = new Order
            {
                UserId = userId,
                ProductId = product.Id,
                Status = status,
                Qty = qty,
                UnitPrice = product.Price,
                Subtotal = product.Price * qty,
                ShippingCost = shipping,
                GrandTotal = (product.Price * qty) + shipping
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            if ((status == "paid" || status == "completed") && makeShipment)
            {
                if (product.Stock < qty) product.Stock = 0;
                else product.Stock -= qty;

                var ship = new Shipment
                {
                    OrderId = order.Id,
                    Status = shipmentStatus ?? "in_transit",
                    Courier = "SEED-COURIER",
                    ShippedAt = DateTimeOffset.UtcNow.AddMinutes(-30)
                };

                if (ship.Status == "delivered")
                {
                    ship.DeliveredAt = DateTimeOffset.UtcNow;
                    order.Status = "completed";
                }

                db.Shipments.Add(ship);
                await db.SaveChangesAsync();
            }
        }
    }
}
