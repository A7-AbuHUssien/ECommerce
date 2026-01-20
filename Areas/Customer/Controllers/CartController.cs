using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Customer.Controllers;

public class CartController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}