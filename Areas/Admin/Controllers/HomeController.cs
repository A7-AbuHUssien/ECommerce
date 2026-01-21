using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE},{StaticData.ADMIN_ROLE},{StaticData.EMPLOYEE_ROLE}")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult NotFoundPage()
    {
        return View();
    }
}