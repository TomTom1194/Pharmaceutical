using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = null!;
}
