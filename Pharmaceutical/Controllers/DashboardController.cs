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
        // Count quotes that are New or Pending (unresolved)
        var pendingQuotes = await _db.QuoteRequests
            .CountAsync(q => q.Status == null || q.Status == "New" || q.Status == "Pending");

        // Count candidates with Pending status (newly registered, not yet processed by Tân)
        var newApplicants = await _db.UserAccounts
            .CountAsync(u => u.Role == "Candidate" && u.Status == "Pending");

        // Recent 5 quote requests
        var recentQuotes = await _db.QuoteRequests
            .OrderByDescending(q => q.SubmittedAt)
            .Take(5)
            .Select(q => new RecentQuoteDto
            {
                QuoteId = q.QuoteId,
                FullName = q.FullName,
                CompanyName = q.CompanyName,
                Status = q.Status ?? "New",
                SubmittedAt = q.SubmittedAt
            })
            .ToListAsync();

        // Recent 5 candidates with Pending status (read-only, managed by Tân)
        var recentApplicants = await (
            from c in _db.CandidateProfiles
            join u in _db.UserAccounts on c.CandidateId equals u.UserId
            where u.Role == "Candidate" && u.Status == "Pending"
            orderby c.CreatedAt descending
            select new RecentApplicantDto
            {
                InvitationId = c.CandidateId,
                CandidateName = c.FullName,
                Subject = u.Email,
                Status = u.Status,
                SentAt = c.CreatedAt
            })
            .Take(5)
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
