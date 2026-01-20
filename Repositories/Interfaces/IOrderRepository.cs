namespace ECommerce.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderDetails(Guid? orderGuid);
}