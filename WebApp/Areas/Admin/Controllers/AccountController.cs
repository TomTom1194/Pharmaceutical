using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _authService.ChangePassword(GetToken(), request);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Change password failed. Please try again.");
            return View(request);
        }

        TempData["SuccessMessage"] = "Password changed successfully.";
        return RedirectToAction("ChangePassword");
    }

    private string GetToken() => User.FindFirst("JwtToken")?.Value ?? string.Empty;
}
