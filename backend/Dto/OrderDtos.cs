namespace backend.Dto
{
    public class OrderSummaryDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? ShipmentStatus { get; set; }
        public long? ShipmentId { get; set; }
    }

    public class OrderStatusUpdateDto
    {
        // draft|paid|completed|cancelled
        public string Status { get; set; } = null!;
    }

    public class OrderDetailDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ShipmentDto? Shipment { get; set; }
    }
}
