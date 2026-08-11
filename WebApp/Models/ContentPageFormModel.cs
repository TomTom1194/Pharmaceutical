using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ContentPageFormModel
{
    public int PageId { get; set; }
    public string Slug { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Published";
}
