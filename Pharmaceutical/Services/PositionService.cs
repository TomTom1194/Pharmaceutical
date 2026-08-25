using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;
namespace Pharmaceutical.Services;
public class PositionService : IPositionService
{
    private readonly PharmaceuticalDbContext _context;
    public PositionService(PharmaceuticalDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Position>> GetAllAsync(bool onlyActive = false)
    {
        var query = _context.Positions.AsQueryable();
        if (onlyActive)
        {
            query = query.Where(p => p.IsActive);
        }
        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
    public async Task<Position?> GetByIdAsync(int id)
    {
        return await _context.Positions.FindAsync(id);
    }
    public async Task<Position> CreateAsync(Position position)
    {
        position.CreatedAt = DateTime.UtcNow;
        _context.Positions.Add(position);
        await _context.SaveChangesAsync();
        return position;
    }
    public async Task<bool> UpdateAsync(Position position)
    {
        var existing = await _context.Positions.FindAsync(position.PositionId);
        if (existing == null) return false;
        existing.Title = position.Title;
        existing.Department = position.Department;
        existing.Type = position.Type;
        existing.SalaryRange = position.SalaryRange;
        existing.Description = position.Description;
        existing.Requirements = position.Requirements;
        existing.IsActive = position.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Positions.FindAsync(id);
        if (existing == null) return false;
        _context.Positions.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
