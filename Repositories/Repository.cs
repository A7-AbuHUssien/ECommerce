using System.Linq.Expressions;
using ECommerce.DataAccess;
using ECommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entityCreated = await _dbSet.AddAsync(entity, cancellationToken);
        return entityCreated.Entity;
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
    public IQueryable<T> Query(
        bool tracked = true,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        if (!tracked)
            query = query.AsNoTracking();

        foreach (var include in includes)
            query = query.Include(include);

        return query;
    }

    public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        if (expression != null)
            query = query.Where(expression);
        query = ApplyIncludes(query, includes);
        if (!tracked)
            query = query.AsNoTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null, bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        if (expression != null)
            query = query.Where(expression);
        query = ApplyIncludes(query, includes);
        if (!tracked)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Commit failed: {e.Message}");
            return -1;
        }
    }

    public IQueryable<T> GetAsQuaryable(
        Expression<Func<T, bool>>? expression = null,
        Expression<Func<T, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (expression != null)
            query = query.Where(expression);
        query = ApplyIncludes(query, includes);
        if (!tracked)
            query = query.AsNoTracking();
        return query;
    }


    //                               WARNING => PRIVATE AREA
    // ------------------------------------------------------------------------------------------------
    private IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[]? includes)
    {
        if (includes is not null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return query;
    }
}