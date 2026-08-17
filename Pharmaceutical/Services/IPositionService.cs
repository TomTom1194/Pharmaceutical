using Pharmaceutical.Models;

namespace Pharmaceutical.Services;

public interface IPositionService
{
    Task<IEnumerable<Position>> GetAllAsync(bool onlyActive = false);
    Task<Position?> GetByIdAsync(int id);
    Task<Position> CreateAsync(Position position);
    Task<bool> UpdateAsync(Position position);
    Task<bool> DeleteAsync(int id);
}
