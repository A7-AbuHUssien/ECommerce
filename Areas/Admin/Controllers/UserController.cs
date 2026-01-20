using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}