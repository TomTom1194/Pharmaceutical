namespace Pharmaceutical.Dtos;

public class QuoteRequestDto
{
    public int QuoteId { get; set; }
    public string FullName { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Comments { get; set; }
    public string? Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
}
