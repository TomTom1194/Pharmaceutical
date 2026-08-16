using System.ComponentModel.DataAnnotations;

namespace Pharmaceutical.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
}
