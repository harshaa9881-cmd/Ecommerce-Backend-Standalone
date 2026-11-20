using Ecommerce.DTOs.Order;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                throw new InvalidOperationException(
                    "Invalid user information.");
            }

            return id;
        }

        // Customer: Place an order
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder()
        {
            try
            {
                var order = await _orderService.PlaceOrderAsync(
                    GetUserId());

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = order.OrderId },
                    order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Customer: View own orders
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync(
                GetUserId());

            return Ok(orders);
        }

       

        // Admin: View all orders
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}", Name = nameof(GetOrderById))]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            // Customers can only view their own orders.
            if (User.IsInRole("Customer") &&
                order.UserId != GetUserId())
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(order);
        }

        // Admin: Update order status
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(
                    id,
                    dto.OrderStatus);

                if (!updated)
                {
                    return NotFound(new { message = "Order not found." });
                }

                return Ok(new
                {
                    message = "Order status updated successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}