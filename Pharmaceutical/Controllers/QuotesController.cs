using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Dtos;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers;

[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _service;

    public QuotesController(IQuoteService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _service.Create(dto);
        return CreatedAtAction(nameof(Create), new { id = result.QuoteId }, result);
    }
}
