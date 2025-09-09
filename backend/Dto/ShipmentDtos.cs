namespace backend.Dto
{
    public class ShipmentBriefDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
    }

    public class ShipmentDto
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
        public string? Courier { get; set; }
        public DateTimeOffset? ShippedAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }
    }

    // Update status delivery: packaging|in_transit|failed
    public class ShipmentStatusUpdateDto
    {
        public string Status { get; set; } = null!;
    }
}
