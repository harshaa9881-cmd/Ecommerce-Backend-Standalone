using System.Security.Claims;
using Ecommerce.DTOs.Cart;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        public async Task<ActionResult<CartResponseDto>> AddItem(
            [FromBody] AddCartItemDto dto)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var cart = await _cartService.AddItemAsync(userId, dto);

                return Ok(cart);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        // PUT: api/cart/items/{id}
        [HttpPut("items/{id:int}")]
        public async Task<ActionResult<CartResponseDto>> UpdateItem(
            int id,
            [FromBody] UpdateCartItemDto dto)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var cart = await _cartService.UpdateItemAsync(
                    userId,
                    id,
                    dto);

                if (cart == null)
                {
                    return NotFound(new
                    {
                        message = "Cart item not found."
                    });
                }

                return Ok(cart);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }
        }

        // DELETE: api/cart/items/{id}
        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            if (!TryGetUserId(out int userId))
            {
                return Unauthorized();
            }

            bool removed = await _cartService.RemoveItemAsync(userId, id);

            if (!removed)
            {
                return NotFound(new
                {
                    message = "Cart item not found."
                });
            }

            return NoContent();
        }

        // Get logged-in user's ID from JWT
        private bool TryGetUserId(out int userId)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }
    }
}