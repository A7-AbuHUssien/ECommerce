using System.Linq.Expressions;

namespace ECommerce.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAsync(bool tracked = true, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors, CancellationToken cancellationToken = default);
    Task EditAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

}