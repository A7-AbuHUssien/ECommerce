using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE},{StaticData.ADMIN_ROLE},{StaticData.EMPLOYEE_ROLE}")]
public class BrandController : Controller
{
    private readonly IBrandService _brandService;

    public BrandController( IBrandService brandService)
    {
        _brandService = brandService;
    }

    // GET: Admin/Brand
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        await _brandService.GetAsync(cancellationToken: cancellationToken);
        return View(await _brandService.GetAsync(tracked:false, cancellationToken: cancellationToken));
    }

    // GET: Admin/Brand/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Brand/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand brand, IFormFile? logo, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(brand);
        }

        await _brandService.Create(brand, logo, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Brand/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id,CancellationToken cancellationToken)
    {
        Brand? brand = await _brandService.GetBrandByIdAsync(id, cancellationToken);
        if (brand == null)
            return RedirectToAction("NotFoundPage", "Home");

        return View(brand);
    }

    // POST: Admin/Brand/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Brand brand, IFormFile? logo, CancellationToken cancellationToken)
    {
        ModelState.Remove("Logo");
        if (!ModelState.IsValid)
                 return View(brand);
        await _brandService.Edit(brand, logo, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Brand/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
    {
       bool result = await _brandService.Delete(id, cancellationToken);
        if (!result)
            return RedirectToAction("NotFoundPage","Home",new { area = "Admin" });
        
        return Json(new { success = true });
    }

    // GET: Admin/Brand/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id,CancellationToken cancellationToken)
    {
        Brand? brand = await _brandService.GetBrandByIdAsync(id,cancellationToken);
        if (brand == null)
            return RedirectToAction("NotFoundPage", "Home", new { area = "Admin" });
        return View(brand);
    }
}
