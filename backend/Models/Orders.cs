using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Order
    {
        public long Id { get; set; }

        public long? UserId { get; set; }
        public AppUser? User { get; set; }

        [Required]
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // draft | paid | completed | cancelled
        [Required, MaxLength(30)]
        public string Status { get; set; } = "draft";

        public int Qty { get; set; } = 1;

        [Column(TypeName = "decimal(14,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal Subtotal { get; set; }      // Qty * UnitPrice

        [Column(TypeName = "decimal(14,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal GrandTotal { get; set; }    // Subtotal + ShippingCost

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
