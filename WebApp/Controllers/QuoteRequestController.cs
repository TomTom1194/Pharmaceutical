using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class QuoteRequestController : Controller
{
    private readonly QuoteApiService _api;

    public QuoteRequestController(QuoteApiService api) => _api = api;

    [AllowAnonymous]
    public IActionResult Create() => View(new QuoteFormModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuoteFormModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ok = await _api.Submit(model);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Failed to submit your request. Please try again.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Your quote request has been submitted. We will contact you shortly.";
        return RedirectToAction(nameof(Create));
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var list = await _api.GetAll();
        return View(list);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _api.GetById(id);
        return dto is null ? NotFound() : View(dto);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        await _api.UpdateStatus(id, status);
        TempData["SuccessMessage"] = "Status updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _api.Delete(id);
        TempData["SuccessMessage"] = "Quote request deleted.";
        return RedirectToAction(nameof(Index));
    }
}
