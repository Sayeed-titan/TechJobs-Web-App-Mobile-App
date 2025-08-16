using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechJobs.Domain.Entities
{
    public class Job : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string? Role { get; set; }               // e.g., "Backend Developer"
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int? MinExperienceYears { get; set; }
        public bool IsApproved { get; set; } = false;

        // Employer
        public int EmployerId { get; set; }
        public User Employer { get; set; } = default!;
        [JsonIgnore]

        public ICollection<JobTechStack> JobTechStacks { get; set; } = new List<JobTechStack>();
        [JsonIgnore]

        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }
}
