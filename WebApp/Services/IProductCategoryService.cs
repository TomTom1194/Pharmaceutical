using WebApp.Models;

namespace WebApp.Services;

public interface IProductCategoryService
{
    Task<List<ProductCategory>> GetAll();
    Task<ProductCategory?> GetById(int id);
    Task<bool> Create(ProductCategory category);
    Task<bool> Update(int id, ProductCategory category);
    Task<bool> Delete(int id);
}
