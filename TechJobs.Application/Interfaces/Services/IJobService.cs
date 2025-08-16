using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechJobs.Application.DTOs;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(Job job, IEnumerable<int> techStackIds);
        Task<IEnumerable<Job>> GetApprovedJobsAsync();
        Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExp, string? role);

        Task<PagedResult<Job>> GetApprovedJobsPagedAsync(PageRequest req, string? sortBy, string? sortDir);
        Task<PagedResult<Job>> SearchPagedAsync(string? techStack, string? location, int? minExp, string? role, PageRequest req, string? sortBy, string? sortDir);


        Task<bool> ApproveJobAsync(int jobId);
        Task<IEnumerable<Job>> GetPendingJobsAsync();


    }
}
