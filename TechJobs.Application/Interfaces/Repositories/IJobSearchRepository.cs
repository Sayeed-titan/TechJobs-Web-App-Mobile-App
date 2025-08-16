using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Interfaces.Repositories
{
    public interface IJobSearchRepository
    {
        Task<List<Job>> SearchAsync(string? techStack, string? location, int? minExperienceYears, string? role);
    }
}
