using ECommerce.Services.Interfaces;
using ECommerce.Utilities;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{StaticData.SUPER_ADMIN_ROLE},{StaticData.ADMIN_ROLE},{StaticData.EMPLOYEE_ROLE}")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;

    public ProductController(
        IProductService productService,
        ICategoryService categoryService,
        IBrandService brandService
    )
    {
        _productService = productService;
        _categoryService = categoryService;
        _brandService = brandService;
    }

    // GET: Admin/Product
    [HttpGet]
    public async Task<IActionResult> Index(AdminProductFilterVM? filter, int page = 1, int pageSize = 10)
    {
        var products = _productService.GetQueryable(false);
        products = _productService.ApplyFilters(products, filter);
        var (pagedProducts, totalItems) = await _productService.PaginateAsync(products, page, pageSize);
        
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.Brands = await _brandService.GetAsync(tracked: false);
        ViewBag.Categories = await _categoryService.GetAsync(tracked: false);

        return View(pagedProducts);
    }

    // GET: Admin/Product/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
            return RedirectToAction("NotFoundPage", "Home");

        return View(product);
    }

    // GET: Admin/Product/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new ProductVM
        {
            Product = new Product(),
            Categories = await _categoryService.GetAsync(),
            Brands = await _brandService.GetAsync()
        };
        return View(vm);
    }

    // POST: Admin/Product/Create
    [HttpPost]
    public async Task<IActionResult> Create(ProductVM productVm, IFormFile? file, List<IFormFile>? subfiles, string[]? colors)
    {
        ModelState.Remove("MainImg");
        if (!ModelState.IsValid)
        {
            var vm = new ProductVM
            {
                Product = productVm.Product,
                Categories = await _categoryService.GetAsync(),
                Brands = await _brandService.GetAsync()
            };
            return View(vm);
        }

        if (productVm.Product != null) await _productService.CreateAsync(productVm.Product, file, subfiles, colors);

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Product/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return RedirectToAction("NotFoundPage", "Home");

        var vm = new ProductVM
        {
            Product = product,
            Categories = await _categoryService.GetAsync(),
            Brands = await _brandService.GetAsync()
        };

        return View(vm);
    }

    // POST: Admin/Product/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(ProductVM productVm, IFormFile? file, List<IFormFile>? subfiles, string[]? colors)
    {
        ModelState.Remove("MainImg");
        if (!ModelState.IsValid)
        {
            var vm = new ProductVM
            {
                Product = productVm.Product,
                Categories = await _categoryService.GetAsync(),
                Brands = await _brandService.GetAsync()
            };
            return View(vm);
        }

        if (productVm.Product != null) await _productService.EditAsync(productVm.Product, file, subfiles, colors);

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Product/Delete/5
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _productService.DeleteAsync(id);
        return Json(new { success = deleted });
    }
}
