using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Repositories.Interfaces;

public interface IProductSubImageRepository : IRepository<ProductSubImage>
{
    Task<bool> RemoveRange(IEnumerable<ProductSubImage> subImages);
}