using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Services.Interfaces;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetUserCartAsync(ClaimsPrincipal user, string? promoCode);
    Task AddToCartAsync(ClaimsPrincipal user, int productId, int count);
    Task IncrementAsync(ClaimsPrincipal user, int productId);
    Task DecrementAsync(ClaimsPrincipal user, int productId);
    Task RemoveAsync(ClaimsPrincipal user, int productId);
    Task<string> CreateStripeSessionAsync(ClaimsPrincipal user, HttpRequest request);
    Task PayedSuccess(string sessionId, string userId);
}