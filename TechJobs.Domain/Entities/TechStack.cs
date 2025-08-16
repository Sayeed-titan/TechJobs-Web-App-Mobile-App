using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechJobs.Domain.Entities
{
    public class TechStack : BaseEntity
    {
        public string Name { get; set; } = default!;
        [JsonIgnore]

        public ICollection<JobTechStack> JobTechStacks { get; set; } = new List<JobTechStack>();
    }

}