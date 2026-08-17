using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmaceutical.Models;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PositionController : Controller
{
    private readonly PositionApiService _api;

    public PositionController(PositionApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var positions = await _api.GetAllAsync();
        return View(positions);
    }

    public IActionResult Create()
    {
        return View(new Position());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Position position)
    {
        if (ModelState.IsValid)
        {
            var token = Request.Cookies["jwtToken"];
            if (token != null)
            {
                var success = await _api.CreateAsync(position, token);
                if (success)
                {
                    TempData["SuccessMessage"] = "Position created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("", "Failed to create position. Please try again.");
        }
        return View(position);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var position = await _api.GetByIdAsync(id);
        if (position == null) return NotFound();
        return View(position);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Position position)
    {
        if (id != position.PositionId) return BadRequest();

        if (ModelState.IsValid)
        {
            var token = Request.Cookies["jwtToken"];
            if (token != null)
            {
                var success = await _api.UpdateAsync(id, position, token);
                if (success)
                {
                    TempData["SuccessMessage"] = "Position updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("", "Failed to update position. Please try again.");
        }
        return View(position);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var token = Request.Cookies["jwtToken"];
        if (token != null)
        {
            var success = await _api.DeleteAsync(id, token);
            if (success)
            {
                TempData["SuccessMessage"] = "Position deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete position.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
