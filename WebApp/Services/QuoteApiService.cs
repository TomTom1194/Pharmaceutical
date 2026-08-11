using System.Net.Http.Json;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Services;

public class QuoteApiService
{
    private readonly HttpClient _http;

    public QuoteApiService(IHttpClientFactory factory) =>
        _http = factory.CreateClient("PharmaApi");

    public async Task<List<QuoteRequestDto>> GetAll() =>
        await _http.GetFromJsonAsync<List<QuoteRequestDto>>("api/quotes") ?? [];

    public async Task<QuoteRequestDto?> GetById(int id) =>
        await _http.GetFromJsonAsync<QuoteRequestDto>($"api/quotes/{id}");

    public async Task<bool> Submit(QuoteFormModel form)
    {
        var res = await _http.PostAsJsonAsync("api/quotes", form);
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateStatus(int id, string status)
    {
        var res = await _http.PatchAsJsonAsync($"api/quotes/{id}/status", new { status });
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var res = await _http.DeleteAsync($"api/quotes/{id}");
        return res.IsSuccessStatusCode;
    }
}
