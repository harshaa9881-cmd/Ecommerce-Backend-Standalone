using Ecommerce.DTOs.Order;

namespace Ecommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(int userId);
        Task<List<OrderResponseDto>> GetMyOrdersAsync(int userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(int orderId);
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}