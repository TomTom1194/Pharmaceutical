using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApp.Dtos;

namespace WebApp.Services;

public class CandidateProfileService : ICandidateProfileService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CandidateProfileService> _logger;

    public CandidateProfileService(IHttpClientFactory httpClientFactory, ILogger<CandidateProfileService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<CandidateProfileResultDto> GetProfile(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/candidate/profile");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetProfile fail: {StatusCode}", response.StatusCode);
            return new CandidateProfileResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load profile information."
            };
        }

        var data = JsonConvert.DeserializeObject<CandidateProfileDto>(responseJson);
        return new CandidateProfileResultDto { Success = true, Data = data };
    }

    public async Task<CandidateProfileResultDto> UpdateProfile(string token, CandidateProfileUpdateRequestDto request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/candidate/profile") { Content = content };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("UpdateProfile fail: {StatusCode}", response.StatusCode);
            return new CandidateProfileResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not save resume. Please try again."
            };
        }

        return new CandidateProfileResultDto { Success = true };
    }

    public async Task<CandidateProfileResultDto> UploadProfileImage(string token, IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(streamContent, "profileImage", file.FileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/candidate/profile-image") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("UploadProfileImage fail: {StatusCode}", response.StatusCode);
            return new CandidateProfileResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not upload the photo. Please try again."
            };
        }

        return new CandidateProfileResultDto { Success = true };
    }

    public async Task<ProfileImageDownloadResult> GetProfileImage(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/candidate/profile-image");
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
}
