using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmaceutical.Models;

[Table("UserAccount")]
public class UserAccount
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required, MaxLength(255)]
    [Column("email")]
    public string? Email { get; set; } 

    [Required, MaxLength(255)]
    [Column("password_hash")]
    public string? PasswordHash { get; set; } 

    [Required, MaxLength(20)]
    [Column("role")]
    public string? Role { get; set; } 

    [Required, MaxLength(20)]
    [Column("status")]
    public string? Status { get; set; } 

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }
}