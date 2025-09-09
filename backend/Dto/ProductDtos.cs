namespace backend.Dto
{
    public class ProductListItemDto
    {
        public long Id { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
    }

    public class BuyProductRequestDto
    {
        public long ProductId { get; set; }
        public int Qty { get; set; } = 1;
        public decimal ShippingCost { get; set; } = 0;
        public string? Courier { get; set; }
    }

    public class BuyProductResultDto
    {
        public long OrderId { get; set; }
        public string OrderStatus { get; set; } = null!;
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GrandTotal { get; set; }
        public ShipmentBriefDto Shipment { get; set; } = null!;
    }
}
