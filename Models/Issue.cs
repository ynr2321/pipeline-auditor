using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class Issue
    {
        public string? Type { get; set; }
        public string? Category { get; set; }
        public object? Message { get; set; }
        public object? Data { get; set; }
    }
}
