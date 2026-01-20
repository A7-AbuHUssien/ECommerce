using System.Linq.Expressions;

namespace ECommerce.Services.Interfaces;

public interface IBrandService
{
    Task Create(Brand brand, IFormFile? logo, CancellationToken cancellationToken);
    Task<Brand?> GetBrandByIdAsync(int id, CancellationToken cancellationToken);
    Task Edit(Brand brand, IFormFile? logo, CancellationToken cancellationToken);
    Task<bool> Delete(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Brand>> GetAsync
    (
        Expression<Func<Brand, bool>>? expression = null,
        Expression<Func<Brand, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
    );  
}