using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Models;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Position>>> GetPositions([FromQuery] bool onlyActive = false)
    {
        var positions = await _positionService.GetAllAsync(onlyActive);
        return Ok(positions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Position>> GetPosition(int id)
    {
        var position = await _positionService.GetByIdAsync(id);
        if (position == null) return NotFound();
        return Ok(position);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Position>> CreatePosition(Position position)
    {
        var created = await _positionService.CreateAsync(position);
        return CreatedAtAction(nameof(GetPosition), new { id = created.PositionId }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePosition(int id, Position position)
    {
        if (id != position.PositionId) return BadRequest();
        var success = await _positionService.UpdateAsync(position);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePosition(int id)
    {
        var success = await _positionService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
