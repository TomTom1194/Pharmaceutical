using WebApp.Models;

namespace WebApp.Services;

public interface ITabletSpecificationService
{
    Task<List<TabletSpecification>> GetAll();
    Task<TabletSpecification?> GetById(int productId);
    Task<bool> Create(TabletSpecification spec);
    Task<bool> Update(int productId, TabletSpecification spec);
    Task<bool> Delete(int productId);
}
