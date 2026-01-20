using Microsoft.AspNetCore.Mvc;

namespace ECommerce518.Areas.Admin.Controllers;

public class CategoryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}