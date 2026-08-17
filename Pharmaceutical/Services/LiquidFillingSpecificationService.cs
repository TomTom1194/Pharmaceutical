using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services
{
    public interface ILiquidFillingSpecificationService
    {
        Task<List<LiquidFillingSpecification>> GetAll();
        Task<LiquidFillingSpecification?> GetById(int productId);
        Task<LiquidFillingSpecification> Create(LiquidFillingSpecification spec);
        Task<LiquidFillingSpecification?> Update(int productId, LiquidFillingSpecification spec);
        Task<bool> Delete(int productId);
    }

    public class LiquidFillingSpecificationService : ILiquidFillingSpecificationService
    {
        private readonly PharmaceuticalDbContext _db;

        public LiquidFillingSpecificationService(PharmaceuticalDbContext db)
        {
            _db = db;
        }

        public async Task<List<LiquidFillingSpecification>> GetAll()
        {
            return await _db.LiquidFillingSpecifications.ToListAsync();
        }

        public async Task<LiquidFillingSpecification?> GetById(int productId)
        {
            return await _db.LiquidFillingSpecifications.FindAsync(productId);
        }

        public async Task<LiquidFillingSpecification> Create(LiquidFillingSpecification spec)
        {
            _db.LiquidFillingSpecifications.Add(spec);
            await _db.SaveChangesAsync();
            return spec;
        }

        public async Task<LiquidFillingSpecification?> Update(int productId, LiquidFillingSpecification spec)
        {
            var existing = await _db.LiquidFillingSpecifications.FindAsync(productId);
            if (existing == null)
                return null;

            existing.AirPressure = spec.AirPressure;
            existing.AirVolume = spec.AirVolume;
            existing.FillingSpeed = spec.FillingSpeed;
            existing.FillingRangeMl = spec.FillingRangeMl;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int productId)
        {
            var existing = await _db.LiquidFillingSpecifications.FindAsync(productId);
            if (existing == null)
                return false;

            _db.LiquidFillingSpecifications.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
