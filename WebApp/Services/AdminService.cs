using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WebApp.Dtos;

namespace WebApp.Services;

public class AdminService : IAdminService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IHttpClientFactory httpClientFactory, ILogger<AdminService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<AdminCandidatesResultDto> GetCandidates(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/admin/candidates");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetCandidates fail: {StatusCode}", response.StatusCode);
            return new AdminCandidatesResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load candidate list."
            };
        }

        var data = JsonConvert.DeserializeObject<List<AdminCandidateListItemDto>>(responseJson) ?? new();
        return new AdminCandidatesResultDto { Success = true, Data = data };
    }

    public async Task<AdminCandidateDetailResultDto> GetCandidateDetail(string token, int candidateId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/admin/candidates/{candidateId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetCandidateDetail fail: {StatusCode}", response.StatusCode);
            return new AdminCandidateDetailResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load candidate detail."
            };
        }

        var data = JsonConvert.DeserializeObject<AdminCandidateDetailDto>(responseJson);
        return new AdminCandidateDetailResultDto { Success = true, Data = data };
    }

    public async Task<InterviewInvitationResultDto> SendInvitation(string token, int candidateId, int? positionId, SendInterviewInvitationRequestDto request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"api/admin/candidates/{candidateId}/invite";
        if (positionId.HasValue)
            url += $"?positionId={positionId.Value}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest);
        var responseJson = await response.Content.ReadAsStringAsync();

        // 502 = the API recorded the invitation but the SMTP send itself failed.
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("SendInvitation fail: {StatusCode}", response.StatusCode);
            return new InterviewInvitationResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ??
                               "Could not send the invitation email. It may have been recorded as failed."
            };
        }

        var data = JsonConvert.DeserializeObject<InterviewInvitationDto>(responseJson);
        return new InterviewInvitationResultDto { Success = true, Data = data };
    }

    public async Task<AdminResumeDownloadResult> DownloadResume(string token, int candidateId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/admin/candidates/{candidateId}/resume/download");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("DownloadResume fail: {StatusCode}", response.StatusCode);
            return new AdminResumeDownloadResult
            {
                Success = false,
                ErrorMessage = ExtractMessage(errorJson) ?? "Could not download the resume file."
            };
        }

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                       ?? response.Content.Headers.ContentDisposition?.FileName
                       ?? "resume";

        return new AdminResumeDownloadResult
        {
            Success = true,
            Content = bytes,
            ContentType = contentType,
            FileName = fileName.Trim('"')
        };
    }

    public async Task<AdminPositionsResultDto> GetPositions(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/admin/positions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetPositions fail: {StatusCode}", response.StatusCode);
            return new AdminPositionsResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load positions."
            };
        }

        var data = JsonConvert.DeserializeObject<List<AdminPositionSummaryDto>>(responseJson) ?? new();
        return new AdminPositionsResultDto { Success = true, Data = data };
    }

    public async Task<AdminPositionApplicationsResultDto> GetPositionApplications(string token, int positionId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/admin/positions/{positionId}/applications");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GetPositionApplications fail: {StatusCode}", response.StatusCode);
            return new AdminPositionApplicationsResultDto
            {
                Success = false,
                ErrorMessage = ExtractMessage(responseJson) ?? "Could not load applications for this position."
            };
        }

        var data = JsonConvert.DeserializeObject<List<AdminPositionApplicationItemDto>>(responseJson) ?? new();
        return new AdminPositionApplicationsResultDto { Success = true, Data = data };
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
