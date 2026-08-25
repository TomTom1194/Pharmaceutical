using Microsoft.AspNetCore.Http;
using WebApp.Dtos;

namespace WebApp.Services;

public interface IResumeService
{
    Task<ResumeResultDto> GetCurrentResume(string token);
    Task<ResumeResultDto> UploadResume(string token, IFormFile file);
    Task<ResumeDownloadResultDto> DownloadResume(string token, int resumeId);
}
