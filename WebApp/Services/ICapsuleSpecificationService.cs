using WebApp.Models;

namespace WebApp.Services;

public interface ICapsuleSpecificationService
{
    Task<List<CapsuleSpecification>> GetAll();
    Task<CapsuleSpecification?> GetById(int productId);
    Task<bool> Create(CapsuleSpecification spec);
    Task<bool> Update(int productId, CapsuleSpecification spec);
    Task<bool> Delete(int productId);
}
