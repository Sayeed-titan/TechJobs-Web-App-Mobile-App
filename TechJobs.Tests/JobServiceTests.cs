using Microsoft.EntityFrameworkCore;
using TechJobs.Application.Interfaces;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Application.Services;
using TechJobs.Domain.Entities;
using TechJobs.Infrastructure.Data;
using TechJobs.Infrastructure.Repositories;
using TechJobs.Infrastructure.UnitOfWork;

namespace TechJobs.Tests;

public class JobServiceTests
{
    private AppDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(IJobService svc, IUnitOfWork uow, AppDbContext ctx, User employer, TechStack ts1)> BootstrapAsync()
    {
        var ctx = NewInMemoryContext();

        var uow = new UnitOfWork(ctx);
        var svc = new JobService(uow);

        var employer = new User { FullName = "Emp", Email = "e@x", PasswordHash = "h", Role = TechJobs.Domain.Enums.RoleType.Employer };
        var ts1 = new TechStack { Name = "C#" };
        var ts2 = new TechStack { Name = "ASP.NET Core" };
        await ctx.Users.AddAsync(employer);
        await ctx.TechStacks.AddRangeAsync(ts1, ts2);
        await ctx.SaveChangesAsync();

        return (svc, uow, ctx, employer, ts1);
    }

    [Fact]
    public async Task CreateJob_Adds_Job_And_TechStacks()
    {
        var (svc, uow, ctx, employer, ts1) = await BootstrapAsync();

        var job = new Job
        {
            Title = "API Dev",
            Role = "Backend",
            EmployerId = employer.Id,
            Location = "Dhaka",
            MinExperienceYears = 2
        };

        var created = await svc.CreateJobAsync(job, new[] { ts1.Id });
        await uow.SaveChangesAsync();

        var stored = await ctx.Jobs.Include(j => j.JobTechStacks).FirstOrDefaultAsync(j => j.Id == created.Id);

        Assert.NotNull(stored);
        Assert.Single(stored!.JobTechStacks);
        Assert.False(stored.IsApproved); // default false
    }

    [Fact]
    public async Task GetApprovedJobs_Returns_Only_Approved()
    {
        var (svc, uow, ctx, employer, _) = await BootstrapAsync();

        var a = new Job { Title = "A", EmployerId = employer.Id, IsApproved = true };
        var b = new Job { Title = "B", EmployerId = employer.Id, IsApproved = false };
        await ctx.Jobs.AddRangeAsync(a, b);
        await ctx.SaveChangesAsync();

        var approved = await svc.GetApprovedJobsAsync();
        Assert.Single(approved);
        Assert.Equal("A", approved.First().Title);
    }
}
