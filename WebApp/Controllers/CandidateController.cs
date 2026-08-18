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
    private readonly ICandidateProfileService _profileService;
    private readonly IApplicationService _applicationService;
    private readonly IResumeService _resumeService;

    public CandidateController(ICandidateProfileService profileService, IApplicationService applicationService, IResumeService resumeService)
    {
        _profileService = profileService;
        _applicationService = applicationService;
        _resumeService = resumeService;
    }

    
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var result = await _profileService.GetProfile(GetToken());

        if (!result.Success)
            ViewBag.ErrorMessage = result.ErrorMessage;

        return View(result.Data);
    }

   
    [HttpGet]
    public async Task<IActionResult> ExportCv()
    {
        var result = await _profileService.GetProfile(GetToken());

        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "CV information is not available.";
            return RedirectToAction("Profile");
        }

        return View(result.Data);
    }


    // Lists the positions this candidate has applied to.
    [HttpGet]
    public async Task<IActionResult> MyApplications()
    {
        var result = await _applicationService.GetMyApplications(GetToken());

        if (!result.Success)
            ViewBag.ErrorMessage = result.ErrorMessage;

        return View(result.Data);
    }

    
    [HttpGet]
    public async Task<IActionResult> UpdateResume()
    {
        var result = await _profileService.GetProfile(GetToken());
        ViewBag.HasProfileImage = result.Success && result.Data != null && result.Data.HasProfileImage;

        var resumeResult = await _resumeService.GetCurrentResume(GetToken());
        ViewBag.CurrentResume = resumeResult.Success ? resumeResult.Data : null;

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

    private const long MaxImageSizeBytes = 2 * 1024 * 1024; 
    private const long MaxResumeSizeBytes = 5 * 1024 * 1024; 
    private static readonly string[] AllowedResumeExtensions = { ".pdf", ".doc", ".docx" };

    [HttpPost]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> UpdateResume(CandidateProfileUpdateRequestDto request)
    {
        if (!ModelState.IsValid)
            return View(request);

        if (request.ProfileImageFile != null && request.ProfileImageFile.Length > MaxImageSizeBytes)
        {
            ModelState.AddModelError("",
                $"Image \"{request.ProfileImageFile.FileName}\" is {(request.ProfileImageFile.Length / 1024.0 / 1024.0):0.00} MB, which exceeds the 2 MB limit.");
            return View(request);
        }

        if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            var extension = Path.GetExtension(request.ResumeFile.FileName).ToLowerInvariant();
            if (!AllowedResumeExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "CV file must be a PDF, DOC or DOCX.");
                return View(request);
            }

            if (request.ResumeFile.Length > MaxResumeSizeBytes)
            {
                ModelState.AddModelError("",
                    $"CV file \"{request.ResumeFile.FileName}\" is {(request.ResumeFile.Length / 1024.0 / 1024.0):0.00} MB, which exceeds the 5 MB limit.");
                return View(request);
            }
        }

        var result = await _profileService.UpdateProfile(GetToken(), request);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Could not save resume. Please try again.");
            return View(request);
        }

        // Photo and CV file are saved together with the rest of the form, only if one was picked.
        if (request.ProfileImageFile != null && request.ProfileImageFile.Length > 0)
        {
            var imageResult = await _profileService.UploadProfileImage(GetToken(), request.ProfileImageFile);
            if (!imageResult.Success)
            {
                TempData["ErrorMessage"] = imageResult.ErrorMessage ?? "Resume saved, but the photo could not be uploaded.";
                return RedirectToAction("Profile");
            }
        }

        if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            var resumeResult = await _resumeService.UploadResume(GetToken(), request.ResumeFile);
            if (!resumeResult.Success)
            {
                TempData["ErrorMessage"] = resumeResult.ErrorMessage ?? "Resume saved, but the CV file could not be uploaded.";
                return RedirectToAction("Profile");
            }
        }

        TempData["SuccessMessage"] = "Resume saved successfully.";
        return RedirectToAction("Profile");
    }

    
    [HttpGet]
    public async Task<IActionResult> ProfileImage()
    {
        var result = await _profileService.GetProfileImage(GetToken());

        if (!result.Success || result.Content == null)
            return NotFound();

        return File(result.Content, result.ContentType ?? "application/octet-stream");
    }

    // Called from the Careers page "Apply Now" button — creates a real Application row via the API.
    [HttpPost]
    public async Task<IActionResult> Apply(int positionId)
    {
        var result = await _applicationService.Apply(GetToken(), positionId);

        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Could not submit your application. Please try again.";
            return RedirectToAction("Careers", "Page");
        }

        TempData["SuccessMessage"] = "Your application has been submitted successfully!";
        return RedirectToAction("Careers", "Page");
    }

    private string GetToken() => User.FindFirst("JwtToken")?.Value ?? string.Empty;
}
