using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IHttpClientFactory httpClientFactory, ILogger<ProductService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<Product>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/product");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll products failed: {StatusCode}", response.StatusCode);
            return new List<Product>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Product>>(json) ?? new List<Product>();
    }

    public async Task<Product?> GetById(int id)
    {
        var response = await _httpClient.GetAsync($"api/product/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Product>(json);
    }

    public async Task<bool> Create(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
    {
        using var content = BuildFormContent(product, mainImage, subImage1, subImage2);
        var response = await _httpClient.PostAsync("api/product", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
    {
        using var content = BuildFormContent(product, mainImage, subImage1, subImage2);
        var response = await _httpClient.PutAsync($"api/product/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/product/{id}");
        return response.IsSuccessStatusCode;
    }

    private static MultipartFormDataContent BuildFormContent(Product product, IFormFile? mainImage, IFormFile? subImage1, IFormFile? subImage2)
    {
        var content = new MultipartFormDataContent();

        content.Add(new StringContent(product.ModelName), "ModelName");
        content.Add(new StringContent(product.CategoryId?.ToString() ?? ""), "CategoryId");
        content.Add(new StringContent(product.Summary ?? ""), "Summary");
        content.Add(new StringContent(product.Description ?? ""), "Description");
        content.Add(new StringContent(product.OutputLabel ?? ""), "OutputLabel");
        content.Add(new StringContent((product.IsPublished ?? true).ToString()), "IsPublished");

        AddImage(content, "mainImage", mainImage);
        AddImage(content, "subImage1", subImage1);
        AddImage(content, "subImage2", subImage2);

        return content;
    }

    private static void AddImage(MultipartFormDataContent content, string name, IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return;

        var fileContent = new StreamContent(file.OpenReadStream());
        content.Add(fileContent, name, file.FileName);
    }
}
