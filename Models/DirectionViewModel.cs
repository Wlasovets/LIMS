using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DirectionViewModel
    {
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public DateTime DirectDate { get; set; }
        public string Department { get; set; }
        public string State { get; set; }
    }
}