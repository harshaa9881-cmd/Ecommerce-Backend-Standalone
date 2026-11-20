using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task AddOrderAsync(Order order);
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<List<Order>> GetAllAsync();
        void ClearCart(Cart cart);
        Task SaveChangesAsync();
    }
}