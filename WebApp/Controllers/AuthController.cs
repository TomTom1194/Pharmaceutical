using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Services;

namespace WebApp.Controllers;

public class AuthController :Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Application", new { area = "Admin" });

            if (User.IsInRole("Candidate"))
                return RedirectToAction("Profile", "Candidate");

            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _authService.Login(request);

        if (result == null)
        {
            ModelState.AddModelError("", "Email or Password is incorrect");
            return View(request);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, result.Email),
            new Claim(ClaimTypes.Role, result.Role),
            new Claim("JwtToken", result.Token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));


        if (string.Equals(result.Role, "Candidate", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Profile", "Candidate");
        }

        if (string.Equals(result.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Application", new { area = "Admin" });

            if (User.IsInRole("Candidate"))
                return RedirectToAction("Profile", "Candidate");

            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _authService.Register(request);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Registration failed. Please try again.");
            return View(request);
        }

        TempData["SuccessMessage"] = "Account created successfully. Please log in.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
