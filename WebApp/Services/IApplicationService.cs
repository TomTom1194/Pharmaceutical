using WebApp.Dtos;

namespace WebApp.Services;

public interface IApplicationService
{
    Task<ApplicationListResultDto> GetMyApplications(string token);
}
