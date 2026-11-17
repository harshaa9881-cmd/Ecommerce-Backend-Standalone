using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        Task AddAsync(Category category);

        void Update(Category category);

        void Delete(Category category);

        Task<bool> HasProductsAsync(int categoryId);

        Task SaveChangesAsync();
    }
}