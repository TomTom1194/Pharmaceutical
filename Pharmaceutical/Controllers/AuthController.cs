using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;
using Pharmaceutical.Models;

namespace Pharmaceutical.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PharmaceuticalDbContext _db;
        readonly IConfiguration _config;

        public AuthController(PharmaceuticalDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var emailExists = await _db.UserAccounts
                .AnyAsync(u => u.Email == req.Email);

            if (emailExists)
                return Conflict(new { message = "Email already registered" });

            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = new UserAccount
                {
                    Email = req.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                    Role = "Candidate",
                    Status = "Active"
                };

                _db.UserAccounts.Add(user);
                await _db.SaveChangesAsync(); // generates UserId (identity)

                // Bare-minimum profile shell; the candidate fills in the rest
                // later via the "Create Resume" flow in the candidate portal.
                var profile = new CandidateProfile
                {
                    CandidateId = user.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                _db.CandidateProfiles.Add(profile);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(Register), new RegisterResponse
                {
                    UserId = user.UserId,
                    Email = user.Email!,
                    Role = user.Role!
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {

            var user = await _db.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
                return Unauthorized(new { message = "Email or Password incorrect" });


            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized(new { message = "Email or Password incorrect" });


            if (user.Status != "Active")
                return Unauthorized(new { message = "Account now is inactive" });


            var expiresMinutes = int.Parse(_config["Jwt:ExpiresInMinutes"]!);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);


            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();


            return Ok(new LoginResponse
            {
                Token = tokenString,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = expiresAt
            });

        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest req)
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(sub, out var userId))
                return Unauthorized(new { message = "Invalid token" });

            var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return Unauthorized(new { message = "Invalid token" });

            bool isCurrentValid = BCrypt.Net.BCrypt.Verify(req.CurrentPassword, user.PasswordHash);
            if (!isCurrentValid)
                return BadRequest(new { message = "Current password is incorrect" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }
    }
}
