using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class ListPipelineResponse
    {
        public int count {  get; set; }
        public List<Pipeline> value { get; set; }
    }
}
