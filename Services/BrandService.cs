using System.Linq.Expressions;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services;

public class BrandService : IBrandService
{
    private readonly IRepository<Brand> _repository;

    public BrandService(IRepository<Brand> repository)
    {
        _repository = repository;
    }

    public async Task Create(Brand brand, IFormFile? logo, CancellationToken cancellationToken)
    {
        brand.Logo = await UploadLogoAsync(logo, cancellationToken);
        await _repository.CreateAsync(brand, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task<Brand?> GetBrandByIdAsync(int id, CancellationToken cancellationToken)
    {
        Brand? brand = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
        return brand;
    }

    public async Task Edit(Brand brand, IFormFile? logo, CancellationToken cancellationToken)
    {
        try
        {
            string? oldLogo = brand.Logo;
            if (logo is not null)
            {
                // Delete logo file if exists
                if (!string.IsNullOrEmpty(brand.Logo))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Brand",
                        brand.Logo);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                brand.Logo = await UploadLogoAsync(logo, cancellationToken);
                _repository.Update(brand);
                await _repository.CommitAsync(cancellationToken);
            }
            else
            {
                brand.Logo = oldLogo;
                _repository.Update(brand);
                await _repository.CommitAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<bool> Delete(int id, CancellationToken cancellationToken)
    {
        Brand? brand = await GetBrandByIdAsync(id, cancellationToken: cancellationToken);
        if (brand == null)
            return false;

        // Delete logo file if exists
        if (!string.IsNullOrEmpty(brand.Logo))
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Brand", brand.Logo);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _repository.Delete(brand);
        await _repository.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<IEnumerable<Brand>> GetAsync
    (
        Expression<Func<Brand, bool>>? expression = null,
        Expression<Func<Brand, object>>[]? includes = null,
        bool tracked = true,
        CancellationToken cancellationToken = default
    )
    {
        return await _repository.GetAsync(tracked: false, cancellationToken: cancellationToken);
    }

    // ---------------------------------------------------------------------------------
    private async Task<string?> UploadLogoAsync(IFormFile? logo, CancellationToken cancellationToken)
    {
        if (logo == null || logo.Length == 0) return null;

        string fileName = Guid.NewGuid() + Path.GetExtension(logo.FileName);
        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Brand");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        string filePath = Path.Combine(uploadPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await logo.CopyToAsync(stream, cancellationToken);

        return fileName;
    }
}