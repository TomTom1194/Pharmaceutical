using WebApp.Dtos;

namespace WebApp.Services;

public interface IAdminService
{
    Task<AdminCandidatesResultDto> GetCandidates(string token);
    Task<AdminCandidateDetailResultDto> GetCandidateDetail(string token, int candidateId);
    Task<InterviewInvitationResultDto> SendInvitation(string token, int candidateId, int? positionId, SendInterviewInvitationRequestDto request);
    Task<AdminResumeDownloadResult> DownloadResume(string token, int candidateId);
    Task<AdminPositionsResultDto> GetPositions(string token);
    Task<AdminPositionApplicationsResultDto> GetPositionApplications(string token, int positionId);
    Task<AdminPositionApplicationsResultDto> GetApplications(string token, int? positionId, string? status, string? keyword);
}

public class AdminResumeDownloadResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
}
