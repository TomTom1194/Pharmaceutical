using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Dtos;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class CandidateController : Controller
{
    private readonly IResumeService _resumeService;
    private readonly ICandidateProfileService _profileService;

    public CandidateController(IResumeService resumeService, ICandidateProfileService profileService)
    {
        _resumeService = resumeService;
        _profileService = profileService;
    }

    // Candidate Portal landing page.
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var result = await _profileService.GetProfile(GetToken());

        if (!result.Success)
            ViewBag.ErrorMessage = result.ErrorMessage;

        return View(result.Data);
    }

    // CV management page: shows the current uploaded resume file and offers
    // the "Create Resume" entry point for filling in structured resume data.
    [HttpGet]
    public async Task<IActionResult> ManageCv()
    {
        var result = await _resumeService.GetCurrentResume(GetToken());

        if (!result.Success)
            ViewBag.ErrorMessage = result.ErrorMessage;

        return View(result.Data);
    }

    // The real business rule is 5 MB (matches the API). The RequestSizeLimit here is
    // set well above that on purpose: if it were set to exactly 5 MB, Kestrel would
    // reject an oversized upload mid-stream by resetting the connection outright
    // (the browser shows "This site can't be reached" instead of a real error page).
    // Giving it headroom lets the request reach the action below, where we can return
    // a friendly, redirected error message instead.
    private const long MaxUploadSizeBytes = 5 * 1024 * 1024; // 5 MB, must match the API's limit

    [HttpPost]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> UploadCv(IFormFile resumeFile)
    {
        if (resumeFile == null || resumeFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please choose a file to upload.";
            return RedirectToAction("ManageCv");
        }

        if (resumeFile.Length > MaxUploadSizeBytes)
        {
            TempData["ErrorMessage"] =
                $"File \"{resumeFile.FileName}\" is {(resumeFile.Length / 1024.0 / 1024.0):0.00} MB, which exceeds the 5 MB limit.";
            return RedirectToAction("ManageCv");
        }

        var result = await _resumeService.UploadResume(GetToken(), resumeFile);

        if (result.Success)
            TempData["SuccessMessage"] = "Resume uploaded successfully.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction("ManageCv");
    }

    // "Create Resume": fill in personal info, education and work experience.
    [HttpGet]
    public async Task<IActionResult> CreateResume()
    {
        var result = await _profileService.GetProfile(GetToken());

        var model = new CandidateProfileUpdateRequestDto();
        if (result.Success && result.Data != null)
        {
            model.FullName = result.Data.FullName;
            model.Phone = result.Data.Phone;
            model.Address = result.Data.Address;
            model.Summary = result.Data.Summary;

            model.Educations = result.Data.Educations
                .Select(e => new EducationInputDto
                {
                    Institution = e.Institution,
                    Qualification = e.Qualification,
                    Field = e.Field,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .ToList();

            model.WorkExperiences = result.Data.WorkExperiences
                .Select(w => new WorkExperienceInputDto
                {
                    Employer = w.Employer,
                    Title = w.Title,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    Description = w.Description
                })
                .ToList();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateResume(CandidateProfileUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var result = await _profileService.UpdateProfile(GetToken(), request);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Could not save resume. Please try again.");
            return View(request);
        }

        TempData["SuccessMessage"] = "Resume saved successfully.";
        return RedirectToAction("ManageCv");
    }

    private string GetToken() => User.FindFirst("JwtToken")?.Value ?? string.Empty;
}
