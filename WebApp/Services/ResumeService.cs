using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApp.Dtos;

namespace WebApp.Services;

public class ResumeService : IResumeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResumeService> _logger;

    public ResumeService(IHttpClientFactory httpClientFactory, ILogger<ResumeService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<ResumeResultDto> GetCurrentResume(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/candidate/current");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            // No resume uploaded yet is a normal state, not an error.
            return new ResumeResultDto { Success = true, Data = null };
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetCurrentResume fail: {StatusCode}", response.StatusCode);
            return new ResumeResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load resume information."
            };
        }

        var data = JsonConvert.DeserializeObject<ResumeResponseDto>(responseJson);
        return new ResumeResultDto { Success = true, Data = data };
    }

    public async Task<ResumeResultDto> UploadResume(string token, IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);
        content.Add(streamContent, "resume", file.FileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/candidate/upload") { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("UploadResume fail: {StatusCode}", response.StatusCode);
            return new ResumeResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Resume upload failed. Please try again."
            };
        }

        var data = JsonConvert.DeserializeObject<ResumeResponseDto>(responseJson);
        return new ResumeResultDto { Success = true, Data = data };
    }

    public async Task<ResumeDownloadResultDto> DownloadResume(string token, int resumeId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/candidate/download/{resumeId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("DownloadResume fail: {StatusCode}", response.StatusCode);
            return new ResumeDownloadResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(errorJson) ?? "Could not download the CV file."
            };
        }

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName
                       ?? "resume";

        return new ResumeDownloadResultDto
        {
            Success = true,
            Content = bytes,
            ContentType = contentType,
            FileName = fileName.Trim('"')
        };
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
