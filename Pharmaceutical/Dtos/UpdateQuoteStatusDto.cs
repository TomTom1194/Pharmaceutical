using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

public class UpdateQuoteStatusDto
{
    [Required, MaxLength(20)]
    public string Status { get; set; } = null!;
}
