using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlMaystro.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace AlMaystro.API.Controllers;

[ApiController, Route("api/auth")]
public class AuthController(IConfiguration cfg) : ControllerBase
{
    [HttpPost("login"), EnableRateLimiting("login")]
    public ActionResult<LoginResult> Login(LoginDto dto)
    {
        var user = cfg["Owner:Username"];
        var hash = cfg["Owner:PasswordHash"]; // BCrypt hash
        if (dto.Username != user || string.IsNullOrEmpty(hash) || !BCrypt.Net.BCrypt.Verify(dto.Password, hash))
            return Unauthorized(new { message = "بيانات الدخول غير صحيحة" });

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
        var expires = DateTime.UtcNow.AddDays(30); // long-lived so the mobile app stays signed in
        var token = new JwtSecurityToken(
            issuer: cfg["Jwt:Issuer"], audience: cfg["Jwt:Audience"],
            claims: new[] { new Claim(ClaimTypes.Name, user!), new Claim(ClaimTypes.Role, "Owner") },
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new LoginResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
