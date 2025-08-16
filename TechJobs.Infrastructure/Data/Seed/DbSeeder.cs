using Microsoft.EntityFrameworkCore;
using TechJobs.Domain.Entities;
using TechJobs.Domain.Enums;

namespace TechJobs.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext ctx)
    {
        await ctx.Database.MigrateAsync();

        // Ensure (create or fix) users with proper bcrypt hashes
        var admin = await EnsureUserAsync(
            ctx,
            email: "admin@techjobs.local",
            fullName: "System Admin",
            plainPassword: "Admin@123",
            role: RoleType.Admin
        );

        var employer = await EnsureUserAsync(
            ctx,
            email: "hr@acme.local",
            fullName: "Acme HR",
            plainPassword: "Employer@123",
            role: RoleType.Employer,
            companyName: "Acme Software",
            companyWebsite: "https://acme.local"
        );

        var candidate = await EnsureUserAsync(
            ctx,
            email: "john@dev.local",
            fullName: "John Candidate",
            plainPassword: "Candidate@123",
            role: RoleType.Candidate,
            skillsCsv: "C#, .NET, Angular",
            resumeUrl: "https://example.com/resume/john"
        );

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

        // Jobs
        if (!await ctx.Jobs.AnyAsync())
        {
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
                IsApproved = false
            };

            await ctx.Jobs.AddRangeAsync(job1, job2);
            await ctx.SaveChangesAsync();

            // attach tech stacks
            TechStack TS(string name) => ctx.TechStacks.First(ts => ts.Name == name);
            ctx.JobTechStacks.AddRange(
                new JobTechStack { JobId = job1.Id, TechStackId = TS("C#").Id },
                new JobTechStack { JobId = job1.Id, TechStackId = TS("ASP.NET Core").Id },
                new JobTechStack { JobId = job1.Id, TechStackId = TS("SQL Server").Id },

                new JobTechStack { JobId = job2.Id, TechStackId = TS("C#").Id },
                new JobTechStack { JobId = job2.Id, TechStackId = TS("Angular").Id },
                new JobTechStack { JobId = job2.Id, TechStackId = TS("React").Id }
            );

            await ctx.SaveChangesAsync();
        }
    }

    private static async Task<User> EnsureUserAsync(
        AppDbContext ctx,
        string email,
        string fullName,
        string plainPassword,
        RoleType role,
        string? companyName = null,
        string? companyWebsite = null,
        string? skillsCsv = null,
        string? resumeUrl = null)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
                Role = role,
                CompanyName = companyName,
                CompanyWebsite = companyWebsite,
                SkillsCsv = skillsCsv,
                ResumeUrl = resumeUrl
            };
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            return user;
        }

        // Update weak/legacy hashes in-place
        if (!IsBcrypt(user.PasswordHash))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // Keep other metadata fresh (optional)
        user.FullName = fullName;
        user.Role = role;
        if (companyName is not null) user.CompanyName = companyName;
        if (companyWebsite is not null) user.CompanyWebsite = companyWebsite;
        if (skillsCsv is not null) user.SkillsCsv = skillsCsv;
        if (resumeUrl is not null) user.ResumeUrl = resumeUrl;

        ctx.Users.Update(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    private static bool IsBcrypt(string? hash)
        => !string.IsNullOrWhiteSpace(hash) &&
           (hash!.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$"));
}
