using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ArticleController : Controller
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;

    public ArticleController(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
    {
        _http = factory.CreateClient("PharmaApi");
        _httpContextAccessor = httpContextAccessor;
        _env = env;
    }

    private void AttachToken()
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirst("JwtToken")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IActionResult> Index(string? search, string? sort)
    {
        AttachToken();
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        var url = $"api/articles/all?search={Uri.EscapeDataString(search ?? "")}&sort={Uri.EscapeDataString(sort ?? "")}";
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode) return View(new List<ArticleAdminDto>());
        var json = await res.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<ArticleAdminDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        return View(list);
    }

    public IActionResult Create() => View(new SaveArticleViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveArticleViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        AttachToken();
        if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images", "articles");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ThumbnailFile.FileName);
            using var fs = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
            await model.ThumbnailFile.CopyToAsync(fs);
            model.Thumbnail = "/images/articles/" + fileName;
        }
        var payload = JsonSerializer.Serialize(model);
        var res = await _http.PostAsync("api/articles", new StringContent(payload, Encoding.UTF8, "application/json"));
        if (res.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to create article. Slug may already exist.");
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        AttachToken();
        var res = await _http.GetAsync($"api/articles/id/{id}");
        if (!res.IsSuccessStatusCode) return NotFound();
        var json = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<ArticleAdminDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (dto is null) return NotFound();
        var model = new SaveArticleViewModel
        {
            Title       = dto.Title,
            Slug        = dto.Slug,
            Summary     = dto.Summary,
            Content     = dto.Content,
            Thumbnail   = dto.Thumbnail,
            AuthorName  = dto.AuthorName,
            PublishedAt = dto.PublishedAt,
            IsEditorPick = dto.IsEditorPick,
            Status      = dto.Status
        };
        ViewBag.ArticleId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaveArticleViewModel model)
    {
        if (!ModelState.IsValid) { ViewBag.ArticleId = id; return View(model); }
        AttachToken();
        if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images", "articles");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ThumbnailFile.FileName);
            using var fs = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
            await model.ThumbnailFile.CopyToAsync(fs);
            model.Thumbnail = "/images/articles/" + fileName;
        }
        var payload = JsonSerializer.Serialize(model);
        var res = await _http.PutAsync($"api/articles/{id}", new StringContent(payload, Encoding.UTF8, "application/json"));
        if (res.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ModelState.AddModelError("", "Failed to update article.");
        ViewBag.ArticleId = id;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEditorsPicks(List<int> ids)
    {
        if (ids != null && ids.Count > 5) return RedirectToAction(nameof(Index));
        ids ??= new List<int>();
        AttachToken();
        var payload = JsonSerializer.Serialize(ids);
        var res = await _http.PostAsync("api/articles/update-editors-picks", new StringContent(payload, Encoding.UTF8, "application/json"));
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        AttachToken();
        await _http.DeleteAsync($"api/articles/{id}");
        return RedirectToAction(nameof(Index));
    }
}




