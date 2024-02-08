using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pipelineAuditor.Models
{
    internal class Run
    {
        public List<Record> Records { get; set; }
    }
}





//internal class Run : IEnumerable<Record>
//{
//    public List<Record> Records { get; set; }

//    public Run()
//    {
//        Records = new List<Record>();
//    }


//    // Implementing interface methods
//    public IEnumerator<Record> GetEnumerator()
//    {
//        return Records.GetEnumerator();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return Records.GetEnumerator();
//    }

//}