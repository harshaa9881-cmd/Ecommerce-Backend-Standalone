using Ecommerce.DTOs.Category;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return categories.Select(category => new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            }).ToList();
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName.Trim(),
                Description = dto.Description?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);

            await _categoryRepository.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryResponseDto?> UpdateAsync(
            int id,
            UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            category.CategoryName = dto.CategoryName.Trim();
            category.Description = dto.Description?.Trim();

            _categoryRepository.Update(category);

            await _categoryRepository.SaveChangesAsync();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return false;
            }

            bool hasProducts =
                await _categoryRepository.HasProductsAsync(id);

            if (hasProducts)
            {
                throw new InvalidOperationException(
                    "Cannot delete this category because products are associated with it.");
            }

            _categoryRepository.Delete(category);

            await _categoryRepository.SaveChangesAsync();

            return true;
        }
    }
}