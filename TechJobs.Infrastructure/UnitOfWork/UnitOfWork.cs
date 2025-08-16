using TechJobs.Application.Interfaces;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Domain.Entities;
using TechJobs.Infrastructure.Data;
using TechJobs.Infrastructure.Repositories;

namespace TechJobs.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Job> Jobs { get; }
    public IGenericRepository<JobApplication> JobApplications { get; }
    public IGenericRepository<TechStack> TechStacks { get; }
    public IJobSearchRepository JobSearch { get; }

    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        Users = new GenericRepository<User>(_ctx);
        Jobs = new GenericRepository<Job>(_ctx);
        JobApplications = new GenericRepository<JobApplication>(_ctx);
        TechStacks = new GenericRepository<TechStack>(_ctx);
        JobSearch = new JobSearchRepository(_ctx);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _ctx.SaveChangesAsync(cancellationToken);

    public ValueTask DisposeAsync() => _ctx.DisposeAsync();
}
