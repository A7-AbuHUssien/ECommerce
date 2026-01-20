using Microsoft.AspNetCore.Mvc;
using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE},{StaticData.ADMIN_ROLE},{StaticData.EMPLOYEE_ROLE}")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index()
    {
        // جلب جميع الطلبات من قاعدة البيانات
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
    {
        await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(Guid guid) 
    {
        if (guid == Guid.Empty) return NotFound();

        // جلب الطلب مع بيانات المستخدم والمنتجات
        var order = await _orderService.GetOrderDetails(guid);
    
        if (order == null)
        {
            return NotFound();
        }

        // تمرير بيانات المستخدم عبر ViewBag لسهولة الوصول إليها في الـ View
        ViewBag.CustomerName = order.User?.UserName;
        ViewBag.CustomerEmail = order.User?.Email;
        ViewBag.CustomerPhone = order.User?.PhoneNumber; // إذا كنت تجمعه عند التسجيل
        ViewBag.OrderDateFormatted = order.OrderDate.ToString("MMMM dd, yyyy");

        return View(order);
    }
}