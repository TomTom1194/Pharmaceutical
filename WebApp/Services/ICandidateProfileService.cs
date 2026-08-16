using WebApp.Dtos;

namespace WebApp.Services;

public interface ICandidateProfileService
{
    Task<CandidateProfileResultDto> GetProfile(string token);
    Task<CandidateProfileResultDto> UpdateProfile(string token, CandidateProfileUpdateRequestDto request);
}
