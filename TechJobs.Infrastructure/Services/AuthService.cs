using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechJobs.Application.DTOs;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Domain.Entities;
using TechJobs.Domain.Enums;
using TechJobs.Infrastructure.Data;

namespace TechJobs.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _ctx;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext ctx, IConfiguration config)
    {
        _ctx = ctx;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        // Validate role
        if (!Enum.TryParse<RoleType>(req.Role, ignoreCase: true, out var role))
            throw new ArgumentException("Invalid role. Use Admin, Employer, or Candidate.");

        if (await _ctx.Users.AnyAsync(u => u.Email == req.Email))
            throw new InvalidOperationException("Email already registered.");

        var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        var user = new User
        {
            FullName = req.FullName,
            Email = req.Email,
            PasswordHash = hash,
            Role = role,
            CompanyName = req.CompanyName,
            CompanyWebsite = req.CompanyWebsite,
            SkillsCsv = req.SkillsCsv,
            ResumeUrl = req.ResumeUrl
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        return CreateToken(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        var user = await _ctx.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null) return null;

        try
        {
            var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!ok) return null;
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Legacy/bad hash in DB → treat as invalid credentials
            return null;
        }

        return CreateToken(user);
    }


    private AuthResponse CreateToken(User user)
    {
        var keyStr = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = _config["Jwt:Issuer"] ?? "TechJobs";
        var audience = _config["Jwt:Audience"] ?? "TechJobs.Clients";
        var expiresMins = int.TryParse(_config["Jwt:ExpiresInMinutes"], out var m) ? m : 120;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMins),
            signingCredentials: creds
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse(
            AccessToken: tokenStr,
            TokenType: "Bearer",
            ExpiresAtUtc: token.ValidTo,
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.ToString()
        );
    }
}
