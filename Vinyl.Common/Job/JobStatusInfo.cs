using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.Common.Job
{
    public class JobStatusInfo
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? LastStartDate { get; set; }
        public DateTime? LastFinishDate { get; set; }
        public string Result { get; set; }
    }
}
