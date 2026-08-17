using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ContentPageFormModel
{
    public int PageId { get; set; }
    public string Slug { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    public string? BannerImageUrl { get; set; }
    
    public IFormFile? BannerImageUpload { get; set; }

    // Structured fields for Homepage
    public string? HeroTitle { get; set; }
    public string? HeroSubtitle { get; set; }
    public string? AboutTitle { get; set; }
    public string? AboutDescription { get; set; }
    public string? QuoteEmail { get; set; }
    public string? QuotePhone { get; set; }

    public List<IFormFile>? SliderImageUploads { get; set; }
    public List<string> ExistingSliderImages { get; set; } = new();

    // For Careers Page
    public List<JobOpeningModel> JobOpenings { get; set; } = new();

    // For About Us Page
    public List<CoreValueModel> CoreValues { get; set; } = new();

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Published";
}
