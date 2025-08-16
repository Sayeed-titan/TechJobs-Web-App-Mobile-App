using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(Job job, IEnumerable<int> techStackIds);
        Task<IEnumerable<Job>> GetApprovedJobsAsync();
        Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExp, string? role);
    }
}
