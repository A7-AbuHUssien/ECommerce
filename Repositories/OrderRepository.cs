using ECommerce.DataAccess;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories;

public class OrderRepository : Repository<Order>,IOrderRepository
{
    private ApplicationDbContext _context;
    public OrderRepository(ApplicationDbContext context):base(context)
    {
        _context = context;
    }
    public async Task<Order?> GetOrderDetails(Guid? orderGuid)
    {
        if (orderGuid == null) return null;

        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.Product) 
            .FirstOrDefaultAsync(o => o.OrderGuid == orderGuid);
    }
}