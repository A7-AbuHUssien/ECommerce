using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;
    public OrderService(IOrderRepository orderRepository, IRepository<OrderItem> orderItemRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<IEnumerable<Order>> GetUserOrders(string? userId)
    {
        IEnumerable<Order> orders = await _orderRepository.GetAsync(expression: e => e.UserId == userId,
            includes: [o => o.OrderItems], tracked: false);
        return orders;
    }

    public async Task<Order?> GetOrderDetails(Guid? orderGuid)
    {
        var order = await _orderRepository.GetOrderDetails(orderGuid);
        if (order == null) return null;
        return order;
    }
    
    public async Task<bool> DeleteOrderAsync(int? orderId)
    {
        var order = await _orderRepository.GetOneAsync(e => e.Id == orderId);
        if (order == null) return false;
        _orderRepository.Delete(order);
        await _orderRepository.CommitAsync();
        return true;
    }
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAsync(includes: [o => o.User]);
    }
    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepository.GetOneAsync(o => o.Id == orderId);
    
        if (order != null)
        {
            order.OrderStatus = status;
        
            // If the admin cancels the order, you might want to handle payment logic here too
            if (status == "Cancelled")
            {
                order.PaymentStatus = "Refunded/Cancelled";
            }

            await _orderRepository.CommitAsync();
        }
    }
}