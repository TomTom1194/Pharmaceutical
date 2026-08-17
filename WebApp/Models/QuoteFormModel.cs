using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class QuoteFormModel
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(255)]
    public string FullName { get; set; } = null!;

    [MaxLength(255)]
    public string? CompanyName { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Comments { get; set; }
}
