using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Repositories.Interfaces;

public interface IProductColor: IRepository<ProductColor>
{
    void AddRange(IEnumerable<ProductColor> productColors,CancellationToken cancellationToken);
    void RemoveRange(IEnumerable<ProductColor> productColors);
}