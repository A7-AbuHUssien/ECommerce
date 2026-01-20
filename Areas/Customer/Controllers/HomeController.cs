using System.Diagnostics;
using ECommerce518.DataAccess;
using ECommerce518.Models;
using ECommerce518.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce518.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(FilterVM filterVm, int page = 1)
        {
            const decimal discount = 50;
            var products = _context.Products.AsQueryable();

            // Add Filters
            products = products.Include(e => e.Category);

            if(filterVm.ProductName is not null)
            {
                products = products.Where(e=>e.Name.Contains(filterVm.ProductName));
                ViewBag.ProductName = filterVm.ProductName;
            }

            if(filterVm.MinPrice > 0)
            {
                products = products.Where(e=> (e.Price - e.Price * (e.Discount / 100)) > filterVm.MinPrice);
                ViewBag.MinPrice = filterVm.MinPrice;
            }

            if (filterVm.MaxPrice > 0)
            {
                products = products.Where(e => (e.Price - e.Price * (e.Discount / 100)) < filterVm.MaxPrice);
                ViewBag.MaxPrice = filterVm.MaxPrice;
            }

            if (filterVm.CategoryId > 0)
            {
                products = products.Where(e=>e.CategoryId == filterVm.CategoryId);
                ViewBag.CategoryId = filterVm.CategoryId;
            }

            if (filterVm.IsHot)
            {
                products = products.Where(e => e.Discount > discount);
                ViewBag.IsHot = filterVm.IsHot;
            }

            // List Of categories
            var categories = _context.Categories.AsQueryable();
            //ViewBag.Categories = categories.ToList();
            ViewData["Categories"] = categories.ToList();

            // Add Pagination
            var totalPages = Math.Ceiling(products.Count() / 8.0);
            products = products.Skip((page - 1) * 8).Take(8);
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;

            return View(products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Welcome()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
