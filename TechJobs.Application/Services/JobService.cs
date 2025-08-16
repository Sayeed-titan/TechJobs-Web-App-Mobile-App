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

        // Attach tech stacks
        foreach (var tsId in techStackIds)
        {
            job.JobTechStacks.Add(new JobTechStack { Job = job, TechStackId = tsId });
        }

        await _uow.SaveChangesAsync();
        return job;
    }

    public async Task<IEnumerable<Job>> GetApprovedJobsAsync()
    {
        return await _uow.Jobs.GetAllAsync(j => j.IsApproved, "Employer", "JobTechStacks.TechStack");
    }

    public Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExp, string? role)
        => _uow.JobSearch.SearchAsync(techStack, location, minExp, role);
}
