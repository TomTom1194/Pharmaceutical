using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class PageController : Controller
{
    private readonly ContentPageApiService _api;

    public PageController(ContentPageApiService api)
    {
        _api = api;
    }

    [Route("careers")]
    public Task<IActionResult> Careers() => Index("careers");

    [Route("page/{slug}")]
    public async Task<IActionResult> Index(string slug)
    {
        var page = await _api.GetBySlug(slug);
        
        if (page == null)
        {
            return NotFound();
        }

        bool isPreview = Request.Query["preview"] == "true" && User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin");
        if (page.Status != "Published" && !isPreview)
        {
            return View("DraftMaintenance");
        }

        if (slug == "careers")
        {
            var model = new HomePageContentModel();
            if (!string.IsNullOrEmpty(page.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(page.Body);
                    if (json != null) model = json;
                }
                catch { }
            }
            ViewData["BannerImageUrl"] = page.BannerImageUrl;
            ViewData["Title"] = page.Title;
            return View("Careers", model);
        }

        return View(page);
    }
}
