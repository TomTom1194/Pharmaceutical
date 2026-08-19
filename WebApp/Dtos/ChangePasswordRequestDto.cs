using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Input current password")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Input new password")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm your new password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Confirm password does not match")]
    public string ConfirmPassword { get; set; }
}

public class ChangePasswordResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
