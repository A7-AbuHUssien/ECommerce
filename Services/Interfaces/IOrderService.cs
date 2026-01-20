namespace ECommerce.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetUserOrders(string? userId);
    Task<Order?> GetOrderDetails(Guid? orderGuid);
    Task<bool> DeleteOrderAsync(int? orderId);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task UpdateOrderStatusAsync(int orderId, string status);
}