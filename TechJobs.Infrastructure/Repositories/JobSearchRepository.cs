using Microsoft.EntityFrameworkCore;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Domain.Entities;
using TechJobs.Infrastructure.Data;

namespace TechJobs.Infrastructure.Repositories;

public class JobSearchRepository : IJobSearchRepository
{
    private readonly AppDbContext _ctx;
    public JobSearchRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExperienceYears, string? role)
    {
        // STEP 1: Execute stored proc (NO composition here)
        var baseJobs = await _ctx.Jobs
            .FromSqlInterpolated($@"EXEC dbo.sp_SearchJobs 
                @TechStack={techStack}, 
                @Location={location}, 
                @MinExperienceYears={minExperienceYears}, 
                @Role={role}")
            .AsNoTracking()
            .ToListAsync();

        if (baseJobs.Count == 0)
            return new List<Job>();

        var ids = baseJobs.Select(j => j.Id).ToList();

        // STEP 2: Compose normally (EF query) to load related data
        var richJobs = await _ctx.Jobs
            .Where(j => ids.Contains(j.Id))
            .Include(j => j.Employer)
            .Include(j => j.JobTechStacks).ThenInclude(jt => jt.TechStack)
            .AsNoTracking()
            .ToListAsync();

        // Preserve ordering roughly (optional)
        var order = ids.Select((id, idx) => new { id, idx }).ToDictionary(x => x.id, x => x.idx);
        richJobs.Sort((a, b) => order[a.Id].CompareTo(order[b.Id]));

        return richJobs;
    }
}
