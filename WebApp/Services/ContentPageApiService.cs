using System.Net.Http.Json;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Services;

public class ContentPageApiService
{
    private readonly HttpClient _http;

    public ContentPageApiService(IHttpClientFactory factory) =>
        _http = factory.CreateClient("PharmaApi");

    public async Task<List<ContentPageDto>> GetAll() =>
        await _http.GetFromJsonAsync<List<ContentPageDto>>("api/contentpages") ?? [];

    public async Task<ContentPageDto?> GetById(int id) =>
        await _http.GetFromJsonAsync<ContentPageDto>($"api/contentpages/id/{id}");

    public async Task<bool> Update(int id, ContentPageFormModel form)
    {
        var res = await _http.PutAsJsonAsync($"api/contentpages/{id}", new
        {
            title = form.Title,
            body = form.Body,
            status = form.Status
        });
        return res.IsSuccessStatusCode;
    }
}
