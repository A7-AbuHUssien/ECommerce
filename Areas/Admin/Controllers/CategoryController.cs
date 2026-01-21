using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE},{StaticData.ADMIN_ROLE},{StaticData.EMPLOYEE_ROLE}")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await _categoryService.GetAsync(tracked:false, cancellationToken: cancellationToken));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        await _categoryService.Create(category,cancellationToken);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id,CancellationToken cancellationToken)
    {
        Category? category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken: cancellationToken);
        if (category is null)
            return RedirectToAction("NotFoundPage","Home");
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Category category,CancellationToken cancellationToken)
    {
       await _categoryService.Edit(category,cancellationToken);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
    {
        Category? category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken: cancellationToken);
        if (category == null)
        {
            return RedirectToAction("NotFoundPage","Home");
        }

        if (await _categoryService.Delete(category.Id,cancellationToken))
            return Json(new { success = true });
        return Json(new { success = false });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id,CancellationToken cancellationToken)
    {
        Category? category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken: cancellationToken);
        if (category is null)
            return RedirectToAction("NotFoundPage","Home");
        return View(category);
    }
}