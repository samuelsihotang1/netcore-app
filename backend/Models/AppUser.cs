using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class AppUser : IdentityUser<long>
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
