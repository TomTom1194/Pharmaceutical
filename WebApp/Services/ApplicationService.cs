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

    public async Task<ApplicationSnapshotResultDto> GetApplicationDetail(string token, int applicationId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/candidate/applications/{applicationId}/detail");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetApplicationDetail fail: {StatusCode}", response.StatusCode);
            return new ApplicationSnapshotResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load application detail."
            };
        }

        var data = JsonConvert.DeserializeObject<ApplicationSnapshotDto>(responseJson);
        return new ApplicationSnapshotResultDto { Success = true, Data = data };
    }

    public async Task<ProfileImageDownloadResult> GetApplicationProfileImage(string token, int applicationId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/candidate/applications/{applicationId}/profile-image");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new ProfileImageDownloadResult { Success = false };

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        return new ProfileImageDownloadResult { Success = true, Content = bytes, ContentType = contentType };
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
