using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(category => category.CategoryId == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _context.Products
                .AnyAsync(product => product.CategoryId == categoryId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}