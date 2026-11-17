
namespace Ecommerce.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public string PaymentMethod { get; set; } = "CashOnDelivery";

        // Navigation properties
        public ApplicationUser? User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
