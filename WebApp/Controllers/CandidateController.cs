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

        
        var existingProfile = await _profileService.GetProfile(GetToken());
        var hasProfileImage = existingProfile.Success && existingProfile.Data != null && existingProfile.Data.HasProfileImage;

        if (!hasProfileImage && (request.ProfileImageFile == null || request.ProfileImageFile.Length == 0))
        {
            ModelState.AddModelError("", "Please upload a profile image.");
            return View(request);
        }

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
    public async Task<IActionResult> ApplicationDetail(int id)
    {
        var token = GetToken();

        var appsResult = await _applicationService.GetMyApplications(token);
        var application = appsResult.Data?.FirstOrDefault(a => a.ApplicationId == id);

        if (application == null)
        {
            TempData["ErrorMessage"] = appsResult.Success
                ? "Application not found."
                : (appsResult.ErrorMessage ?? "Could not load application.");
            return RedirectToAction("MyApplications");
        }

        var profileResult = await _profileService.GetProfile(token);
        var resumeResult = await _resumeService.GetCurrentResume(token);

        var model = new ApplicationDetailViewModel
        {
            Application = application,
            Profile = profileResult.Success ? profileResult.Data : null,
            Resume = resumeResult.Success ? resumeResult.Data : null
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadResume(int id)
    {
        var result = await _resumeService.DownloadResume(GetToken(), id);

        if (!result.Success || result.Content == null)
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Could not download CV file.";
            return RedirectToAction("MyApplications");
        }

        return File(result.Content, result.ContentType ?? "application/octet-stream", result.FileName ?? "resume");
    }


    [HttpGet]
    public async Task<IActionResult> ProfileImage()
    {
        var result = await _profileService.GetProfileImage(GetToken());

        if (!result.Success || result.Content == null)
            return NotFound();

        return File(result.Content, result.ContentType ?? "application/octet-stream");
    }

    
    [HttpPost]
    public async Task<IActionResult> Apply(int positionId)
    {
        var result = await _applicationService.Apply(GetToken(), positionId);

        if (!result.Success)
        {
            var errorMessage = result.ErrorMessage ?? "Could not submit your application. Please try again.";

            if (result.RequiresProfileCompletion)
            {
                var updateResumeUrl = Url.Action("UpdateResume", "Candidate");
                errorMessage += $" <a href=\"{updateResumeUrl}\" class=\"alert-link\">Update Profile Now</a>";
                TempData["ErrorMessageHtml"] = true;
            }

            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction("Careers", "Page");
        }

        TempData["SuccessMessage"] = "Your application has been submitted successfully!";
        return RedirectToAction("Careers", "Page");
    }

    private string GetToken() => User.FindFirst("JwtToken")?.Value ?? string.Empty;
}
