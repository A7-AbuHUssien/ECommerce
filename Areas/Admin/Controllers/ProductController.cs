using Microsoft.AspNetCore.Mvc;

namespace ECommerce518.Areas.Admin.Controllers;

public class ProductController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}