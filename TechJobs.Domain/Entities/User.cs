using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TechJobs.Domain.Enums;

namespace TechJobs.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public RoleType Role { get; set; }

        // Candidate fields
        public string? SkillsCsv { get; set; }    // quick MVP; later normalize
        public string? ResumeUrl { get; set; }

        // Employer fields
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }

        // Navigation
        [JsonIgnore]
        public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
        [JsonIgnore]
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
