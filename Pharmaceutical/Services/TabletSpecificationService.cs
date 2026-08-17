using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services
{
    public interface ITabletSpecificationService
    {
        Task<List<TabletSpecification>> GetAll();
        Task<TabletSpecification?> GetById(int productId);
        Task<TabletSpecification> Create(TabletSpecification spec);
        Task<TabletSpecification?> Update(int productId, TabletSpecification spec);
        Task<bool> Delete(int productId);
    }

    public class TabletSpecificationService : ITabletSpecificationService
    {
        private readonly PharmaceuticalDbContext _db;

        public TabletSpecificationService(PharmaceuticalDbContext db)
        {
            _db = db;
        }

        public async Task<List<TabletSpecification>> GetAll()
        {
            return await _db.TabletSpecifications.ToListAsync();
        }

        public async Task<TabletSpecification?> GetById(int productId)
        {
            return await _db.TabletSpecifications.FindAsync(productId);
        }

        public async Task<TabletSpecification> Create(TabletSpecification spec)
        {
            _db.TabletSpecifications.Add(spec);
            await _db.SaveChangesAsync();
            return spec;
        }

        public async Task<TabletSpecification?> Update(int productId, TabletSpecification spec)
        {
            var existing = await _db.TabletSpecifications.FindAsync(productId);
            if (existing == null)
                return null;

            existing.ModelNumber = spec.ModelNumber;
            existing.Dies = spec.Dies;
            existing.MaxPressure = spec.MaxPressure;
            existing.MaxDiameterMm = spec.MaxDiameterMm;
            existing.MaxDepthFillMm = spec.MaxDepthFillMm;
            existing.ProductionCapacity = spec.ProductionCapacity;
            existing.MachineSize = spec.MachineSize;
            existing.NetWeightKg = spec.NetWeightKg;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int productId)
        {
            var existing = await _db.TabletSpecifications.FindAsync(productId);
            if (existing == null)
                return false;

            _db.TabletSpecifications.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
