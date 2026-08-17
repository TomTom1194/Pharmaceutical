using System.Text;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class TabletSpecificationService : ITabletSpecificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TabletSpecificationService> _logger;

    public TabletSpecificationService(IHttpClientFactory httpClientFactory, ILogger<TabletSpecificationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<TabletSpecification>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/tabletspecification");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll tablet specs failed: {StatusCode}", response.StatusCode);
            return new List<TabletSpecification>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TabletSpecification>>(json) ?? new List<TabletSpecification>();
    }

    public async Task<TabletSpecification?> GetById(int productId)
    {
        var response = await _httpClient.GetAsync($"api/tabletspecification/{productId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TabletSpecification>(json);
    }

    public async Task<bool> Create(TabletSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/tabletspecification", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int productId, TabletSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/tabletspecification/{productId}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int productId)
    {
        var response = await _httpClient.DeleteAsync($"api/tabletspecification/{productId}");
        return response.IsSuccessStatusCode;
    }
}
