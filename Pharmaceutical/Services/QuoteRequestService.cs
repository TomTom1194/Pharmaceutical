using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Models;
namespace Pharmaceutical.Services
{
    public interface IQuoteRequestService
    {
        Task<List<QuoteRequest>> GetAll();
        Task<QuoteRequest?> GetById(int id);
        Task<QuoteRequest> Create(QuoteRequest quoteRequest);
        Task<QuoteRequest?> Update(int id, QuoteRequest quoteRequest);
        Task<bool> Delete(int id);
    }
    public class QuoteRequestService : IQuoteRequestService
    {
        private readonly PharmaceuticalDbContext _db;
        public QuoteRequestService(PharmaceuticalDbContext db)
        {
            _db = db;
        }
        public async Task<List<QuoteRequest>> GetAll()
        {
            return await _db.QuoteRequests.ToListAsync();
        }
        public async Task<QuoteRequest?> GetById(int id)
        {
            return await _db.QuoteRequests.FindAsync(id);
        }
        public async Task<QuoteRequest> Create(QuoteRequest quoteRequest)
        {
            quoteRequest.SubmittedAt = DateTime.UtcNow;
            quoteRequest.Status = "Pending";
            _db.QuoteRequests.Add(quoteRequest);
            await _db.SaveChangesAsync();
            return quoteRequest;
        }
        public async Task<QuoteRequest?> Update(int id, QuoteRequest quoteRequest)
        {
            var existing = await _db.QuoteRequests.FindAsync(id);
            if (existing == null)
                return null;
            existing.FullName = quoteRequest.FullName;
            existing.CompanyName = quoteRequest.CompanyName;
            existing.Address = quoteRequest.Address;
            existing.City = quoteRequest.City;
            existing.State = quoteRequest.State;
            existing.PostalCode = quoteRequest.PostalCode;
            existing.Country = quoteRequest.Country;
            existing.Email = quoteRequest.Email;
            existing.Phone = quoteRequest.Phone;
            existing.Comments = quoteRequest.Comments;
            existing.Status = quoteRequest.Status;
            existing.HandledBy = quoteRequest.HandledBy;
            await _db.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> Delete(int id)
        {
            var existing = await _db.QuoteRequests.FindAsync(id);
            if (existing == null)
                return false;
            _db.QuoteRequests.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
