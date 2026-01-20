using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Identity.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}