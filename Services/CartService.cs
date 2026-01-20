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
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<OrderItem> _orderItemRepository;

    public CartService(
        UserManager<ApplicationUser> userManager,
        IRepository<Cart> cartRepository,
        IRepository<Product> productRepository,
        IRepository<Promotion> promotionRepository,
        IRepository<Order> orderRepository,
        IRepository<OrderItem> orderItemRepository)
    {
        _userManager = userManager;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _promotionRepository = promotionRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
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
            SuccessUrl = $"{request.Scheme}://{request.Host}/Customer/Cart/Success?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{request.Scheme}://{request.Host}/Customer/Cart/Index",
            
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
        var session = await service.CreateAsync(options);

        return session.Url;
    }

    public async Task PayedSuccess(string sessionId, string userId)
    {
        // 1. تنظيف الـ SessionId من أي علامات زائدة (تجنباً للخطأ السابق)
        sessionId = sessionId.Trim().Replace(";", "");

        // 2. التأكد من Stripe أن العملية تمت بنجاح فعلاً
        var service = new SessionService();
        var session = await service.GetAsync(sessionId);

        if (session.PaymentStatus.ToLower() == "paid")
        {
            // 3. التحقق: هل الأوردر ده اتسجل قبل كدا؟ (مهم جداً لمنع التكرار)
            var existingOrder = await _orderRepository.GetOneAsync(o => o.StripeSessionId == sessionId);
        
            if (existingOrder is null) // إذا لم يكن موجوداً، نقوم بإنشائه
            {
                // 4. جلب بيانات السلة
                var cartItems = await _cartRepository.GetAsync(
                    expression: e => e.ApplicationUserId == userId,
                    includes: [e => e.Product]);

                if (cartItems.Any())
                {
                    // 5. إنشاء الأوردر
                    var order = new Order {
                        UserId = userId,
                        OrderDate = DateTime.UtcNow,
                        Total = (decimal)session.AmountTotal / 100,
                        StripeSessionId = sessionId,
                        PaymentStatus = "Paid",
                        OrderItems = cartItems.Select(ci => new OrderItem {
                            ProductId = ci.ProductId,
                            Quantity = ci.Count,
                            Price = ci.Price // نستخدم السعر المخزن في السلة
                        }).ToList()
                    };

                    await _orderRepository.CreateAsync(order);
                    
                    // 6. مسح السلة
                    foreach(var item in cartItems)
                    {
                        _cartRepository.Delete(item);
                    }

                    // حفظ كل التغييرات
                    await _orderRepository.CommitAsync();
                    await _cartRepository.CommitAsync();
                }
            }
        }
    }
}