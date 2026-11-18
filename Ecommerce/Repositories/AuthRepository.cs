
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(ApplicationUser user)
        {
            await _context.ApplicationUsers.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
