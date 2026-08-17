using WebApp.Dtos;

namespace WebApp.Services;

public interface IAuthService
{
    public Task<LoginResponseDto?> Login(LoginRequestDto request);
    public Task<RegisterResultDto> Register(RegisterRequestDto request);
}
