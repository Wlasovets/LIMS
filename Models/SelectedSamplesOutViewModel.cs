using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class SelectedSamplesOutViewModel
    {
        public int RegistrationNumber { get; set; }
        public string SampleName { get; set; }
        public string Applicant { get; set; }
        public DateTime SamplingDate { get; set; }
    }
}