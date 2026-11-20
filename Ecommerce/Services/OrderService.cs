using Ecommerce.Data;
using Ecommerce.DTOs.Order;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderService(
            IOrderRepository orderRepository,
            ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _orderRepository.GetCartByUserIdAsync(userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    throw new InvalidOperationException(
                        "Your cart is empty.");
                }

                // Check all products and stock before changing anything.
                foreach (var item in cart.CartItems)
                {
                    if (item.Product == null || !item.Product.IsActive)
                    {
                        throw new InvalidOperationException(
                            "One or more products are unavailable.");
                    }

                    if (item.Quantity <= 0 ||
                        item.Product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for {item.Product.ProductName}.");
                    }
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Pending",
                    PaymentMethod = "CashOnDelivery"
                };

                foreach (var item in cart.CartItems)
                {
                    var product = item.Product!;

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity
                    });

                    order.TotalAmount += product.Price * item.Quantity;

                    // Reduce stock only during successful order placement.
                    product.StockQuantity -= item.Quantity;
                }

                await _orderRepository.AddOrderAsync(order);

                _orderRepository.ClearCart(cart);

                await _orderRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapToDto(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<OrderResponseDto>> GetMyOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            return order == null ? null : MapToDto(order);
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(MapToDto).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(
            int orderId,
            string status)
        {
            var allowedStatuses = new[]
            {
                "Pending",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            status = status.Trim();

            if (!allowedStatuses.Contains(
                status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid order status.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            order.OrderStatus = allowedStatuses.First(
                s => s.Equals(
                    status,
                    StringComparison.OrdinalIgnoreCase));

            await _orderRepository.SaveChangesAsync();

            return true;
        }

        private static OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,

                Items = order.OrderItems.Select(item =>
                    new OrderItemResponseDto
                    {
                        OrderItemId = item.OrderItemId,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        Subtotal = item.UnitPrice * item.Quantity
                    }).ToList()
            };
        }
    }
}