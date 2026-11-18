using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<(List<Product> Products, int TotalCount)> GetAllAsync(
            string? searchTerm,
            int? categoryId,
            int pageNumber,
            int pageSize);

        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        void Update(Product product);

        Task<bool> CategoryExistsAsync(int categoryId);

        Task SaveChangesAsync();
    }
}