using System.Security.Claims;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    public CartController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(string? code = null)
    {
        var cart = await _cartService.GetUserCartAsync(User, code);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int count)
    {
        await _cartService.AddToCartAsync(User, productId, count);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> IncrementCount(int productId)
    {
        await _cartService.IncrementAsync(User, productId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DecrementCount(int productId)
    {
        await _cartService.DecrementAsync(User, productId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteProduct(int productId)
    {
        await _cartService.RemoveAsync(User, productId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Pay()
    {
        var url = await _cartService.CreateStripeSessionAsync(User, Request);
        return Redirect(url);
    }
    // -----------------------
    public async Task<IActionResult> Success(string sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _cartService.PayedSuccess(sessionId, userId);
        return RedirectToAction(nameof(Index));    
    }
}