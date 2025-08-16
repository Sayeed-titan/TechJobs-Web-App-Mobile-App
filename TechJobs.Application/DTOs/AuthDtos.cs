namespace TechJobs.Application.DTOs;

public record RegisterRequest(
    string FullName,
    string Email,
    string Password,
    string Role,                // "Admin" | "Employer" | "Candidate"
    string? CompanyName,
    string? CompanyWebsite,
    string? SkillsCsv,
    string? ResumeUrl
);

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    int UserId,
    string FullName,
    string Email,
    string Role
);
