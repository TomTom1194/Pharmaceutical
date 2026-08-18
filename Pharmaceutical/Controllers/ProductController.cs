using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Models;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
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
            var product = await _service.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
        {
            var created = await _service.Create(product, mainImage, subImage1, subImage2);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
        {
            var updated = await _service.Update(id, product, mainImage, subImage1, subImage2);
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
