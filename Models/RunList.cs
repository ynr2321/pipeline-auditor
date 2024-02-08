using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class RunList
    {
        public int count { get; set; }
        public List<RunMetadata> value { get; set; }
    }
}