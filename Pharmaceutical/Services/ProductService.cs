using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task<Product> Create(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2);
        Task<Product?> Update(int id, Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2);
        Task<bool> Delete(int id);
    }

    public class ProductService : IProductService
    {
        private readonly PharmaceuticalDbContext _db;
        private readonly IWebHostEnvironment _env;
        private const string ImagesFolder = "uploads/products";

        public ProductService(PharmaceuticalDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _db.Products
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<Product?> GetById(int id)
        {
            return await _db.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> Create(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            await AddImages(product.ProductId, mainImage, subImage1, subImage2);

            return product;
        }

        public async Task<Product?> Update(int id, Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing == null)
                return null;

            existing.CategoryId = product.CategoryId;
            existing.ModelName = product.ModelName;
            existing.Summary = product.Summary;
            existing.Description = product.Description;
            existing.OutputLabel = product.OutputLabel;
            existing.IsPublished = product.IsPublished;

            await ReplaceImage(id, mainImage, displayOrder: 0, isThumbnail: true);
            await ReplaceImage(id, subImage1, displayOrder: 1, isThumbnail: false);
            await ReplaceImage(id, subImage2, displayOrder: 2, isThumbnail: false);

            await _db.SaveChangesAsync();

            return existing;
        }

        private async Task ReplaceImage(int productId, IFormFile? file, int displayOrder, bool isThumbnail)
        {
            if (file == null || file.Length == 0)
                return;

            var old = await _db.ImageProducts.FirstOrDefaultAsync(i => i.ProductId == productId && i.DisplayOrder == displayOrder);
            if (old != null)
            {
                DeleteImageFile(old.Url);
                _db.ImageProducts.Remove(old);
            }

            _db.ImageProducts.Add(await BuildImageProduct(productId, file, displayOrder, isThumbnail));
        }

        private void DeleteImageFile(string url)
        {
            var webRoot = string.IsNullOrEmpty(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            var filePath = Path.Combine(webRoot, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); } catch (IOException) { /* file may still be in use; ignore */ }
            }
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _db.Products.FindAsync(id);
            if (existing == null)
                return false;

            existing.IsPublished = false;
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task AddImages(int productId, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
        {
            var displayOrder = 0;

            if (mainImage != null && mainImage.Length > 0)
            {
                _db.ImageProducts.Add(await BuildImageProduct(productId, mainImage, displayOrder++, isThumbnail: true));
            }

            if (subImage1 != null && subImage1.Length > 0)
            {
                _db.ImageProducts.Add(await BuildImageProduct(productId, subImage1, displayOrder++, isThumbnail: false));
            }

            if (subImage2 != null && subImage2.Length > 0)
            {
                _db.ImageProducts.Add(await BuildImageProduct(productId, subImage2, displayOrder++, isThumbnail: false));
            }

            await _db.SaveChangesAsync();
        }

        private async Task<ImageProduct> BuildImageProduct(int productId, IFormFile file, int displayOrder, bool isThumbnail)
        {
            return new ImageProduct
            {
                ProductId = productId,
                DisplayOrder = displayOrder,
                IsThumbnail = isThumbnail,
                Url = await SaveImageFile(file)
            };
        }

        private async Task<string> SaveImageFile(IFormFile file)
        {
            var webRoot = string.IsNullOrEmpty(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            var uploadsFolder = Path.Combine(webRoot, ImagesFolder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{ImagesFolder}/{fileName}";
        }
    }
}
