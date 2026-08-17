using System.Text;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class CapsuleSpecificationService : ICapsuleSpecificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CapsuleSpecificationService> _logger;

    public CapsuleSpecificationService(IHttpClientFactory httpClientFactory, ILogger<CapsuleSpecificationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<CapsuleSpecification>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/capsulespecification");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll capsule specs failed: {StatusCode}", response.StatusCode);
            return new List<CapsuleSpecification>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<CapsuleSpecification>>(json) ?? new List<CapsuleSpecification>();
    }

    public async Task<CapsuleSpecification?> GetById(int productId)
    {
        var response = await _httpClient.GetAsync($"api/capsulespecification/{productId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CapsuleSpecification>(json);
    }

    public async Task<bool> Create(CapsuleSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/capsulespecification", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int productId, CapsuleSpecification spec)
    {
        var content = new StringContent(JsonConvert.SerializeObject(spec), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/capsulespecification/{productId}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int productId)
    {
        var response = await _httpClient.DeleteAsync($"api/capsulespecification/{productId}");
        return response.IsSuccessStatusCode;
    }
}
