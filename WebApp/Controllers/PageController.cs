using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
namespace WebApp.Controllers;
public class PageController : Controller
{
    private readonly ContentPageApiService _api;
    private readonly PositionApiService _positionApi;
    public PageController(ContentPageApiService api, PositionApiService positionApi)
    {
        _api = api;
        _positionApi = positionApi;
    }
    [Route("careers")]
    public async Task<IActionResult> Careers()
    {
        var model = new HomePageContentModel
        {
            HeroTitle = "Join Our Team",
            HeroSubtitle = "Help us build the future of pharmaceutical manufacturing with innovation and excellence.",
            AboutTitle = "Why Join Us",
            AboutDescription = "We are committed to innovation, uncompromising quality, and empowering every employee to achieve their absolute best."
        };
        var page = await _api.GetBySlug("careers");
        if (page != null)
        {
            if (!string.IsNullOrEmpty(page.Body))
            {
                try
                {
                    var json = System.Text.Json.JsonSerializer.Deserialize<HomePageContentModel>(page.Body);
                    if (json != null)
                    {
                        if (!string.IsNullOrEmpty(json.HeroTitle)) model.HeroTitle = json.HeroTitle;
                        if (!string.IsNullOrEmpty(json.HeroSubtitle)) model.HeroSubtitle = json.HeroSubtitle;
                        if (!string.IsNullOrEmpty(json.AboutTitle)) model.AboutTitle = json.AboutTitle;
                        if (!string.IsNullOrEmpty(json.AboutDescription)) model.AboutDescription = json.AboutDescription;
                    }
                }
                catch
                {
                }
            }
            if (!string.IsNullOrEmpty(page.BannerImageUrl))
            {
                ViewData["BannerImageUrl"] = page.BannerImageUrl;
            }
        }
        ViewData["Title"] = "Careers";
        ViewBag.Positions = await _positionApi.GetAllAsync(true); 
        return View("Careers", model);
    }
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
        return View(page);
    }
}
