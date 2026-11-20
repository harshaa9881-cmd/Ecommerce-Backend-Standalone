using Ecommerce.DTOs.Cart;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

namespace Ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        // Get the logged-in customer's cart
        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync();
            }

            return MapToResponseDto(cart);
        }

        // Add a product to the cart
        public async Task<CartResponseDto> AddItemAsync(
            int userId,
            AddCartItemDto dto)
        {
            var product = await _productRepository.GetByIdAsync(
                dto.ProductId);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Product not found or inactive.");
            }

            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _cartRepository.AddAsync(cart);
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == dto.ProductId);

            int requestedQuantity = dto.Quantity;

            // If the product already exists, increase its quantity
            if (existingItem != null)
            {
                requestedQuantity += existingItem.Quantity;
            }

            // Check available stock
            if (requestedQuantity > product.StockQuantity)
            {
                throw new InvalidOperationException(
                    $"Only {product.StockQuantity} units are available.");
            }

            if (existingItem != null)
            {
                existingItem.Quantity = requestedQuantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = dto.Quantity,
                    Cart = cart
                };

                cart.CartItems.Add(cartItem);
            }

            await _cartRepository.SaveChangesAsync();

            // Reload cart to get updated product information
            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);

            return MapToResponseDto(updatedCart!);
        }

        // Update an existing cart item's quantity
        public async Task<CartResponseDto?> UpdateItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                return null;
            }

            if (cartItem.Product == null || !cartItem.Product.IsActive)
            {
                throw new InvalidOperationException(
                    "This product is no longer available.");
            }

            if (dto.Quantity > cartItem.Product.StockQuantity)
            {
                throw new InvalidOperationException(
                    $"Only {cartItem.Product.StockQuantity} units are available.");
            }

            cartItem.Quantity = dto.Quantity;

            await _cartRepository.SaveChangesAsync();

            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);

            return MapToResponseDto(updatedCart!);
        }

        // Remove an item from the customer's cart
        public async Task<bool> RemoveItemAsync(
            int userId,
            int cartItemId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return false;
            }

            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
            {
                return false;
            }

            _cartRepository.RemoveItem(cartItem);

            await _cartRepository.SaveChangesAsync();

            return true;
        }

        // Convert database Cart model into response DTO
        private static CartResponseDto MapToResponseDto(Cart cart)
        {
            var items = cart.CartItems
                .Where(ci => ci.Product != null)
                .Select(ci => new CartItemResponseDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product!.ProductName,
                    ImageUrl = ci.Product.ImageUrl,
                    UnitPrice = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity
                })
                .ToList();

            return new CartResponseDto
            {
                CartId = cart.CartId,
                Items = items,
                TotalAmount = items.Sum(item => item.Subtotal)
            };
        }
    }
}