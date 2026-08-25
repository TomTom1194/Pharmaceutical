using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Services;

namespace WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ApplicationController : Controller
{
    private readonly IAdminService _adminService;

    public ApplicationController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? positionId, string? status, string? keyword)
    {
        var positionsResult = await _adminService.GetPositions(GetToken());
        ViewBag.Positions = positionsResult.Data;
        if (!positionsResult.Success)
            ViewBag.ErrorMessage = positionsResult.ErrorMessage;

        ViewBag.SelectedPositionId = positionId;
        ViewBag.Status = status;
        ViewBag.Keyword = keyword;

        var applications = new List<AdminPositionApplicationItemDto>();
        var appsResult = await _adminService.GetApplications(GetToken(), positionId, status, keyword);
        if (appsResult.Success)
        {
            applications = appsResult.Data;
        }
        else
        {
            ViewBag.ErrorMessage = appsResult.ErrorMessage;
        }

        return View(applications);
    }

    [HttpGet]
    public async Task<IActionResult> CandidateDetail(int id, int? positionId)
    {
        var result = await _adminService.GetCandidateDetail(GetToken(), id, positionId);

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

    
    [HttpGet]
    public async Task<IActionResult> ViewResume(int id)
    {
        var result = await _adminService.ViewResume(GetToken(), id);

        if (!result.Success || result.Content == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Could not load resume.";
            return RedirectToAction("CandidateDetail", new { id });
        }

        return File(result.Content, result.ContentType ?? "application/octet-stream");
    }

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
