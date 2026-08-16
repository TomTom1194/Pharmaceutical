using Pharmaceutical.Dtos;

namespace Pharmaceutical.Services;

public interface IQuoteService
{
    Task<QuoteRequestDto> Create(CreateQuoteDto dto);
}
