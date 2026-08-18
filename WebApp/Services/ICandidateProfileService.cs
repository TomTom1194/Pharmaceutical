using Microsoft.AspNetCore.Http;
using WebApp.Dtos;

namespace WebApp.Services;

public interface ICandidateProfileService
{
    Task<CandidateProfileResultDto> GetProfile(string token);
    Task<CandidateProfileResultDto> UpdateProfile(string token, CandidateProfileUpdateRequestDto request);
    Task<CandidateProfileResultDto> UploadProfileImage(string token, IFormFile file);
    Task<ProfileImageDownloadResult> GetProfileImage(string token);
}

public class ProfileImageDownloadResult
{
    public bool Success { get; set; }
    public byte[]? Content { get; set; }
    public string? ContentType { get; set; }
}
