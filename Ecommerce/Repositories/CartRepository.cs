using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public void RemoveItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}