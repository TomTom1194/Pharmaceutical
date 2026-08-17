using WebApp.Models;

namespace WebApp.Services;

public interface IProductService
{
    Task<List<Product>> GetAll();
    Task<Product?> GetById(int id);
    Task<bool> Create(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2);
    Task<bool> Update(int id, Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2);
    Task<bool> Delete(int id);
}
