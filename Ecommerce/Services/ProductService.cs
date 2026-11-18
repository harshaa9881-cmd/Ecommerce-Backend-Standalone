using Ecommerce.DTOs.Product;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductListResponseDto> GetAllAsync(
            string? searchTerm,
            int? categoryId,
            int pageNumber,
            int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 50);

            searchTerm = searchTerm?.Trim();

            var result = await _productRepository.GetAllAsync(
                searchTerm,
                categoryId,
                pageNumber,
                pageSize);

            var products = result.Products
                .Select(p => MapToResponseDto(p))
                .ToList();

            return new ProductListResponseDto
            {
                Products = products,
                TotalCount = result.TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize)
            };
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return MapToResponseDto(product);
        }

        public async Task<ProductResponseDto> CreateAsync(
            CreateProductDto dto)
        {
            var categoryExists =
                await _productRepository.CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "The selected category does not exist or is inactive.");
            }

            var product = new Product
            {
                ProductName = dto.ProductName.Trim(),
                Description = dto.Description?.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = dto.ImageUrl?.Trim(),
                CategoryId = dto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var createdProduct =
                await _productRepository.GetByIdAsync(product.ProductId);

            return MapToResponseDto(createdProduct!);
        }

        public async Task<ProductResponseDto?> UpdateAsync(
            int id,
            UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            var categoryExists =
                await _productRepository.CategoryExistsAsync(dto.CategoryId);

            if (!categoryExists)
            {
                throw new InvalidOperationException(
                    "The selected category does not exist or is inactive.");
            }

            product.ProductName = dto.ProductName.Trim();
            product.Description = dto.Description?.Trim();
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.ImageUrl = dto.ImageUrl?.Trim();
            product.CategoryId = dto.CategoryId;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToResponseDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            // Deactivate instead of permanently deleting.
            product.IsActive = false;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        private static ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.CategoryName ?? string.Empty
            };
        }
    }
}