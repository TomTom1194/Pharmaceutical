using System.Text;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductCategoryService> _logger;

    public ProductCategoryService(IHttpClientFactory httpClientFactory, ILogger<ProductCategoryService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<ProductCategory>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/productcategory");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll categories failed: {StatusCode}", response.StatusCode);
            return new List<ProductCategory>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<ProductCategory>>(json) ?? new List<ProductCategory>();
    }

    public async Task<ProductCategory?> GetById(int id)
    {
        var response = await _httpClient.GetAsync($"api/productcategory/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ProductCategory>(json);
    }

    public async Task<bool> Create(ProductCategory category)
    {
        var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/productcategory", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, ProductCategory category)
    {
        var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/productcategory/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/productcategory/{id}");
        return response.IsSuccessStatusCode;
    }
}
