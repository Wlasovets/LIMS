using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DirectionToResearchViewModel
    {
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public DateTime DirectionDate { get; set; }
        public string Applicant { get; set; }
    }
}