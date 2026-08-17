using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Models;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapsuleSpecificationController : ControllerBase
    {
        private readonly ICapsuleSpecificationService _service;

        public CapsuleSpecificationController(ICapsuleSpecificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById(int productId)
        {
            var spec = await _service.GetById(productId);
            if (spec == null)
                return NotFound();

            return Ok(spec);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CapsuleSpecification spec)
        {
            var created = await _service.Create(spec);
            return CreatedAtAction(nameof(GetById), new { productId = created.ProductId }, created);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, CapsuleSpecification spec)
        {
            var updated = await _service.Update(productId, spec);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var deleted = await _service.Delete(productId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
