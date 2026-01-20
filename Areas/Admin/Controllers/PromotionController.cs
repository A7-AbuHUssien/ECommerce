using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize (Roles = $"{StaticData.ADMIN_ROLE},{StaticData.SUPER_ADMIN_ROLE}")]
public class PromotionController : Controller
{
    private readonly IPromotionService _service;
    

    public PromotionController(IPromotionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var promotions = await _service.GetAllAsync();
        return View(promotions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var products = await _service.GetProductsAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Promotion promo)
    {
        if (ModelState.IsValid)
        {
            await _service.CreateAsync(promo);
            TempData["success"] = "Promotion created successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        var products = await _service.GetProductsAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name");

        return View(promo);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var promo = await _service.GetByIdAsync(id);
        if (promo == null) return NotFound();

        var products = await _service.GetProductsAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name");

        return View(promo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Promotion promo)
    {
        if (ModelState.IsValid)
        {
            await _service.UpdateAsync(promo);
            TempData["success"] = "Promotion updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(promo);
    }

    [HttpPost]
    public async Task<IActionResult> ActivateDeactivate(int id)
    {
        var res = await _service.ToggleActivationAsync(id);
        if(res)
            TempData["success"] = "Promotion activated successfully!";
        else
            TempData["error"] = "Promotion activated failed!";
        return RedirectToAction(nameof(Index));
    }
}
