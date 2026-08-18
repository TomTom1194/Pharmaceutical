using WebApp.Models;

namespace WebApp.Services;

public interface ILiquidFillingSpecificationService
{
    Task<List<LiquidFillingSpecification>> GetAll();
    Task<LiquidFillingSpecification?> GetById(int productId);
    Task<bool> Create(LiquidFillingSpecification spec);
    Task<bool> Update(int productId, LiquidFillingSpecification spec);
    Task<bool> Delete(int productId);
}
