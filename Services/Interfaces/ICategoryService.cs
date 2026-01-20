using System.Linq.Expressions;

namespace ECommerce.Services.Interfaces;

public interface ICategoryService
{
    Task Create(Category category, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    Task Edit(Category category, CancellationToken cancellationToken);
    Task<bool> Delete(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Category>> GetAsync
    (
        Expression<Func<Category, bool>>? expression = null,
        Expression<Func<Category, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
    );
}