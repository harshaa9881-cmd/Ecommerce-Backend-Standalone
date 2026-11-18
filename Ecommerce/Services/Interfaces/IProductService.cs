using Ecommerce.DTOs.Product;

namespace Ecommerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductListResponseDto> GetAllAsync(
            string? searchTerm,
            int? categoryId,
            int pageNumber,
            int pageSize);

        Task<ProductResponseDto?> GetByIdAsync(int id);

        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);

        Task<ProductResponseDto?> UpdateAsync(
            int id,
            UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);
    }
}