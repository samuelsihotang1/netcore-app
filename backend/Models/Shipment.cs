using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Shipment
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        [MaxLength(60)]
        public string? Courier { get; set; }

        // packaging | in_transit
        [Required, MaxLength(30)]
        public string Status { get; set; } = "packaging";

        public DateTimeOffset? ShippedAt { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
