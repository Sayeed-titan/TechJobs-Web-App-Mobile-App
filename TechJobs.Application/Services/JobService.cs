using TechJobs.Application.DTOs;
using TechJobs.Application.Interfaces;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _uow;

    public JobService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Job> CreateJobAsync(Job job, IEnumerable<int> techStackIds)
    {
        job.IsApproved = false; // admin approval later
        await _uow.Jobs.AddAsync(job);

        foreach (var tsId in techStackIds)
        {
            job.JobTechStacks.Add(new JobTechStack { Job = job, TechStackId = tsId });
        }

        await _uow.SaveChangesAsync();
        return job;
    }

    public async Task<IEnumerable<Job>> GetApprovedJobsAsync()
    {
        // include Employer + TechStacks for mapping
        return await _uow.Jobs.GetAllAsync(j => j.IsApproved, "Employer", "JobTechStacks.TechStack");
    }

    public Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExp, string? role)
        => _uow.JobSearch.SearchAsync(techStack, location, minExp, role);

    public async Task<PagedResult<Job>> GetApprovedJobsPagedAsync(PageRequest req, string? sortBy, string? sortDir)
    {
        var all = await GetApprovedJobsAsync(); // IEnumerable<Job> with includes
        var sorted = SortJobs(all, sortBy, sortDir);
        return Page(sorted, req);
    }

    public async Task<PagedResult<Job>> SearchPagedAsync(string? techStack, string? location, int? minExp, string? role, PageRequest req, string? sortBy, string? sortDir)
    {
        var results = await SearchAsync(techStack, location, minExp, role); // List<Job> with includes
        var sorted = SortJobs(results, sortBy, sortDir);
        return Page(sorted, req);
    }

    public async Task<bool> ApproveJobAsync(int jobId)
    {
        var job = await _uow.Jobs.GetByIdAsync(jobId);
        if (job == null) return false;
        job.IsApproved = true;
        _uow.Jobs.Update(job);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Job>> GetPendingJobsAsync()
    {
        return await _uow.Jobs.GetAllAsync(j => !j.IsApproved, "Employer", "JobTechStacks.TechStack");
    }


    // ---- helpers ----
    private static IEnumerable<Job> SortJobs(IEnumerable<Job> jobs, string? sortBy, string? sortDir)
    {
        bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        // supported keys: createdAt, title, location, experience, employer
        switch ((sortBy ?? "").Trim().ToLowerInvariant())
        {
            case "title":
                return desc ? jobs.OrderByDescending(j => j.Title) : jobs.OrderBy(j => j.Title);
            case "location":
                return desc ? jobs.OrderByDescending(j => j.Location) : jobs.OrderBy(j => j.Location);
            case "experience":
            case "minexperienceyears":
                return desc ? jobs.OrderByDescending(j => j.MinExperienceYears ?? int.MaxValue)
                            : jobs.OrderBy(j => j.MinExperienceYears ?? int.MaxValue);
            case "employer":
                return desc ? jobs.OrderByDescending(j => j.Employer.FullName)
                            : jobs.OrderBy(j => j.Employer.FullName);
            case "createdat":
            default:
                return desc ? jobs.OrderByDescending(j => j.CreatedAtUtc)
                            : jobs.OrderBy(j => j.CreatedAtUtc);
        }
    }

    private static PagedResult<Job> Page(IEnumerable<Job> jobs, PageRequest req)
    {
        var page = req.SafePage;
        var size = req.SafePageSize;
        var total = jobs.Count();
        var items = jobs.Skip((page - 1) * size).Take(size).ToList();
        return new PagedResult<Job>(items, page, size, total);
    }
}
