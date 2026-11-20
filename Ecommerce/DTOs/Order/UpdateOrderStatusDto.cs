using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        [MaxLength(30)]
        public string OrderStatus { get; set; } = string.Empty;
    }
}