using Pharmaceutical.Dtos;

namespace Pharmaceutical.Services;

public interface IQuoteService
{
    Task<List<QuoteRequestDto>> GetAll();
    Task<QuoteRequestDto?> GetById(int id);
    Task<QuoteRequestDto> Create(CreateQuoteDto dto);
    Task<bool> UpdateStatus(int id, UpdateQuoteStatusDto dto);
    Task<bool> Delete(int id);
}
