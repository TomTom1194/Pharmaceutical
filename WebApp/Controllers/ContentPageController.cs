using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class ContentPageController : Controller
{
    private readonly ContentPageApiService _api;

    public ContentPageController(ContentPageApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAll();
        return View(list);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _api.GetById(id);
        if (dto is null) return NotFound();

        var model = new ContentPageFormModel
        {
            PageId = dto.PageId,
            Slug = dto.Slug,
            Title = dto.Title ?? "",
            Body = dto.Body,
            Status = dto.Status ?? "Draft"
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContentPageFormModel model)
    {
        if (id != model.PageId) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var ok = await _api.Update(id, model);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Failed to update page.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Page updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
