using System.Linq.Expressions;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _repository;

    public CategoryService(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public async Task Create(Category category, CancellationToken cancellationToken)
    {
        await _repository.CreateAsync(category, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        Category? category = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
        return category;
    }

    public async Task Edit(Category category, CancellationToken cancellationToken)
    {
        _repository.Update(category);
        await _repository.CommitAsync(cancellationToken);
        
    }

    public async Task<bool> Delete(int id, CancellationToken cancellationToken)
    {
        Category? category = await GetCategoryByIdAsync(id, cancellationToken: cancellationToken);
        if (category == null)
            return false;

        _repository.Delete(category);
        await _repository.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Category>> GetAsync
    (
        Expression<Func<Category, bool>>? expression = null,
        Expression<Func<Category, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _repository.GetAsync(expression, includes, tracked, cancellationToken);

    }
    
}