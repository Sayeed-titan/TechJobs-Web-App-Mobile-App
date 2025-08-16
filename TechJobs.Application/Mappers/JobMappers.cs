using System.Linq;
using TechJobs.Application.DTOs;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Mappers;

public static class JobMappers
{
    public static JobDto ToDto(this Job j)
    {
        var employer = new EmployerDto(
            j.Employer.Id,
            j.Employer.FullName,
            j.Employer.CompanyName,
            j.Employer.CompanyWebsite
        );

        var stacks = j.JobTechStacks
            .Select(x => new TechStackDto(x.TechStack.Id, x.TechStack.Name))
            .ToList();

        return new JobDto(
            j.Id,
            j.Title,
            j.Role,
            j.Description,
            j.Location,
            j.MinExperienceYears,
            j.IsApproved,
            employer,
            stacks
        );
    }
}
