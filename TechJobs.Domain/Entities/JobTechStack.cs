using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechJobs.Domain.Entities
{
    public class JobTechStack
    {
        public int JobId { get; set; }
        [JsonIgnore]

        public Job Job { get; set; } = default!;

        public int TechStackId { get; set; }
        [JsonIgnore]

        public TechStack TechStack { get; set; } = default!;
    }
}
