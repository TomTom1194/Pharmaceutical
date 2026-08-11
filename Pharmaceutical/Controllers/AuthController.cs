using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pharmaceutical.Data;
using Pharmaceutical.Dtos;

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
    }
}
