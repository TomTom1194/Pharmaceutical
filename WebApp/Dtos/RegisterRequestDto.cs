using System.ComponentModel.DataAnnotations;

namespace WebApp.Dtos;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Input Email")]
    [EmailAddress(ErrorMessage = "Email is not correct type")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Input Password")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm your password")]
    [Compare(nameof(Password), ErrorMessage = "Password confirmation does not match")]
    public string ConfirmPassword { get; set; }
}
