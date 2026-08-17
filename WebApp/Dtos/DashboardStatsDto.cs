namespace WebApp.Dtos;

public class DashboardStatsDto
{
    public int PendingQuotesCount { get; set; }
    public int NewApplicantsCount { get; set; }
    public List<RecentQuoteDto> RecentQuotes { get; set; } = new();
    public List<RecentApplicantDto> RecentApplicants { get; set; } = new();
}

public class RecentQuoteDto
{
    public int QuoteId { get; set; }
    public string FullName { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class RecentApplicantDto
{
    public int InvitationId { get; set; }
    public string? CandidateName { get; set; }
    public string? Subject { get; set; }
    public string? Status { get; set; }
    public DateTime? SentAt { get; set; }
}
