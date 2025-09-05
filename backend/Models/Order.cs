using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Order
    {
        public long Id { get; set; }

        public long? UserId { get; set; }

        [Required, MaxLength(30)] // draft|paid|completed|cancelled
        public string Status { get; set; } = "draft";

        [Column(TypeName = "decimal(14,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(14,2)")]
        public decimal GrandTotal { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Navigation
        public User? User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
