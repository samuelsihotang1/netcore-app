using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class OrderItem
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
        public long ProductId { get; set; }

        public int Qty { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal LineTotal { get; set; } // Qty * UnitPrice

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
