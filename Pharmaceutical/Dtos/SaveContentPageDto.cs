using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

public class SaveContentPageDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Body { get; set; }

    public string? BannerImageUrl { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }
}
