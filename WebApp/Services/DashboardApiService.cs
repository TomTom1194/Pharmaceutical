using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp.Dtos;

namespace WebApp.Services;

public class DashboardApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _http = factory.CreateClient("PharmaApi");
    }

    public async Task<DashboardStatsDto?> GetStats()
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirst("JwtToken")?.Value;
        if (!string.IsNullOrEmpty(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await _http.GetAsync("api/dashboard/stats");
        return res.IsSuccessStatusCode
            ? await res.Content.ReadFromJsonAsync<DashboardStatsDto>()
            : null;
    }
}
