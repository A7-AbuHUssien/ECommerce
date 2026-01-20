using ECommerce.Models;
using ECommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
public class PromotionsController : Controller
{
    private readonly IRepository<Promotion> _promotionRepo;
    private readonly IRepository<Product> _productRepo; // نحتاجه لعرض قائمة المنتجات في الـ Dropdown

    public PromotionsController(IRepository<Promotion> promotionRepo, IRepository<Product> productRepo)
    {
        _promotionRepo = promotionRepo;
        _productRepo = productRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var promotions = await _promotionRepo.GetAsync(includes:[p => p.Product]);
        return View(promotions);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var products = await _productRepo.GetAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Promotion promo)
    {
        if (ModelState.IsValid)
        {
            await _promotionRepo.CreateAsync(promo);
            await _promotionRepo.CommitAsync();
            TempData["success"] = "Promotion created successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        var products = await _productRepo.GetAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name");
        return View(promo);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var promo = await _promotionRepo.GetOneAsync(e => e.Id == id);
        if (promo == null) return NotFound();

        var products = await _productRepo.GetAsync();
        ViewBag.ProductList = new SelectList(products, "Id", "Name", promo.ProductId);
        return View(promo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Promotion promo)
    {
        if (ModelState.IsValid)
        {
            _promotionRepo.Update(promo);
            await _promotionRepo.CommitAsync();
            TempData["success"] = "Promotion updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(promo);
    }

    [HttpPost]
    public async Task<IActionResult> ActivateDeactivate(int id)
    {
        var promo = await _promotionRepo.GetOneAsync(e => e.Id == id);
        if (promo == null) return NotFound();

        // تبديل الحالة (Toggle)
        promo.IsValid = !promo.IsValid;
        
        _promotionRepo.Update(promo);
        await _promotionRepo.CommitAsync();
        
        return Json(new { success = true, status = promo.IsValid });
    }
}