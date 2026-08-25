using System.Net.Http.Headers;
using Newtonsoft.Json;
using WebApp.Dtos;

namespace WebApp.Services;

public class ApplicationService : IApplicationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IHttpClientFactory httpClientFactory, ILogger<ApplicationService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<ApplicationListResultDto> GetMyApplications(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/candidate/applications");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetMyApplications fail: {StatusCode}", response.StatusCode);
            return new ApplicationListResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load your applications."
            };
        }

        var data = JsonConvert.DeserializeObject<List<ApplicationDto>>(responseJson) ?? new List<ApplicationDto>();
        return new ApplicationListResultDto { Success = true, Data = data };
    }

    public async Task<ApplyResultDto> Apply(string token, int positionId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/candidate/positions/{positionId}/apply");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Apply fail: {StatusCode}", response.StatusCode);
            return new ApplyResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not submit your application.",
                RequiresProfileCompletion = ExtractRequiresProfileCompletion(responseJson)
            };
        }

        return new ApplyResultDto { Success = true };
    }

    private static string? ExtractMessage(string json)
    {
        try
        {
            var obj = JsonConvert.DeserializeAnonymousType(json, new { message = "" });
            return obj?.message;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool ExtractRequiresProfileCompletion(string json)
    {
        try
        {
            var obj = JsonConvert.DeserializeAnonymousType(json, new { requiresProfileCompletion = false });
            return obj?.requiresProfileCompletion ?? false;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
