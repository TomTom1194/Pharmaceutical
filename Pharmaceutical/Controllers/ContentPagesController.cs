using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Dtos;
using Pharmaceutical.Services;
namespace Pharmaceutical.Controllers;
[ApiController]
[Route("api/contentpages")]
public class ContentPagesController : ControllerBase
{
    private readonly IContentPageService _service;
    public ContentPagesController(IContentPageService service) => _service = service;
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAll());
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var dto = await _service.GetBySlug(slug);
        return dto is null ? NotFound() : Ok(dto);
    }
    [HttpGet("id/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetById(id);
        return dto is null ? NotFound() : Ok(dto);
    }
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveContentPageDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? adminUserId = int.TryParse(userIdString, out var uid) ? uid : null;
        var ok = await _service.Update(id, dto, adminUserId);
        return ok ? NoContent() : NotFound();
    }
}
