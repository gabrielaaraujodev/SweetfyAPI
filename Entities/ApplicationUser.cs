using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SweetfyAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        [ForeignKey(nameof(Bakery))]
        public int? BakeryId { get; set; }
        public Bakery? Bakery { get; set; }
    }
}
