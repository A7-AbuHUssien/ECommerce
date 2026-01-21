using System.Linq.Expressions;
using ECommerce.ViewModels;

namespace ECommerce.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAsync(bool tracked = true, CancellationToken cancellationToken = default);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Product> CreateAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors,
        CancellationToken cancellationToken = default);

    Task EditAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    IQueryable<Product> ApplyFilters(IQueryable<Product> products, ProductFilterVM? filter);
    IQueryable<Product> GetQueryable(bool tracked = true);

    Task<(IEnumerable<Product> Products, int TotalItems)>
        PaginateAsync(
            IQueryable<Product> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> GetRelatedProducts(int productId);
}