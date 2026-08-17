using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class QuoteRequestController : Controller
{
    private readonly QuoteApiService _api;
    private readonly ContentPageApiService _contentApi;

    public QuoteRequestController(QuoteApiService api, ContentPageApiService contentApi)
    {
        _api = api;
        _contentApi = contentApi;
    }

    [AllowAnonymous]
    [HttpGet("Quote")]
    public async Task<IActionResult> Create()
    {
        await LoadPageContent();
        if (ViewData["IsDraft"] is true) return View("DraftMaintenance");
        return View(new QuoteFormModel());
    }

    [HttpPost("Quote")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuoteFormModel model)
    {
        await LoadPageContent();
        if (ViewData["IsDraft"] is true) return View("DraftMaintenance");

        if (!ModelState.IsValid) 
        {
            return View(model);
        }

        var ok = await _api.Submit(model);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Failed to submit your request. Please try again.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Your quote request has been submitted. We will contact you shortly.";
        return RedirectToAction(nameof(Create));
    }

    private async Task LoadPageContent()
    {
        var page = await _contentApi.GetBySlug("quote");
        
        bool isPreview = Request.Query["preview"] == "true" && User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin");
        if (page != null && page.Status != "Published" && !isPreview)
        {
            ViewData["IsDraft"] = true;
            return;
        }

        var model = new HomePageContentModel();
        
        if (page != null && !string.IsNullOrEmpty(page.Body))
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(page.Body);
                if (json != null) model = json;
            }
            catch { }
        }
        
        ViewBag.ContentModel = model;
    }
}
