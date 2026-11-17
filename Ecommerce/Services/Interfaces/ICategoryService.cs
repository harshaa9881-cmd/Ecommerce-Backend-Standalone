using Ecommerce.DTOs.Category;

namespace Ecommerce.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();

        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);

        Task<CategoryResponseDto?> UpdateAsync(int id, UpdateCategoryDto dto);

        Task<bool> DeleteAsync(int id);
    }
}