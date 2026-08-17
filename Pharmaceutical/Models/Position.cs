using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("Positions")]
public class Position
{
    [Key]
    [Column("PositionId")]
    public int PositionId { get; set; }

    [Required, MaxLength(255)]
    [Column("Title")]
    public string Title { get; set; } = null!;

    [Required, MaxLength(100)]
    [Column("Department")]
    public string Department { get; set; } = null!;

    [Required, MaxLength(50)]
    [Column("Type")]
    public string Type { get; set; } = null!;

    [MaxLength(100)]
    [Column("SalaryRange")]
    public string? SalaryRange { get; set; }

    [Required]
    [Column("Description")]
    public string Description { get; set; } = null!;

    [Required]
    [Column("Requirements")]
    public string Requirements { get; set; } = null!;

    // DB default is 1 (Active); still set explicitly on insert from C# to avoid
    // relying on the server-side default when EF sends an explicit value.
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    // DB default is GETUTCDATE(); set explicitly on insert for the same reason.
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
