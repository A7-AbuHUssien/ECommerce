using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Identity.Controllers;

public class ProfileController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}