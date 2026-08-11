using System.Text;
using Newtonsoft.Json;
using WebApp.Dtos;

namespace WebApp.Services;



public class AuthService :IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("PharmaApi");
        _logger = logger;
    }

    public async Task<LoginResponseDto?> Login(LoginRequestDto request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Login fail: {StatusCode}", response.StatusCode);
            return null;
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<LoginResponseDto>(responseJson);

        return result;
    }
}