using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required, MaxLength(80)]
        public string Sku { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(14,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; } = 0;

        public int? WeightGram { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
