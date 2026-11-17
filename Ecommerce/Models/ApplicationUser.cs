
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class ApplicationUser
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // One user has one cart
        public Cart? Cart { get; set; }

        // One user can have many orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
