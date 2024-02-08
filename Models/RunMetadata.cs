using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class RunMetadata
    {
        public object? _links { get; set; }
        public object? variables { get; set; }
        public object? templateParameters { get; set; }
        public object? pipeline { get; set; }
        public object? state { get; set; }
        public object? result { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? finishedDate { get; set; }
        public string? url { get; set; }
        public int id { get; set; }
        public object? name { get; set; }
    }
}



   