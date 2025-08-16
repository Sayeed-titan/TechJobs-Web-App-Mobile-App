using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechJobs.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public int JobId { get; set; }
        [JsonIgnore]

        public Job Job { get; set; } = default!;

        public int CandidateId { get; set; }
        [JsonIgnore]

        public User Candidate { get; set; } = default!;

        public string? CoverLetter { get; set; }
        public string Status { get; set; } = "Submitted"; // MVP: Submitted/Reviewed/Accepted/Rejected
    }
}
