
namespace Ecommerce.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ApplicationUser? User { get; set; }

        // One cart can contain many cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
