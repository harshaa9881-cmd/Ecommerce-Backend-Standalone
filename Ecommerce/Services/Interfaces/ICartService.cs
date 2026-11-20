using Ecommerce.DTOs.Cart;

namespace Ecommerce.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);

        Task<CartResponseDto> AddItemAsync(
            int userId,
            AddCartItemDto dto);

        Task<CartResponseDto?> UpdateItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto);

        Task<bool> RemoveItemAsync(
            int userId,
            int cartItemId);
    }
}