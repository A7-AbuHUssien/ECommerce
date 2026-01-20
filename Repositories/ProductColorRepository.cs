using ECommerce.DataAccess;
using ECommerce.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Repositories;

public class ProductColorRepository: Repository<ProductColor>,IProductColor
{
    private ApplicationDbContext _context;
    public ProductColorRepository(ApplicationDbContext context):base(context)
    {
        _context = context;
    }

    public void AddRange(IEnumerable<ProductColor> productColors,CancellationToken cancellationToken = default)
    {
        _context.AddRangeAsync(productColors, cancellationToken);
    }

    public void RemoveRange(IEnumerable<ProductColor> productColors)
    {
        _context.RemoveRange(productColors);
    }
}