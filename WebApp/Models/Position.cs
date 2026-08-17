using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Models;

public class Position
{
    public int PositionId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;



    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(100)]
    public string SalaryRange { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Requirements { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}
