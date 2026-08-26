using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;
using Pharmaceutical.Services;

namespace Pharmaceutical.Controllers
{
    // CV management for the Admin Portal. Role check relies on the "Role"
    // claim minted at login (AuthController.Login), so only accounts with
    // UserAccount.Role == "Admin" can reach any action here.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly PharmaceuticalDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public AdminController(PharmaceuticalDbContext db, IWebHostEnvironment env, IEmailService emailService)
        {
            _db = db;
            _env = env;
            _emailService = emailService;
        }

        private int? GetAdminUserId()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sub, out var id) ? id : null;
        }

        // One entry per position, with how many applications it has. Backs the
        // Admin Portal "Applications" page's position filter buttons.
        [HttpGet("positions")]
        public async Task<IActionResult> GetPositions()
        {
            var positions = await _db.Positions
                .OrderBy(p => p.Title)
                .Select(p => new AdminPositionSummaryResponse
                {
                    PositionId = p.PositionId,
                    Title = p.Title,
                    Department = p.Department,
                    ApplicationCount = _db.Applications.Count(a => a.PositionId == p.PositionId)
                })
                .ToListAsync();

            return Ok(positions);
        }

        // The candidates who applied to a given position.
        [HttpGet("positions/{id:int}/applications")]
        public async Task<IActionResult> GetPositionApplications(int id)
        {
            var positionExists = await _db.Positions.AnyAsync(p => p.PositionId == id);
            if (!positionExists)
                return NotFound(new { message = "Position not found" });

            var result = await BuildApplicationItems(_db.Applications.Where(a => a.PositionId == id));
            return Ok(result);
        }

        // All applications across every position, with optional filters.
        // Backs the Admin Portal "Applications" page (position quick-filter,
        // search box, and status dropdown).
        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications([FromQuery] int? positionId, [FromQuery] string? status, [FromQuery] string? keyword)
        {
            var query = _db.Applications.AsQueryable();

            if (positionId.HasValue)
                query = query.Where(a => a.PositionId == positionId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            var result = await BuildApplicationItems(query, keyword);
            return Ok(result);
        }

        // Shared projection logic for both the per-position and the all-applications
        // endpoints above, so they stay consistent.
        private async Task<List<AdminPositionApplicationItem>> BuildApplicationItems(
            IQueryable<Application> applicationsQuery, string? keyword = null)
        {
            var applications = await applicationsQuery
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            var candidateIds = applications.Select(a => a.CandidateId).Distinct().ToList();
            var positionIds = applications.Select(a => a.PositionId).Distinct().ToList();

            var profiles = await _db.CandidateProfiles
                .Where(p => candidateIds.Contains(p.CandidateId))
                .ToListAsync();

            var users = await _db.UserAccounts
                .Where(u => candidateIds.Contains(u.UserId))
                .ToListAsync();

            var positions = await _db.Positions
                .Where(p => positionIds.Contains(p.PositionId))
                .ToListAsync();

            var withResume = await _db.ResumeFiles
                .Where(r => r.CandidateId != null && candidateIds.Contains(r.CandidateId.Value) && r.IsCurrent == true)
                .Select(r => r.CandidateId!.Value)
                .Distinct()
                .ToListAsync();

            var result = applications.Select(a =>
            {
                var profile = profiles.FirstOrDefault(p => p.CandidateId == a.CandidateId);
                var user = users.FirstOrDefault(u => u.UserId == a.CandidateId);
                var position = positions.FirstOrDefault(p => p.PositionId == a.PositionId);

                return new AdminPositionApplicationItem
                {
                    ApplicationId = a.ApplicationId,
                    CandidateId = a.CandidateId,
                    FullName = profile?.FullName,
                    Email = user?.Email ?? "",
                    Phone = profile?.Phone,
                    AccountStatus = user?.Status,
                    AppliedDate = a.AppliedDate,
                    ApplicationStatus = a.Status,
                    HasResume = withResume.Contains(a.CandidateId),
                    PositionId = a.PositionId,
                    PositionTitle = position?.Title
                };
            });

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(r =>
                    (r.FullName != null && r.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    r.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return result.ToList();
        }

        [HttpGet("candidates")]
        public async Task<IActionResult> GetCandidates()
        {
            var candidates = await _db.UserAccounts
                .Where(u => u.Role == "Candidate")
                .Select(u => new AdminCandidateListItem
                {
                    CandidateId = u.UserId,
                    Email = u.Email!,
                    Status = u.Status
                })
                .ToListAsync();

            var candidateIds = candidates.Select(c => c.CandidateId).ToList();

            var profiles = await _db.CandidateProfiles
                .Where(p => candidateIds.Contains(p.CandidateId))
                .ToListAsync();

            var withResume = await _db.ResumeFiles
                .Where(r => r.CandidateId != null && candidateIds.Contains(r.CandidateId.Value) && r.IsCurrent == true)
                .Select(r => r.CandidateId!.Value)
                .Distinct()
                .ToListAsync();

            foreach (var c in candidates)
            {
                var profile = profiles.FirstOrDefault(p => p.CandidateId == c.CandidateId);
                if (profile != null)
                {
                    c.FullName = profile.FullName;
                    c.Phone = profile.Phone;
                    c.CreatedAt = profile.CreatedAt;
                }

                c.HasResume = withResume.Contains(c.CandidateId);
            }

            return Ok(candidates.OrderByDescending(c => c.CreatedAt).ToList());
        }

        // CV detail — Admin only. When positionId is supplied (i.e. this
        // candidate is being viewed from a specific position's applications
        // list), the Interview Invitation History is scoped to just that
        // Application so invitations sent for a different position the same
        // candidate applied to don't show up here.
        [HttpGet("candidates/{id:int}")]
        public async Task<IActionResult> GetCandidateDetail(int id, [FromQuery] int? positionId)
        {
            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Candidate");
            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == id);

            if (user == null || profile == null)
                return NotFound(new { message = "Candidate not found" });

            var invitationsQuery = _db.InterviewInvitations.Where(i => i.CandidateId == id);

            Application? scopedApplication = null;
            Position? scopedPosition = null;

            if (positionId.HasValue)
            {
                scopedApplication = await _db.Applications
                    .FirstOrDefaultAsync(a => a.CandidateId == id && a.PositionId == positionId.Value);

                if (scopedApplication != null)
                {
                    scopedPosition = await _db.Positions
                        .FirstOrDefaultAsync(p => p.PositionId == scopedApplication.PositionId);
                }

                // Scope to this one application's invitations. Older invitations
                // sent before ApplicationId was tracked (application_id == null)
                // are still included when they belong to this candidate's only/most
                // recent application context isn't determinable, so they're left
                // out here — they'll still show up when viewing without a
                // positionId (e.g. from the candidate list).
                invitationsQuery = scopedApplication != null
                    ? invitationsQuery.Where(i => i.ApplicationId == scopedApplication.ApplicationId)
                    : invitationsQuery.Where(i => false);
            }

            // When viewing a specific application, show the candidate's profile
            // exactly as it was AT THE MOMENT they applied (ApplicationLog),
            // not their current, possibly since-edited, live profile. Older
            // applications made before ApplicationLog existed have no log row,
            // so fall back to the live profile for those.
            var log = scopedApplication != null
                ? await _db.ApplicationLogs.FirstOrDefaultAsync(l => l.ApplicationId == scopedApplication.ApplicationId)
                : null;

            string? fullName, phone, address, summary, profileImageKey;
            List<EducationItemDto> educations;
            List<WorkExperienceItemDto> workExperiences;
            ResumeFile? resume;

            if (log != null)
            {
                fullName = log.FullName;
                phone = log.Phone;
                address = log.Address;
                summary = log.Summary;
                profileImageKey = log.ProfileImage;

                educations = string.IsNullOrEmpty(log.EducationsJson)
                    ? new List<EducationItemDto>()
                    : (JsonSerializer.Deserialize<List<EducationSnapshotDto>>(log.EducationsJson) ?? new())
                        .Select(e => new EducationItemDto
                        {
                            Institution = e.Institution,
                            Qualification = e.Qualification,
                            Field = e.Field,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate
                        })
                        .ToList();

                workExperiences = string.IsNullOrEmpty(log.WorkExperiencesJson)
                    ? new List<WorkExperienceItemDto>()
                    : (JsonSerializer.Deserialize<List<WorkExperienceSnapshotDto>>(log.WorkExperiencesJson) ?? new())
                        .Select(w => new WorkExperienceItemDto
                        {
                            Employer = w.Employer,
                            Title = w.Title,
                            StartDate = w.StartDate,
                            EndDate = w.EndDate,
                            Description = w.Description
                        })
                        .ToList();

                // Resolve by StorageKey back to the real ResumeFile row (old
                // resumes are kept, just flagged IsCurrent = false) so the
                // "View"/"Download" buttons open the exact file the candidate
                // had on file at apply time, even if they've since replaced it.
                resume = string.IsNullOrEmpty(log.ResumeStorageKey)
                    ? null
                    : await _db.ResumeFiles.FirstOrDefaultAsync(r => r.CandidateId == id && r.StorageKey == log.ResumeStorageKey);
            }
            else
            {
                fullName = profile.FullName;
                phone = profile.Phone;
                address = profile.Address;
                summary = profile.Summary;
                profileImageKey = profile.ProfileImage;

                educations = await _db.EducationRecords
                    .Where(e => e.CandidateId == id)
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

                workExperiences = await _db.WorkExperiences
                    .Where(w => w.CandidateId == id)
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

                resume = await _db.ResumeFiles
                    .Where(r => r.CandidateId == id && r.IsCurrent == true)
                    .OrderByDescending(r => r.UploadedAt)
                    .FirstOrDefaultAsync();
            }

            var invitations = await invitationsQuery
                .OrderByDescending(i => i.SentAt)
                .Select(i => new InterviewInvitationResponse
                {
                    InvitationId = i.InvitationId,
                    CandidateId = i.CandidateId,
                    Subject = i.Subject,
                    Type = i.Type,
                    Status = i.Status,
                    SentAt = i.SentAt
                })
                .ToListAsync();

            return Ok(new AdminCandidateDetailResponse
            {
                CandidateId = id,
                Email = user.Email!,
                Status = user.Status,
                FullName = fullName,
                Phone = phone,
                Address = address,
                Summary = summary,
                CreatedAt = profile.CreatedAt,
                HasProfileImage = !string.IsNullOrEmpty(profileImageKey),
                Educations = educations,
                WorkExperiences = workExperiences,
                Resume = resume == null ? null : new ResumeResponse
                {
                    ResumeId = resume.ResumeId,
                    OriginalName = resume.OriginalName ?? resume.StorageKey,
                    MimeType = resume.MimeType,
                    Size = resume.Size,
                    UploadedAt = resume.UploadedAt,
                    IsCurrent = resume.IsCurrent ?? false
                },
                Invitations = invitations,
                ApplicationId = scopedApplication?.ApplicationId,
                PositionId = scopedApplication?.PositionId,
                PositionTitle = scopedPosition?.Title,
                Department = scopedPosition?.Department,
                AppliedDate = scopedApplication?.AppliedDate,
                ApplicationStatus = scopedApplication?.Status
            });
        }

        // resumeId is optional: when the candidate's CandidateDetail page is
        // scoped to a specific application, the "View"/"Download" buttons pass
        // the exact resume that was on file at apply time (per ApplicationLog),
        // which may no longer be the candidate's current resume. Without it,
        // this falls back to whatever the candidate's current resume is.
        [HttpGet("candidates/{id:int}/resume/download")]
        public async Task<IActionResult> DownloadCandidateResume(int id, [FromQuery] int? resumeId)
        {
            var resume = resumeId.HasValue
                ? await _db.ResumeFiles.FirstOrDefaultAsync(r => r.CandidateId == id && r.ResumeId == resumeId.Value)
                : await _db.ResumeFiles
                    .Where(r => r.CandidateId == id && r.IsCurrent == true)
                    .OrderByDescending(r => r.UploadedAt)
                    .FirstOrDefaultAsync();

            if (resume == null)
                return NotFound(new { message = "No resume on file for this candidate" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "Resumes");
            var filePath = Path.Combine(storageRoot, resume.StorageKey);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File is missing from storage" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, resume.MimeType ?? "application/octet-stream", resume.OriginalName ?? resume.StorageKey);
        }

        // Same file as above, but without a filename on the response — no
        // "attachment" Content-Disposition gets set, so the browser renders
        // it inline (e.g. PDFs open in the browser's built-in viewer) instead
        // of forcing a download. Used by the Admin Portal's "View CV" button.
        [HttpGet("candidates/{id:int}/resume/view")]
        public async Task<IActionResult> ViewCandidateResume(int id, [FromQuery] int? resumeId)
        {
            var resume = resumeId.HasValue
                ? await _db.ResumeFiles.FirstOrDefaultAsync(r => r.CandidateId == id && r.ResumeId == resumeId.Value)
                : await _db.ResumeFiles
                    .Where(r => r.CandidateId == id && r.IsCurrent == true)
                    .OrderByDescending(r => r.UploadedAt)
                    .FirstOrDefaultAsync();

            if (resume == null)
                return NotFound(new { message = "No resume on file for this candidate" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "Resumes");
            var filePath = Path.Combine(storageRoot, resume.StorageKey);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "File is missing from storage" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, resume.MimeType ?? "application/octet-stream");
        }

        // applicationId is optional: when CandidateDetail is scoped to a
        // specific application, this serves the photo that was on file at
        // apply time (ApplicationLog), which may differ from the candidate's
        // current photo. Without it, falls back to the candidate's live photo.
        [HttpGet("candidates/{id:int}/profile-image")]
        public async Task<IActionResult> GetCandidateProfileImage(int id, [FromQuery] int? applicationId)
        {
            string? storageKey = null;

            if (applicationId.HasValue)
            {
                var log = await _db.ApplicationLogs
                    .FirstOrDefaultAsync(l => l.ApplicationId == applicationId.Value);
                storageKey = log?.ProfileImage;
            }

            if (string.IsNullOrEmpty(storageKey))
            {
                var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == id);
                storageKey = profile?.ProfileImage;
            }

            if (string.IsNullOrEmpty(storageKey))
                return NotFound(new { message = "No profile image on file" });

            var storageRoot = Path.Combine(_env.ContentRootPath, "Storage", "ProfileImages");
            var filePath = Path.Combine(storageRoot, storageKey);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Image is missing from storage" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = Path.GetExtension(storageKey).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
            return File(bytes, contentType);
        }

        // Sends the recruitment/interview email and records it as an
        // InterviewInvitation (D10), regardless of whether the send succeeded,
        // so there is always an audit trail of what was attempted.
        [HttpPost("candidates/{id:int}/invite")]
        public async Task<IActionResult> SendInvitation(int id, [FromQuery] int? positionId, SendInterviewInvitationRequest req)
        {
            var adminId = GetAdminUserId();
            if (adminId == null)
                return Unauthorized();

            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Candidate");
            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == id);

            if (user == null || profile == null)
                return NotFound(new { message = "Candidate not found" });

            // Type picks which tab this was sent from and, in turn, which
            // template is used when Subject/Body are left blank, and which
            // status the application moves to on a successful send.
            var type = NormalizeInvitationType(req.Type);

            var subject = string.IsNullOrWhiteSpace(req.Subject) ? DefaultSubjectFor(type) : req.Subject;
            var body = string.IsNullOrWhiteSpace(req.Body) ? DefaultBodyFor(type, profile.FullName) : req.Body;

            // Resolve the specific application this invitation is being sent for
            // (when known), so the invitation record can be scoped to it — a
            // candidate who applied to several positions shouldn't have an
            // invitation for one position show up in another position's history.
            Application? application = null;
            if (positionId.HasValue)
            {
                application = await _db.Applications
                    .FirstOrDefaultAsync(a => a.CandidateId == id && a.PositionId == positionId.Value);
            }

            var sent = await _emailService.SendEmailAsync(user.Email!, subject, body);

            var invitation = new InterviewInvitation
            {
                CandidateId = id,
                ApplicationId = application?.ApplicationId,
                SentBy = adminId,
                Subject = subject,
                Type = type,
                Body = body,
                Status = sent ? "Sent" : "Failed",
                SentAt = DateTime.UtcNow
            };

            _db.InterviewInvitations.Add(invitation);

            // On a successful send, move the specific application this email
            // was sent from to the status matching the most recent email type,
            // so the Applications page always reflects the latest email sent.
            if (sent && application != null)
                application.Status = ApplicationStatusFor(type);

            await _db.SaveChangesAsync();

            var response = new InterviewInvitationResponse
            {
                InvitationId = invitation.InvitationId,
                CandidateId = id,
                Subject = invitation.Subject,
                Type = invitation.Type,
                Status = invitation.Status,
                SentAt = invitation.SentAt
            };

            if (!sent)
                return StatusCode(502, response); // recorded, but the SMTP send failed

            return Ok(response);
        }

        private static readonly string[] KnownInvitationTypes = { "Interview", "Offer", "Decline" };

        private static string NormalizeInvitationType(string? type) =>
            KnownInvitationTypes.FirstOrDefault(t => string.Equals(t, type, StringComparison.OrdinalIgnoreCase))
            ?? "Interview";

        // The Application.Status value that a successful send of this email
        // type moves the application to.
        private static string ApplicationStatusFor(string type) => type switch
        {
            "Offer" => "Offer",
            "Decline" => "Decline",
            _ => "Sent"
        };

        private static string DefaultSubjectFor(string type) => type switch
        {
            "Offer" => "Job Offer",
            "Decline" => "Application Update",
            _ => "Interview Invitation"
        };

        private static string DefaultBodyFor(string type, string? fullName)
        {
            var name = string.IsNullOrWhiteSpace(fullName) ? "Candidate" : fullName;
            return type switch
            {
                "Offer" =>
                    $"<p>Dear {name},</p>" +
                    "<p>Congratulations! We are pleased to offer you the position. Our recruitment team will reach out " +
                    "shortly with the offer details and next steps.</p>" +
                    "<p>Best regards,<br/>Recruitment Team</p>",
                "Decline" =>
                    $"<p>Dear {name},</p>" +
                    "<p>Thank you for taking the time to apply and for your interest in joining our team. After careful " +
                    "consideration, we have decided not to move forward with your application at this time.</p>" +
                    "<p>We wish you the best in your future endeavors.</p>" +
                    "<p>Best regards,<br/>Recruitment Team</p>",
                _ =>
                    $"<p>Dear {name},</p>" +
                    "<p>We were impressed with your application and would like to invite you for an interview. " +
                    "Our recruitment team will contact you shortly to arrange a time.</p>" +
                    "<p>Best regards,<br/>Recruitment Team</p>"
            };
        }
    }
}
