using Microsoft.EntityFrameworkCore;
using TechJobs.Domain.Entities;

namespace TechJobs.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext ctx)
    {
        // Ensure DB/migrations are applied
        await ctx.Database.MigrateAsync();

        // Users (Admin, Employer, Candidate)
        if (!await ctx.Users.AnyAsync())
        {
            var admin = new User
            {
                FullName = "System Admin",
                Email = "admin@techjobs.local",
                PasswordHash = "admin-hash", // TODO: replace with real hash when auth is added
                Role = Domain.Enums.RoleType.Admin
            };
            var employer = new User
            {
                FullName = "Acme HR",
                Email = "hr@acme.local",
                PasswordHash = "employer-hash",
                Role = Domain.Enums.RoleType.Employer,
                CompanyName = "Acme Software",
                CompanyWebsite = "https://acme.local"
            };
            var candidate = new User
            {
                FullName = "John Candidate",
                Email = "john@dev.local",
                PasswordHash = "candidate-hash",
                Role = Domain.Enums.RoleType.Candidate,
                SkillsCsv = "C#, .NET, Angular",
                ResumeUrl = "https://example.com/resume/john"
            };

            await ctx.Users.AddRangeAsync(admin, employer, candidate);
            await ctx.SaveChangesAsync();
        }

        // Tech stacks
        if (!await ctx.TechStacks.AnyAsync())
        {
            var stacks = new[]
            {
                new TechStack { Name = "C#" },
                new TechStack { Name = ".NET" },
                new TechStack { Name = "ASP.NET Core" },
                new TechStack { Name = "Angular" },
                new TechStack { Name = "React" },
                new TechStack { Name = "SQL Server" },
                new TechStack { Name = "Azure" }
            };
            await ctx.TechStacks.AddRangeAsync(stacks);
            await ctx.SaveChangesAsync();
        }

        // Jobs + links
        if (!await ctx.Jobs.AnyAsync())
        {
            var employer = await ctx.Users.FirstAsync(u => u.Role == Domain.Enums.RoleType.Employer);

            var job1 = new Job
            {
                Title = "Backend Developer",
                Role = "Backend Developer",
                Description = "Build ASP.NET Core APIs for TechJobs.",
                Location = "Dhaka",
                EmployerId = employer.Id,
                MinExperienceYears = 2,
                IsApproved = true
            };

            var job2 = new Job
            {
                Title = "Fullstack Engineer",
                Role = "Fullstack",
                Description = "Work with .NET + Angular + SQL Server.",
                Location = "Remote",
                EmployerId = employer.Id,
                MinExperienceYears = 1,
                IsApproved = false // waiting for admin approval
            };

            await ctx.Jobs.AddRangeAsync(job1, job2);
            await ctx.SaveChangesAsync();

            // attach tech stacks
            var find = (string name) => ctx.TechStacks.First(ts => ts.Name == name);

            ctx.JobTechStacks.AddRange(
                new JobTechStack { JobId = job1.Id, TechStackId = find("C#").Id },
                new JobTechStack { JobId = job1.Id, TechStackId = find("ASP.NET Core").Id },
                new JobTechStack { JobId = job1.Id, TechStackId = find("SQL Server").Id },

                new JobTechStack { JobId = job2.Id, TechStackId = find("C#").Id },
                new JobTechStack { JobId = job2.Id, TechStackId = find("Angular").Id },
                new JobTechStack { JobId = job2.Id, TechStackId = find("React").Id }
            );

            await ctx.SaveChangesAsync();
        }
    }
}
