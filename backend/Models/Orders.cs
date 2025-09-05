using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Order
    {
        public long Id { get; set; }

        public long? UserId { get; set; }

        [Required]
        public long ProductId { get; set; }

        // draft | paid | completed | cancelled
        [Required, MaxLength(30)]
        public string Status { get; set; } = "draft";

        public int Qty { get; set; } = 1;

        [Column(TypeName = "decimal(14,2)")]
        public decimal UnitPrice { get; set; }

        // computed in DB: Qty * UnitPrice
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(14,2)")]
        public decimal Subtotal { get; private set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal ShippingCost { get; set; }

        // computed in DB: Subtotal + ShippingCost
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(14,2)")]
        public decimal GrandTotal { get; private set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Product Product { get; set; } = null!;
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}