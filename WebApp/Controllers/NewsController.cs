using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Controllers;

public class NewsController : Controller
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOpt = new() { PropertyNameCaseInsensitive = true };

    public NewsController(IHttpClientFactory factory) =>
        _http = factory.CreateClient("PharmaApi");

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        ViewBag.Search = search;
        var url = "api/articles/published" + (search != null ? "?search=" + Uri.EscapeDataString(search) : "");
        
        var list = new List<ArticleAdminDto>();
        var res = await _http.GetAsync(url);
        if (res.IsSuccessStatusCode)
        {
            var json = await res.Content.ReadAsStringAsync();
            list = JsonSerializer.Deserialize<List<ArticleAdminDto>>(json, _jsonOpt) ?? new();
        }

        var picksRes = await _http.GetAsync("api/articles/editors-picks");
        var picks = new List<ArticleAdminDto>();
        if (picksRes.IsSuccessStatusCode)
        {
            var json = await picksRes.Content.ReadAsStringAsync();
            picks = JsonSerializer.Deserialize<List<ArticleAdminDto>>(json, _jsonOpt) ?? new();
        }
        ViewBag.EditorsPicks = picks;

        ViewBag.Top3 = list.Take(3).ToList();
        var allOthers = list.Skip(3).ToList();
        
        int pageSize = 4;
        int totalPages = Math.Max(1, (int)Math.Ceiling(allOthers.Count / (double)pageSize));
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;
        
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        
        var pagedOthers = allOthers.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        return View(pagedOthers);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var res = await _http.GetAsync($"api/articles/{slug}");
        if (!res.IsSuccessStatusCode) return NotFound();
        var json = await res.Content.ReadAsStringAsync();
        var article = JsonSerializer.Deserialize<ArticleAdminDto>(json, _jsonOpt);
        if (article is null) return NotFound();

        var sugRes = await _http.GetAsync("api/articles/published");
        if (sugRes.IsSuccessStatusCode)
        {
            var sugJson = await sugRes.Content.ReadAsStringAsync();
            var all = JsonSerializer.Deserialize<List<ArticleAdminDto>>(sugJson, _jsonOpt) ?? new();
            ViewBag.Suggestions = all.Where(x => x.Id != article.Id).OrderByDescending(x => x.PublishedAt).Take(3).ToList();
        }

        return View(article);
    }
}
