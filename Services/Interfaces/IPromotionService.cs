using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Services.AdminServices.Interfaces;

public interface IPromotionService
{
    Task<IEnumerable<Promotion>> GetAllAsync();
    Task<Promotion?> GetByIdAsync(int id);

    Task<IEnumerable<Product>> GetProductsAsync();
    Task CreateAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);

    Task<bool> ToggleActivationAsync(int promotionId);
}