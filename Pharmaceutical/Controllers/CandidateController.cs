using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;

namespace Pharmaceutical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidateController : ControllerBase
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx" };

        private static readonly string[] AllowedMimeTypes =
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        private static readonly string[] AllowedImageMimeTypes =
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/gif"
        };

        private const long MaxImageSizeBytes = 2 * 1024 * 1024; // 2 MB

        private readonly PharmaceuticalDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CandidateController(PharmaceuticalDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Candidate accounts share their primary key with UserAccount (see AuthController.Register),
        // so the "sub" claim in the JWT is also the CandidateProfile.CandidateId.
        private int? GetCandidateId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : null;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == candidateId);
            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == candidateId);

            if (user == null || profile == null)
                return NotFound(new { message = "Candidate profile not found" });

            var educations = await _db.EducationRecords
                .Where(e => e.CandidateId == candidateId)
                .OrderByDescending(e => e.StartDate)
                .Select(e => new EducationItemDto
                {
                    EducationId = e.EducationId,
                    Institution = e.Institution,
                    Qualification = e.Qualification,
                    Field = e.Field,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .ToListAsync();

            var workExperiences = await _db.WorkExperiences
                .Where(w => w.CandidateId == candidateId)
                .OrderByDescending(w => w.StartDate)
                .Select(w => new WorkExperienceItemDto
                {
                    ExperienceId = w.ExperienceId,
                    Employer = w.Employer,
                    Title = w.Title,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    Description = w.Description
                })
                .ToListAsync();

            return Ok(new CandidateProfileResponse
            {
                CandidateId = profile.CandidateId,
                Email = user.Email!,
                FullName = profile.FullName,
                Phone = profile.Phone,
                Address = profile.Address,
                Summary = profile.Summary,
                CreatedAt = profile.CreatedAt,
                HasProfileImage = !string.IsNullOrEmpty(profile.ProfileImage),
                Educations = educations,
                WorkExperiences = workExperiences
            });
        }

        // "Create Resume" in the candidate portal: fills in personal info and
        // replaces the candidate's education / work-experience entries in one shot.
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(CandidateProfileUpdateRequest req)
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == candidateId);
            if (profile == null)
                return NotFound(new { message = "Candidate profile not found" });

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                profile.FullName = req.FullName;
                profile.Phone = req.Phone;
                profile.Address = req.Address;
                profile.Summary = req.Summary;

                var oldEducations = await _db.EducationRecords
                    .Where(e => e.CandidateId == candidateId)
                    .ToListAsync();
                _db.EducationRecords.RemoveRange(oldEducations);

                var oldWorkExperiences = await _db.WorkExperiences
                    .Where(w => w.CandidateId == candidateId)
                    .ToListAsync();
                _db.WorkExperiences.RemoveRange(oldWorkExperiences);

                foreach (var edu in req.Educations.Where(e => !IsEmpty(e)))
                {
                    _db.EducationRecords.Add(new EducationRecord
                    {
                        CandidateId = candidateId,
                        Institution = edu.Institution,
                        Qualification = edu.Qualification,
                        Field = edu.Field,
                        StartDate = edu.StartDate,
                        EndDate = edu.EndDate
                    });
                }

                foreach (var work in req.WorkExperiences.Where(w => !IsEmpty(w)))
                {
                    _db.WorkExperiences.Add(new WorkExperience
                    {
                        CandidateId = candidateId,
                        Employer = work.Employer,
                        Title = work.Title,
                        StartDate = work.StartDate,
                        EndDate = work.EndDate,
                        Description = work.Description
                    });
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Profile updated" });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Skip rows the candidate left completely blank in the form.
        private static bool IsEmpty(EducationInputDto e) =>
            string.IsNullOrWhiteSpace(e.Institution) &&
            string.IsNullOrWhiteSpace(e.Qualification) &&
            string.IsNullOrWhiteSpace(e.Field) &&
            e.StartDate == null &&
            e.EndDate == null;

        private static bool IsEmpty(WorkExperienceInputDto w) =>
            string.IsNullOrWhiteSpace(w.Employer) &&
            string.IsNullOrWhiteSpace(w.Title) &&
            string.IsNullOrWhiteSpace(w.Description) &&
            w.StartDate == null &&
            w.EndDate == null;

        [HttpPost("profile-image")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            if (profileImage == null || profileImage.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            if (profileImage.Length > MaxImageSizeBytes)
                return BadRequest(new { message = "Image exceeds the 2 MB limit" });

            var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension) || !AllowedImageMimeTypes.Contains(profileImage.ContentType))
                return BadRequest(new { message = "Only JPG, PNG, WEBP and GIF images are allowed" });

            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == candidateId);
            if (profile == null)
                return NotFound(new { message = "Candidate profile not found" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "ProfileImages");
            Directory.CreateDirectory(storageRoot);

            var storageKey = $"{candidateId}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(storageRoot, storageKey);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            // Remove the previous image file so old photos don't pile up in storage.
            var oldStorageKey = profile.ProfileImage;
            profile.ProfileImage = storageKey;
            await _db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldStorageKey))
            {
                var oldFilePath = Path.Combine(storageRoot, oldStorageKey);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            return Ok(new { message = "Profile image updated" });
        }

        [HttpGet("profile-image")]
        public async Task<IActionResult> GetProfileImage()
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == candidateId);
            if (profile == null || string.IsNullOrEmpty(profile.ProfileImage))
                return NotFound(new { message = "No profile image on file" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "ProfileImages");
            var filePath = Path.Combine(storageRoot, profile.ProfileImage);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Image is missing from storage" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, GetImageContentType(profile.ProfileImage));
        }

        private static string GetImageContentType(string fileName) =>
            Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

        // Candidate applies to an open position from the Careers page.
        [HttpPost("positions/{positionId:int}/apply")]
        public async Task<IActionResult> ApplyToPosition(int positionId)
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var candidateExists = await _db.CandidateProfiles.AnyAsync(c => c.CandidateId == candidateId);
            if (!candidateExists)
                return NotFound(new { message = "Candidate profile not found" });

            var position = await _db.Positions.FirstOrDefaultAsync(p => p.PositionId == positionId);
            if (position == null)
                return NotFound(new { message = "Position not found" });

            if (!position.IsActive)
                return BadRequest(new { message = "This position is no longer accepting applications" });

            var alreadyApplied = await _db.Applications
                .AnyAsync(a => a.CandidateId == candidateId && a.PositionId == positionId);
            if (alreadyApplied)
                return Conflict(new { message = "You have already applied to this position" });

            var application = new Application
            {
                CandidateId = candidateId.Value,
                PositionId = positionId,
                AppliedDate = DateTime.UtcNow,
                Status = "Applied"
            };

            _db.Applications.Add(application);
            await _db.SaveChangesAsync();

            return Ok(new ApplicationResponse
            {
                ApplicationId = application.ApplicationId,
                PositionId = position.PositionId,
                PositionTitle = position.Title,
                Department = position.Department,
                AppliedDate = application.AppliedDate,
                Status = application.Status
            });
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications()
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var applications = await _db.Applications
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.AppliedDate)
                .Join(_db.Positions,
                    a => a.PositionId,
                    p => p.PositionId,
                    (a, p) => new ApplicationResponse
                    {
                        ApplicationId = a.ApplicationId,
                        PositionId = p.PositionId,
                        PositionTitle = p.Title,
                        Department = p.Department,
                        AppliedDate = a.AppliedDate,
                        Status = a.Status
                    })
                .ToListAsync();

            return Ok(applications);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var resume = await _db.ResumeFiles
                .Where(r => r.CandidateId == candidateId && r.IsCurrent == true)
                .OrderByDescending(r => r.UploadedAt)
                .FirstOrDefaultAsync();

            if (resume == null)
                return NotFound(new { message = "No resume uploaded yet" });

            return Ok(ToDto(resume));
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MaxFileSizeBytes + 1024)]
        public async Task<IActionResult> Upload(IFormFile resume)
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            if (resume == null || resume.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            if (resume.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File exceeds the 5 MB limit" });

            var extension = Path.GetExtension(resume.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || !AllowedMimeTypes.Contains(resume.ContentType))
                return BadRequest(new { message = "Only PDF, DOC and DOCX files are allowed" });

            var candidateExists = await _db.CandidateProfiles.AnyAsync(c => c.CandidateId == candidateId);
            if (!candidateExists)
                return NotFound(new { message = "Candidate profile not found" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "Resumes");
            Directory.CreateDirectory(storageRoot);

            var storageKey = $"{candidateId}_{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(storageRoot, storageKey);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await resume.CopyToAsync(stream);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Replace: any previously current resume for this candidate is superseded.
                var previous = await _db.ResumeFiles
                    .Where(r => r.CandidateId == candidateId && r.IsCurrent == true)
                    .ToListAsync();

                foreach (var old in previous)
                    old.IsCurrent = false;

                var record = new ResumeFile
                {
                    CandidateId = candidateId,
                    StorageKey = storageKey,
                    OriginalName = resume.FileName,
                    MimeType = resume.ContentType,
                    Size = (int)resume.Length,
                    UploadedAt = DateTime.UtcNow,
                    IsCurrent = true
                };

                _db.ResumeFiles.Add(record);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ToDto(record));
            }
            catch
            {
                await transaction.RollbackAsync();
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                throw;
            }
        }

        [HttpGet("download/{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var candidateId = GetCandidateId();
            if (candidateId == null)
                return Unauthorized();

            var resume = await _db.ResumeFiles
                .FirstOrDefaultAsync(r => r.ResumeId == id && r.CandidateId == candidateId);

            if (resume == null)
                return NotFound();

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "Resumes");
            var filePath = Path.Combine(storageRoot, resume.StorageKey);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File is missing from storage" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, resume.MimeType ?? "application/octet-stream", resume.OriginalName ?? resume.StorageKey);
        }

        private static ResumeResponse ToDto(ResumeFile r) => new()
        {
            ResumeId = r.ResumeId,
            OriginalName = r.OriginalName ?? r.StorageKey,
            MimeType = r.MimeType,
            Size = r.Size,
            UploadedAt = r.UploadedAt,
            IsCurrent = r.IsCurrent ?? false
        };
    }
}
