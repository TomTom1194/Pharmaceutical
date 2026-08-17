using System.Text;
using Newtonsoft.Json;
using WebApp.Models;

namespace WebApp.Services;

public class QuoteRequestService : IQuoteRequestService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<QuoteRequestService> _logger;

    public QuoteRequestService(IHttpClientFactory httpClientFactory, ILogger<QuoteRequestService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<List<QuoteRequest>> GetAll()
    {
        var response = await _httpClient.GetAsync("api/quoterequest");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetAll quote requests failed: {StatusCode}", response.StatusCode);
            return new List<QuoteRequest>();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<QuoteRequest>>(json) ?? new List<QuoteRequest>();
    }

    public async Task<QuoteRequest?> GetById(int id)
    {
        var response = await _httpClient.GetAsync($"api/quoterequest/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<QuoteRequest>(json);
    }

    public async Task<bool> Update(int id, QuoteRequest quoteRequest)
    {
        var content = new StringContent(JsonConvert.SerializeObject(quoteRequest), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/quoterequest/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/quoterequest/{id}");
        return response.IsSuccessStatusCode;
    }
}
