using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ContentPageApiService _contentApi;

    public HomeController(ILogger<HomeController> logger, ContentPageApiService contentApi)
    {
        _logger = logger;
        _contentApi = contentApi;
    }

    public async Task<IActionResult> Index()
    {
        var page = await _contentApi.GetBySlug("home");
        
        bool isPreview = Request.Query["preview"] == "true" && User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin");
        if (page != null && page.Status != "Published" && !isPreview)
        {
            return View("DraftMaintenance");
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
        
        ViewData["BannerImageUrl"] = page?.BannerImageUrl;
        return View(model);
    }

    [HttpGet("about")]
    public async Task<IActionResult> About()
    {
        var page = await _contentApi.GetBySlug("about-us");
        
        bool isPreview = Request.Query["preview"] == "true" && User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Admin");
        if (page != null && page.Status != "Published" && !isPreview)
        {
            return View("DraftMaintenance");
        }

        var model = new HomePageContentModel();

        if (page != null && !string.IsNullOrEmpty(page.Body))
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(page.Body);
                if (json != null) model = json;
            }
            catch 
            { 
                // Fallback for old HTML content
                model.AboutDescription = page.Body;
            }
        }

        ViewData["Title"] = page?.Title ?? "About Us";
        ViewData["BannerImageUrl"] = page?.BannerImageUrl;
        return View(model);
    }
}