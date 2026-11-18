
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task AddAsync(ApplicationUser user);
        Task SaveChangesAsync();
    }
}
