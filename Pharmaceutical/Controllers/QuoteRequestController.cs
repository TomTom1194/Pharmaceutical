using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Models;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteRequestController : ControllerBase
    {
        private readonly IQuoteRequestService _service;

        public QuoteRequestController(IQuoteRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quoteRequest = await _service.GetById(id);
            if (quoteRequest == null)
                return NotFound();

            return Ok(quoteRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuoteRequest quoteRequest)
        {
            var created = await _service.Create(quoteRequest);
            return CreatedAtAction(nameof(GetById), new { id = created.QuoteId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuoteRequest quoteRequest)
        {
            var updated = await _service.Update(id, quoteRequest);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
