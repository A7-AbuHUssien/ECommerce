using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

public class OrderController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}