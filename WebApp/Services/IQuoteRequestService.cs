using WebApp.Models;

namespace WebApp.Services;

public interface IQuoteRequestService
{
    Task<List<QuoteRequest>> GetAll();
    Task<QuoteRequest?> GetById(int id);
    Task<bool> Update(int id, QuoteRequest quoteRequest);
    Task<bool> Delete(int id);
}
