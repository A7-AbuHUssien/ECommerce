using System.Linq.Expressions;
using ECommerce.Repositories.Interfaces;
using ECommerce.Services.Interfaces;

namespace ECommerce.Services;

 public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IProductSubImageRepository _subImageRepository;
        private readonly IProductColor _colorRepository;

        public ProductService(
            IRepository<Product> productRepository,
            IProductSubImageRepository subImageRepository,
            IProductColor colorRepository)
        {
            _productRepository = productRepository;
            _subImageRepository = subImageRepository;
            _colorRepository = colorRepository;
        }

        public async Task<IEnumerable<Product>> GetAsync(bool tracked = true, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetAsync(tracked: tracked, cancellationToken: cancellationToken,
                includes: new Expression<Func<Product, object>>[]
                {
                    p => p.Brand,
                    p => p.Category
                });
        }

        public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _productRepository.GetOneAsync(
                p => p.Id == id,
                includes: new Expression<Func<Product, object>>[] { p => p.Brand, p => p.Category, p => p.SubImages, p => p.Colors },
                tracked: true,
                cancellationToken: cancellationToken
            );
        }

        public async Task<Product> CreateAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors, CancellationToken cancellationToken = default)
        {
            if (mainImg != null)
                product.MainImg = await HandleImgAsync(mainImg, new List<string> { "wwwroot", "Uploads", "Product" });

            var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);
            await _productRepository.CommitAsync(cancellationToken);

            await HandleSubImagesAsync(createdProduct, subImages);
            await HandleColorsAsync(createdProduct, colors);

            return createdProduct;
        }

        public async Task EditAsync(Product product, IFormFile? mainImg, List<IFormFile>? subImages, string[]? colors, CancellationToken cancellationToken = default)
        {
            var oldProduct = await _productRepository.GetOneAsync(x => x.Id == product.Id, tracked: false);

            if (mainImg != null)
                product.MainImg = await HandleImgAsync(mainImg, new List<string> { "wwwroot", "Uploads", "Product" });
            else
                product.MainImg = oldProduct?.MainImg;

            _productRepository.Update(product);
            await _productRepository.CommitAsync(cancellationToken);

            await HandleSubImagesAsync(product, subImages);
            await HandleColorsAsync(product, colors);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id, cancellationToken: cancellationToken);
            if (product == null) return false;

            var subImages = await _subImageRepository.GetAsync(p => p.ProductId == id,cancellationToken: cancellationToken);
            foreach (var sub in subImages)
            {
                var subPath = Path.Combine("wwwroot", "Uploads", "Product", "ProductSubImg", sub.Img);
                if (File.Exists(subPath)) File.Delete(subPath);
                _subImageRepository.Delete(sub);
            }

            var colors = await _colorRepository.GetAsync(c => c.ProductId == id);
            _colorRepository.RemoveRange(colors);

            var mainPath = Path.Combine("wwwroot", "Uploads", "Product", product.MainImg ?? "");
            if (File.Exists(mainPath)) File.Delete(mainPath);

            _productRepository.Delete(product);
            await _productRepository.CommitAsync(cancellationToken);
            await _subImageRepository.CommitAsync(cancellationToken);
            await _colorRepository.CommitAsync(cancellationToken);

            return true;
        }

        // ---------------------- PRIVATE ----------------------
        private async Task<string> HandleImgAsync(IFormFile file, List<string> paths)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var uploadPath = Path.Combine(paths[0], Path.Combine(paths.Skip(1).ToArray()));

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        private async Task HandleSubImagesAsync(Product product, List<IFormFile>? subImages)
        {
            if (subImages == null || !subImages.Any()) return;

            foreach (var subfile in subImages)
            {
                var filename = await HandleImgAsync(subfile, new List<string> { "wwwroot", "Uploads", "Product", "ProductSubImg" });
                await _subImageRepository.CreateAsync(new ProductSubImage
                {
                    ProductId = product.Id,
                    Img = filename
                });
            }
            await _subImageRepository.CommitAsync();
        }

        private async Task HandleColorsAsync(Product product, string[]? colors)
        {
            if (colors == null || !colors.Any()) return;

            var oldColors = await _colorRepository.GetAsync(c => c.ProductId == product.Id);
            _colorRepository.RemoveRange(oldColors);

            foreach (var color in colors)
            {
                await _colorRepository.CreateAsync(new ProductColor
                {
                    ProductId = product.Id,
                    Color = color
                });
            }
            await _colorRepository.CommitAsync();
        }
    }