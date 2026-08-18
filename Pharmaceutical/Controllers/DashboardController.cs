using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;

namespace Pharmaceutical.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly PharmaceuticalDbContext _db;

    public DashboardController(PharmaceuticalDbContext db) => _db = db;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        
        var pendingQuotes = await _db.QuoteRequests
            .CountAsync(q => q.Status == null || q.Status == "New" || q.Status == "Pending");

        
        var newApplicants = await _db.Applications
            .CountAsync(a => a.Status == "Applied");

        
        var recentQuotes = await _db.QuoteRequests
            .Where(q => q.Status == "Pending")
            .OrderByDescending(q => q.SubmittedAt)
            .Take(5)
            .Select(q => new RecentQuoteDto
            {
                QuoteId = q.QuoteId,
                FullName = q.FullName,
                CompanyName = q.CompanyName,
                Status = q.Status ?? "Pending",
                SubmittedAt = q.SubmittedAt
            })
            .ToListAsync();

        
        var recentApplicants = await _db.Applications
            .Where(a => a.Status == "Applied")
            .OrderByDescending(a => a.AppliedDate)
            .Take(5)
            .Join(_db.CandidateProfiles,
                a => a.CandidateId,
                c => c.CandidateId,
                (a, c) => new { a, c })
            .Join(_db.Positions,
                ac => ac.a.PositionId,
                p => p.PositionId,
                (ac, p) => new RecentApplicantDto
                {
                    InvitationId = ac.a.ApplicationId,
                    CandidateName = ac.c.FullName,
                    Subject = p.Title,
                    Status = ac.a.Status,
                    SentAt = ac.a.AppliedDate
                })
            .ToListAsync();

        return Ok(new DashboardStatsDto
        {
            PendingQuotesCount = pendingQuotes,
            NewApplicantsCount = newApplicants,
            RecentQuotes = recentQuotes,
            RecentApplicants = recentApplicants
        });
    }
}
