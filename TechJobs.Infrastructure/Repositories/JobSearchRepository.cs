using Microsoft.EntityFrameworkCore;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Domain.Entities;
using TechJobs.Infrastructure.Data;

namespace TechJobs.Infrastructure.Repositories;

public class JobSearchRepository : IJobSearchRepository
{
    private readonly AppDbContext _ctx;

    public JobSearchRepository(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExperienceYears, string? role)
    {
        // Calls a stored procedure named: dbo.sp_SearchJobs
        // The SP should return columns matching Job entity (and can be expanded later).
        var jobs = await _ctx.Jobs
            .FromSqlInterpolated($@"EXEC dbo.sp_SearchJobs 
                @TechStack={techStack}, 
                @Location={location}, 
                @MinExperienceYears={minExperienceYears}, 
                @Role={role}")
            .ToListAsync();

        return jobs;
    }
}
