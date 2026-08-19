using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        // CV detail — Admin only.
        [HttpGet("candidates/{id:int}")]
        public async Task<IActionResult> GetCandidateDetail(int id)
        {
            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Candidate");
            var profile = await _db.CandidateProfiles.FirstOrDefaultAsync(c => c.CandidateId == id);

            if (user == null || profile == null)
                return NotFound(new { message = "Candidate not found" });

            var educations = await _db.EducationRecords
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

            var workExperiences = await _db.WorkExperiences
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

            var resume = await _db.ResumeFiles
                .Where(r => r.CandidateId == id && r.IsCurrent == true)
                .OrderByDescending(r => r.UploadedAt)
                .FirstOrDefaultAsync();

            var invitations = await _db.InterviewInvitations
                .Where(i => i.CandidateId == id)
                .OrderByDescending(i => i.SentAt)
                .Select(i => new InterviewInvitationResponse
                {
                    InvitationId = i.InvitationId,
                    CandidateId = i.CandidateId,
                    Subject = i.Subject,
                    Status = i.Status,
                    SentAt = i.SentAt
                })
                .ToListAsync();

            return Ok(new AdminCandidateDetailResponse
            {
                CandidateId = id,
                Email = user.Email!,
                Status = user.Status,
                FullName = profile.FullName,
                Phone = profile.Phone,
                Address = profile.Address,
                Summary = profile.Summary,
                CreatedAt = profile.CreatedAt,
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
                Invitations = invitations
            });
        }

        [HttpGet("candidates/{id:int}/resume/download")]
        public async Task<IActionResult> DownloadCandidateResume(int id)
        {
            var resume = await _db.ResumeFiles
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
        public async Task<IActionResult> ViewCandidateResume(int id)
        {
            var resume = await _db.ResumeFiles
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

            var subject = string.IsNullOrWhiteSpace(req.Subject) ? "Interview Invitation" : req.Subject;
            var body = string.IsNullOrWhiteSpace(req.Body) ? BuildDefaultInvitationBody(profile.FullName) : req.Body;

            var sent = await _emailService.SendEmailAsync(user.Email!, subject, body);

            var invitation = new InterviewInvitation
            {
                CandidateId = id,
                SentBy = adminId,
                Subject = subject,
                Body = body,
                Status = sent ? "Sent" : "Failed",
                SentAt = DateTime.UtcNow
            };

            _db.InterviewInvitations.Add(invitation);

            // On a successful send, mark the specific application this invitation
            // was sent from as "Sent" so the Applications page reflects it.
            if (sent && positionId.HasValue)
            {
                var application = await _db.Applications
                    .FirstOrDefaultAsync(a => a.CandidateId == id && a.PositionId == positionId.Value);

                if (application != null)
                    application.Status = "Sent";
            }

            await _db.SaveChangesAsync();

            var response = new InterviewInvitationResponse
            {
                InvitationId = invitation.InvitationId,
                CandidateId = id,
                Subject = invitation.Subject,
                Status = invitation.Status,
                SentAt = invitation.SentAt
            };

            if (!sent)
                return StatusCode(502, response); // recorded, but the SMTP send failed

            return Ok(response);
        }

        private static string BuildDefaultInvitationBody(string? fullName)
        {
            var name = string.IsNullOrWhiteSpace(fullName) ? "Candidate" : fullName;
            return $"<p>Dear {name},</p>" +
                   "<p>We were impressed with your application and would like to invite you for an interview. " +
                   "Our recruitment team will contact you shortly to arrange a time.</p>" +
                   "<p>Best regards,<br/>Recruitment Team</p>";
        }
    }
}
