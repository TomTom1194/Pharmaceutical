using WebApp.Dtos;

namespace WebApp.Services;

public interface IApplicationService
{
    Task<ApplicationListResultDto> GetMyApplications(string token);
    Task<ApplyResultDto> Apply(string token, int positionId);
    Task<ApplicationSnapshotResultDto> GetApplicationDetail(string token, int applicationId);
    Task<ProfileImageDownloadResult> GetApplicationProfileImage(string token, int applicationId);
}
