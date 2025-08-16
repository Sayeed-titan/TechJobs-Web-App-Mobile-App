using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechJobs.Domain.Entities;

namespace TechJobs.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<TechStack> TechStacks => Set<TechStack>();
        public DbSet<JobTechStack> JobTechStacks => Set<JobTechStack>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FullName).HasMaxLength(150).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(180).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Jobs
            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(j => j.Title).HasMaxLength(180).IsRequired();
                entity.Property(j => j.Role).HasMaxLength(120);
                entity.Property(j => j.Location).HasMaxLength(120);

                entity.HasOne(j => j.Employer)
                      .WithMany(u => u.PostedJobs)
                      .HasForeignKey(j => j.EmployerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // TechStacks
            modelBuilder.Entity<TechStack>(entity =>
            {
                entity.Property(t => t.Name).HasMaxLength(80).IsRequired();
            });

            // JobTechStacks (join table)
            modelBuilder.Entity<JobTechStack>(entity =>
            {
                entity.ToTable("JobTechStacks");

                entity.HasKey(jt => new { jt.JobId, jt.TechStackId });

                entity.HasOne(jt => jt.Job)
                      .WithMany(j => j.JobTechStacks)
                      .HasForeignKey(jt => jt.JobId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(jt => jt.TechStack)
                      .WithMany(ts => ts.JobTechStacks)
                      .HasForeignKey(jt => jt.TechStackId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // JobApplications
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.Property(a => a.Status).HasMaxLength(40).IsRequired();

                entity.HasOne(a => a.Job)
                      .WithMany(j => j.Applications)
                      .HasForeignKey(a => a.JobId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Candidate)
                      .WithMany(u => u.Applications)
                      .HasForeignKey(a => a.CandidateId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
