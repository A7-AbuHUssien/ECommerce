namespace ECommerce.Repositories.Interfaces;

public interface IProductSubImage : IRepository<IProductSubImage>
{
    void RemoveRange(IEnumerable<ProductSubImageRepository> products);
}