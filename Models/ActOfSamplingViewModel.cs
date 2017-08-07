using System;

namespace Web.Models
{
    public class ActOfSamplingViewModel
    {
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public DateTime SamplingDate { get; set; }
        public string Applicant { get; set; }
        public string Manufacturer { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeMiddleName { get; set; }
        public string EmployeePost { get; set; }
        public string SamplingTechniqueName { get; set; }
        public string SamplingTechniqueDesignation { get; set; }
        public double Number { get; set; }
        public string Units { get; set; }
    }
}