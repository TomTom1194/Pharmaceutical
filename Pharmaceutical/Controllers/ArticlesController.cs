using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Dtos;
using Pharmaceutical.Services;
namespace Pharmaceutical.Controllers;
[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;
    public ArticlesController(IArticleService service) => _service = service;
    [HttpGet]
    [HttpGet("editors-picks")]
    public async Task<IActionResult> GetEditorsPicks() => Ok(await _service.GetEditorsPicks());
    [HttpGet("published")]
    public async Task<IActionResult> GetPublished([FromQuery] string? search) =>
        Ok(await _service.GetPublished(search));
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort) =>
        Ok(await _service.GetAll(search, sort));
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var dto = await _service.GetBySlug(slug);
        return dto is null ? NotFound() : Ok(dto);
    }
    [HttpGet("id/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetById(id);
        return dto is null ? NotFound() : Ok(dto);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] SaveArticleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.Create(dto);
        return CreatedAtAction(nameof(GetBySlug), new { slug = created.Slug }, created);
    }
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveArticleDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await _service.Update(id, dto);
        return ok ? NoContent() : NotFound();
    }
    [HttpPost("update-editors-picks")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEditorsPicks([FromBody] List<int> ids)
    {
        if (ids != null && ids.Count > 5) return BadRequest("Please select a maximum of 5 articles.");
        ids ??= new List<int>();
        await _service.UpdateEditorsPicks(ids);
        return Ok();
    }
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.Delete(id);
        return ok ? NoContent() : NotFound();
    }
}
