using Pharmaceutical.Models;

namespace WebApp.Services;

public class PositionApiService
{
    private readonly HttpClient _httpClient;

    public PositionApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
    }

    public async Task<IEnumerable<Position>> GetAllAsync(bool onlyActive = false)
    {
        var response = await _httpClient.GetAsync($"/api/positions?onlyActive={onlyActive}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Position>>() ?? Array.Empty<Position>();
        }
        return Array.Empty<Position>();
    }

    public async Task<Position?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/positions/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Position>();
        }
        return null;
    }

    public async Task<bool> CreateAsync(Position position, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/positions");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(position);
        
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(int id, Position position, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/positions/{id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(position);
        
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/positions/{id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
