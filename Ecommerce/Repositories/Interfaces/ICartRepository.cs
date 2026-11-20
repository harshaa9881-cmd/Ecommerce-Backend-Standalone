using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);

        Task AddAsync(Cart cart);

        void RemoveItem(CartItem cartItem);

        Task SaveChangesAsync();
    }
}