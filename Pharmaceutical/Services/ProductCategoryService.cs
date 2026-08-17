using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAll();
        Task<ProductCategory?> GetById(int id);
        Task<ProductCategory> Create(ProductCategory category);
        Task<ProductCategory?> Update(int id, ProductCategory category);
        Task<bool> Delete(int id);
    }

    public class ProductCategoryService : IProductCategoryService
    {
        private readonly PharmaceuticalDbContext _db;

        public ProductCategoryService(PharmaceuticalDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductCategory>> GetAll()
        {
            return await _db.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory?> GetById(int id)
        {
            return await _db.ProductCategories.FindAsync(id);
        }

        public async Task<ProductCategory> Create(ProductCategory category)
        {
            _db.ProductCategories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<ProductCategory?> Update(int id, ProductCategory category)
        {
            var existing = await _db.ProductCategories.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = category.Name;
            existing.Description = category.Description;
            existing.IsActive = category.IsActive;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _db.ProductCategories.FindAsync(id);
            if (existing == null)
                return false;

            existing.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
