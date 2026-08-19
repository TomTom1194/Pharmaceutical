using System.Net.Http.Headers;
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

    public async Task<RegisterResultDto> Register(RegisterRequestDto request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/register", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Register fail: {StatusCode}", response.StatusCode);

            string? message = null;
            try
            {
                var errorObj = JsonConvert.DeserializeAnonymousType(responseJson, new { message = "" });
                message = errorObj?.message;
            }
            catch (JsonException)
            {
                // response body wasn't the expected { message: "" } shape, ignore
            }

            return new RegisterResultDto
            {
                Success = false,
                ErrorMessage = message ?? "Registration failed. Please try again."
            };
        }

        var result = JsonConvert.DeserializeObject<RegisterResponseDto>(responseJson);
        return new RegisterResultDto
        {
            Success = true,
            Data = result
        };
    }

    public async Task<ChangePasswordResultDto> ChangePassword(string token, ChangePasswordRequestDto request)
    {
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/auth/change-password")
        {
            Content = content
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(requestMessage);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("ChangePassword fail: {StatusCode}", response.StatusCode);

            string? message = null;
            try
            {
                var errorObj = JsonConvert.DeserializeAnonymousType(responseJson, new { message = "" });
                message = errorObj?.message;
            }
            catch (JsonException)
            {

            }

            return new ChangePasswordResultDto
            {
                Success = false,
                ErrorMessage = message ?? "Change password failed. Please try again."
            };
        }

        return new ChangePasswordResultDto { Success = true };
    }
}
