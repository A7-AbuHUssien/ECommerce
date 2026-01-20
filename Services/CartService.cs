using System.Security.Claims;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;

namespace ECommerce.Services;

public class CartService : ICartService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Cart> _cartRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Promotion> _promotionRepository;

    public CartService(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Product> productRepository,
        IRepository<Promotion> promotionRepository)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _promotionRepository = promotionRepository;
    }

    private async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
    {
        return await _userManager.GetUserAsync(user)
               ?? throw new InvalidOperationException("User not found");
    }

    public async Task<IEnumerable<Cart>> GetUserCartAsync(ClaimsPrincipal user, string? promoCode)
    {
        var appUser = await GetUserAsync(user);

        var cart = await _cartRepository.GetAsync(
            e => e.ApplicationUserId == appUser.Id,
            includes: [e => e.Product]
        );

        if (string.IsNullOrWhiteSpace(promoCode))
            return cart;

        var promotion = await _promotionRepository.GetOneAsync(e => e.Code == promoCode);
        if (promotion is null)
            return cart;

        var item = cart.FirstOrDefault(e => e.ProductId == promotion.ProductId);
        if (item is null)
            return cart;

        if (!promotion.IsValid || promotion.ValidTo <= DateTime.UtcNow || promotion.MaxUsage <= 0)
            return cart;

        item.Price -= item.Price * (promotion.Discount / 100);
        promotion.MaxUsage--;

        await _cartRepository.CommitAsync();
        return cart;
    }

    public async Task AddToCartAsync(ClaimsPrincipal user, int productId, int count)
    {
        if (count <= 0) count = 1;

        var appUser = await GetUserAsync(user);

        var product = await _productRepository.GetOneAsync(e => e.Id == productId)
                      ?? throw new InvalidOperationException("Product not found");

        var cartItem =
            await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == appUser.Id);

        if (cartItem is not null)
        {
            cartItem.Count += count;
        }
        else
        {
            await _cartRepository.CreateAsync(new Cart
            {
                ApplicationUserId = appUser.Id,
                ProductId = productId,
                Count = count,
                Price = product.Price - product.Price * (product.Discount / 100)
            });
        }

        await _cartRepository.CommitAsync();
    }

    public async Task IncrementAsync(ClaimsPrincipal user, int productId)
    {
        var appUser = await GetUserAsync(user);

        var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == appUser.Id)
                   ?? throw new InvalidOperationException("Cart item not found");

        cart.Count++;
        await _cartRepository.CommitAsync();
    }

    public async Task DecrementAsync(ClaimsPrincipal user, int productId)
    {
        var appUser = await GetUserAsync(user);

        var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == appUser.Id)
                   ?? throw new InvalidOperationException("Cart item not found");

        if (cart.Count > 1)
            cart.Count--;

        await _cartRepository.CommitAsync();
    }

    public async Task RemoveAsync(ClaimsPrincipal user, int productId)
    {
        var appUser = await GetUserAsync(user);

        var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == appUser.Id)
                   ?? throw new InvalidOperationException("Cart item not found");

        _cartRepository.Delete(cart);
        await _cartRepository.CommitAsync();
    }

    public async Task<string> CreateStripeSessionAsync(ClaimsPrincipal user, HttpRequest request)
    {
        var appUser = await GetUserAsync(user);

        var cart = await _cartRepository.GetAsync(
            e => e.ApplicationUserId == appUser.Id,
            includes: [e => e.Product]
        );

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = $"{request.Scheme}://{request.Host}/identity/checkout/success",
            CancelUrl = $"{request.Scheme}://{request.Host}/identity/checkout/cancel",
        };

        foreach (var item in cart)
        {
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "EGP",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                        Description = item.Product.Description,
                    },
                    UnitAmount = (long)item.Price * 100,
                },
                Quantity = item.Count,
            });
        }

        var service = new SessionService();
        var session = service.Create(options);

        return session.Url;
    }
}