namespace TechJobs.Application.DTOs;

public record TechStackDto(int Id, string Name);
public record EmployerDto(int Id, string FullName, string? CompanyName, string? CompanyWebsite);
public record JobDto(
    int Id,
    string Title,
    string? Role,
    string? Description,
    string? Location,
    int? MinExperienceYears,
    bool IsApproved,
    EmployerDto Employer,
    List<TechStackDto> TechStacks
);
