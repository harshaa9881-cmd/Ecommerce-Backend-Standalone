
namespace Ecommerce.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public int CategoryId { get; set; }

        // Navigation property
        public Category? Category { get; set; }

        // One product can appear in many cart items
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // One product can appear in many order items
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
