using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Services;

namespace WebApp.Controllers;

// Admin Portal: candidate CV management. Only accounts whose Role claim is
// "Admin" (set at login from UserAccount.Role) can reach any action here.
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // "Applications": pick a position, see who applied to it.
    [HttpGet]
    public async Task<IActionResult> Index(int? positionId)
    {
        var positionsResult = await _adminService.GetPositions(GetToken());
        ViewBag.Positions = positionsResult.Data;
        if (!positionsResult.Success)
            ViewBag.ErrorMessage = positionsResult.ErrorMessage;

        ViewBag.SelectedPositionId = positionId;

        var applications = new List<AdminPositionApplicationItemDto>();
        if (positionId.HasValue)
        {
            var appsResult = await _adminService.GetPositionApplications(GetToken(), positionId.Value);
            if (appsResult.Success)
            {
                applications = appsResult.Data;
            }
            else
            {
                ViewBag.ErrorMessage = appsResult.ErrorMessage;
            }
        }

        return View(applications);
    }

    // CV detail page — Admin only. positionId (when present) is the application
    // context the admin came from, so "Send Invitation" can mark that specific
    // application as Sent.
    [HttpGet]
    public async Task<IActionResult> CandidateDetail(int id, int? positionId)
    {
        var result = await _adminService.GetCandidateDetail(GetToken(), id);

        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Candidate not found.";
            return RedirectToAction("Index");
        }

        ViewBag.PositionId = positionId;
        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadResume(int id)
    {
        var result = await _adminService.DownloadResume(GetToken(), id);

        if (!result.Success || result.Content == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Could not download resume.";
            return RedirectToAction("CandidateDetail", new { id });
        }

        return File(result.Content, result.ContentType ?? "application/octet-stream", result.FileName ?? "resume");
    }

    // Recruitment email action: sends the interview invitation and records it.
    [HttpPost]
    public async Task<IActionResult> SendInvitation(int id, int? positionId, SendInterviewInvitationRequestDto request)
    {
        var result = await _adminService.SendInvitation(GetToken(), id, positionId, request);

        if (result.Success)
            TempData["SuccessMessage"] = "Invitation email sent successfully.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to send invitation.";

        return RedirectToAction("CandidateDetail", new { id, positionId });
    }

    private string GetToken() => User.FindFirst("JwtToken")?.Value ?? string.Empty;
}
