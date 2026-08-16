using System.Net.Http.Json;
using WebApp.Dtos;
using WebApp.Models;

namespace WebApp.Services;

public class QuoteApiService
{
    private readonly HttpClient _http;

    public QuoteApiService(IHttpClientFactory factory) =>
        _http = factory.CreateClient("PharmaApi");

    public async Task<bool> Submit(QuoteFormModel form)
    {
        var res = await _http.PostAsJsonAsync("api/quotes", form);
        return res.IsSuccessStatusCode;
    }

}
