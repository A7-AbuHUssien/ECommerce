using ECommerce.Repositories.Interfaces;
using ECommerce.Services.AdminServices.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Services.AdminServices;

public class PromotionService : IPromotionService
{
    private readonly IRepository<Promotion> _promotionRepo;
    private readonly IRepository<Product> _productRepo;

    public PromotionService(IRepository<Promotion> promotionRepo, IRepository<Product> productRepo)
    {
        _promotionRepo = promotionRepo;
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        var promotions = await _promotionRepo.GetAsync(includes: [p => p.Product]);
        return promotions;
    }

    public async Task<Promotion?> GetByIdAsync(int id)
    {
        var promo = await _promotionRepo.GetOneAsync(e => e.Id == id);
        if (promo == null) return null;
        return promo;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _productRepo.GetAsync();
    }


    public async Task CreateAsync(Promotion promotion)
    {
        await _promotionRepo.CreateAsync(promotion);
        await _promotionRepo.CommitAsync();
    }

    public async Task UpdateAsync(Promotion promotion)
    {
        _promotionRepo.Update(promotion);
        await _promotionRepo.CommitAsync();
    }

    public async Task<bool> ToggleActivationAsync(int promotionId)
    {
        var promo = await _promotionRepo.GetOneAsync(e => e.Id == promotionId);
        if (promo == null) return false;
        promo.IsValid = !promo.IsValid;
        _promotionRepo.Update(promo);
        await _promotionRepo.CommitAsync();
        return true;
    }
}