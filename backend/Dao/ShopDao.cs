using backend.Data;
using backend.Dto;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using backend.Interface;

namespace backend.Dao
{
    public class ShopDao : IShopDao
    {
        private readonly AppDbContext _db;
        public ShopDao(AppDbContext db) => _db = db;

        public async Task<List<ProductListItemDto>> GetAllProductsAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<BuyProductResultDto> BuyAsync(long userId, BuyProductRequestDto dto)
        {
            if (dto.Qty <= 0) throw new ArgumentException("Qty must be > 0");
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
            if (product == null) throw new InvalidOperationException("Product not found/active");
            if (product.Stock < dto.Qty) throw new InvalidOperationException("Insufficient stock");

            var unitPrice = product.Price;
            var subtotal = unitPrice * dto.Qty;
            var grand = subtotal + dto.ShippingCost;

            using var tx = await _db.Database.BeginTransactionAsync();

            var order = new Order
            {
                UserId = userId,
                ProductId = product.Id,
                Status = "paid",
                Qty = dto.Qty,
                UnitPrice = unitPrice,
                Subtotal = subtotal,
                ShippingCost = dto.ShippingCost,
                GrandTotal = grand
            };
            _db.Orders.Add(order);

            product.Stock -= dto.Qty;
            if (product.Stock < 0) throw new InvalidOperationException("Concurrent stock error");

            await _db.SaveChangesAsync();

            var shipment = new Shipment
            {
                OrderId = order.Id,
                Status = "in_transit",
                Courier = dto.Courier,
                ShippedAt = DateTimeOffset.UtcNow
            };
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return new BuyProductResultDto
            {
                OrderId = order.Id,
                OrderStatus = order.Status,
                Qty = order.Qty,
                UnitPrice = order.UnitPrice,
                Subtotal = order.Subtotal,
                ShippingCost = order.ShippingCost,
                GrandTotal = order.GrandTotal,
                Shipment = new ShipmentBriefDto { Id = shipment.Id, Status = shipment.Status }
            };
        }

        public async Task<List<OrderSummaryDto>> GetUserOrdersAsync(long userId)
        {
            return await _db.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    Status = o.Status,
                    ProductId = o.ProductId,
                    ProductName = o.Product.Name,
                    Qty = o.Qty,
                    UnitPrice = o.UnitPrice,
                    Subtotal = o.Subtotal,
                    ShippingCost = o.ShippingCost,
                    GrandTotal = o.GrandTotal,
                    CreatedAt = o.CreatedAt,
                    ShipmentStatus = o.Shipments.OrderBy(s => s.Id).Select(s => s.Status).FirstOrDefault(),
                    ShipmentId = o.Shipments.OrderBy(s => s.Id).Select(s => (long?)s.Id).FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(long userId, long orderId, string status)
        {
            var allowed = new[] { "draft", "paid", "completed", "cancelled" };
            if (!allowed.Contains(status)) throw new ArgumentException("Invalid order status");

            var order = await _db.Orders.Include(o => o.Shipments)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null) return false;

            if (status == "paid" && !order.Shipments.Any())
            {
                var product = await _db.Products.FirstAsync(p => p.Id == order.ProductId);
                if (product.Stock < order.Qty) throw new InvalidOperationException("Insufficient stock");
                product.Stock -= order.Qty;

                var shipment = new Shipment
                {
                    OrderId = order.Id,
                    Status = "in_transit",
                    ShippedAt = DateTimeOffset.UtcNow
                };
                _db.Shipments.Add(shipment);
            }

            order.Status = status;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(long userId, long orderId)
        {
            var o = await _db.Orders
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Shipments)
                .FirstOrDefaultAsync(x => x.Id == orderId && x.UserId == userId);

            if (o == null) return null;

            var s = o.Shipments.OrderBy(s => s.Id).FirstOrDefault();
            return new OrderDetailDto
            {
                Id = o.Id,
                Status = o.Status,
                ProductId = o.ProductId,
                ProductName = o.Product.Name,
                Qty = o.Qty,
                UnitPrice = o.UnitPrice,
                Subtotal = o.Subtotal,
                ShippingCost = o.ShippingCost,
                GrandTotal = o.GrandTotal,
                CreatedAt = o.CreatedAt,
                Shipment = s == null ? null : new ShipmentDto
                {
                    Id = s.Id,
                    Status = s.Status,
                    Courier = s.Courier,
                    ShippedAt = s.ShippedAt,
                    DeliveredAt = s.DeliveredAt
                }
            };
        }

        public async Task<bool> UpdateShipmentStatusAsync(long userId, long orderId, string status)
        {
            var allowed = new[] { "in_transit", "delivered", "failed" };
            if (!allowed.Contains(status)) throw new ArgumentException("Invalid shipment status");

            var order = await _db.Orders
                .Include(o => o.Shipments)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            if (order == null) return false;

            var shipment = order.Shipments.OrderBy(s => s.Id).FirstOrDefault();
            if (shipment == null) return false;

            shipment.Status = status;
            if (status == "delivered")
            {
                shipment.DeliveredAt = DateTimeOffset.UtcNow;
                order.Status = "completed";
            }
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
