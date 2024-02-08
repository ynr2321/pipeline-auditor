using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class Record
    {
        public object? previousAttempts { get; set; }
        public Guid? id { get; set; }
        public Guid? parentId { get; set; }
        public string? type { get; set; }
        public string? name { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? finishTime { get; set; }
        public string? currentOperation { get; set; }
        public string? percentComplete { get; set; }
        public string? state { get; set; }
        public string? result { get; set; }
        public string? resultCode { get; set; }
        public int? changeId { get; set; }
        public DateTime? lastModified { get; set; }
        public string? workerName { get; set; }
        public int? order { get; set; }
        public Details? details { get; set; }
        public int? errorCount { get; set; }
        public int? warningCount { get; set; }
        public string? url { get; set; }
        public object? log { get; set; }
        public object? task { get; set; }
        public int? attempt { get; set; }
        public string? identifier { get; set; }
        public List<Issue>? issues { get; set; }

    }
}
