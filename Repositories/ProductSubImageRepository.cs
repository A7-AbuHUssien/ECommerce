using ECommerce.DataAccess;
using ECommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Repositories;

public class ProductSubImageRepository : Repository<ProductSubImage>, IProductSubImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductSubImageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<bool> RemoveRange(IEnumerable<ProductSubImage>? subImages)
    {
        if(subImages == null)
            return Task.FromResult(false);
        _context.ProductSubImages.RemoveRange(subImages);
        return Task.FromResult(true);
    }
}