using System.Text;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class LiquidFillingSpecificationService : ILiquidFillingSpecificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LiquidFillingSpecificationService> _logger;

    public LiquidFillingSpecificationService(IHttpClientFactory httpClientFactory, ILogger<LiquidFillingSpecificationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<LiquidFillingSpecification>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/liquidfillingspecification");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll liquid filling specs failed: {StatusCode}", response.StatusCode);
            return new List<LiquidFillingSpecification>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<LiquidFillingSpecification>>(json) ?? new List<LiquidFillingSpecification>();
    }

    public async Task<LiquidFillingSpecification?> GetById(int productId)
    {
        var response = await _httpClient.GetAsync($"api/liquidfillingspecification/{productId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<LiquidFillingSpecification>(json);
    }

    public async Task<bool> Create(LiquidFillingSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/liquidfillingspecification", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int productId, LiquidFillingSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/liquidfillingspecification/{productId}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int productId)
    {
        var response = await _httpClient.DeleteAsync($"api/liquidfillingspecification/{productId}");
        return response.IsSuccessStatusCode;
    }
}
