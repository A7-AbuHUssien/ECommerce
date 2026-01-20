using System.Diagnostics;
using ECommerce.Services.Interfaces;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;
    public HomeController(IProductService productService,
        ICategoryService categoryService, IOrderService orderService,
        UserManager<ApplicationUser> userManager)
    {
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(AdminProductFilterVM filter, int page = 1, int pageSize = 8)
    {
        var productsQuery = _productService.GetQueryable(false);

        productsQuery = _productService.ApplyFilters(productsQuery, filter);

        ViewData["Categories"] = await _categoryService.GetAsync();

        var (pagedProducts, totalItems) =
            await _productService.PaginateAsync(productsQuery, page, pageSize);

        ViewBag.totalPages = Math.Ceiling(totalItems / (double)pageSize);
        ViewBag.currentPage = page;

        return View(pagedProducts);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    public ViewResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Item(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product is null) return NotFound();

        var relatedProduct = await _productService.GetRelatedProducts(id);
        relatedProduct = relatedProduct.Skip(0).Take(4);

        ViewBag.relatedProduct = relatedProduct;

        return View(product);
    }

    public async Task<IActionResult> ShowCustomerOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        var userOrders= await _orderService.GetUserOrders(user.Id);
        return View(userOrders);
    }

    public async Task<IActionResult> ShowCustomerOrderDetails(Guid? orderGuid)
    {
        Order? orderDetails = await _orderService.GetOrderDetails(orderGuid);
        if (orderDetails is null) return NotFound();
        return View(orderDetails);
    }
}