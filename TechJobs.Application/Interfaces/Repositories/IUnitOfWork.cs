using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechJobs.Domain.Entities;

namespace TechJobs.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Job> Jobs { get; }
        IGenericRepository<JobApplication> JobApplications { get; }
        IGenericRepository<TechStack> TechStacks { get; }
        IJobSearchRepository JobSearch { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
