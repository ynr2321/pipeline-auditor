using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class Pipeline
    {
        public string? url {  get; set; }
        public object? id { get; set; }
        public string? name { get; set; }
        public object? _links { get; set; }
        public string? folder { get; set; }
        public int? revision { get; set; }

    }
}
