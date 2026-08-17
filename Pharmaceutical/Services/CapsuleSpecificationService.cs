using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;

namespace Pharmaceutical.Services
{
    public interface ICapsuleSpecificationService
    {
        Task<List<CapsuleSpecification>> GetAll();
        Task<CapsuleSpecification?> GetById(int productId);
        Task<CapsuleSpecification> Create(CapsuleSpecification spec);
        Task<CapsuleSpecification?> Update(int productId, CapsuleSpecification spec);
        Task<bool> Delete(int productId);
    }

    public class CapsuleSpecificationService : ICapsuleSpecificationService
    {
        private readonly PharmaceuticalDbContext _db;

        public CapsuleSpecificationService(PharmaceuticalDbContext db)
        {
            _db = db;
        }

        public async Task<List<CapsuleSpecification>> GetAll()
        {
            return await _db.CapsuleSpecifications.ToListAsync();
        }

        public async Task<CapsuleSpecification?> GetById(int productId)
        {
            return await _db.CapsuleSpecifications.FindAsync(productId);
        }

        public async Task<CapsuleSpecification> Create(CapsuleSpecification spec)
        {
            _db.CapsuleSpecifications.Add(spec);
            await _db.SaveChangesAsync();
            return spec;
        }

        public async Task<CapsuleSpecification?> Update(int productId, CapsuleSpecification spec)
        {
            var existing = await _db.CapsuleSpecifications.FindAsync(productId);
            if (existing == null)
                return null;

            existing.Output = spec.Output;
            existing.CapsuleSizeMm = spec.CapsuleSizeMm;
            existing.MachineDimension = spec.MachineDimension;
            existing.ShippingWeightKg = spec.ShippingWeightKg;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int productId)
        {
            var existing = await _db.CapsuleSpecifications.FindAsync(productId);
            if (existing == null)
                return false;

            _db.CapsuleSpecifications.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
