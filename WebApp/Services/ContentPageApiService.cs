using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Services;

public class ContentPageApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContentPageApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _http = factory.CreateClient("PharmaApi");
    }

    public async Task<List<ContentPageDto>> GetAll() =>
        await _http.GetFromJsonAsync<List<ContentPageDto>>("api/contentpages") ?? [];

    public async Task<ContentPageDto?> GetById(int id)
    {
        var res = await _http.GetAsync($"api/contentpages/id/{id}");
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ContentPageDto>() : null;
    }

    public async Task<ContentPageDto?> GetBySlug(string slug)
    {
        var res = await _http.GetAsync($"api/contentpages/{slug}");
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<ContentPageDto>() : null;
    }

    public async Task<(bool Success, string Error)> Update(int id, ContentPageDto form)
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirst("JwtToken")?.Value;
        if (string.IsNullOrEmpty(token))
        {
            return (false, "WebApp failed to find JwtToken in your session. Please try logging out and logging in again.");
        }

        var request = new HttpRequestMessage(HttpMethod.Put, $"api/contentpages/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new
        {
            title = form.Title,
            body = form.Body,
            bannerImageUrl = form.BannerImageUrl,
            status = form.Status
        });

        var res = await _http.SendAsync(request);
        
        if (res.IsSuccessStatusCode) return (true, "");
        var err = await res.Content.ReadAsStringAsync();
        return (false, $"Code {res.StatusCode}: {err}");
    }
}
