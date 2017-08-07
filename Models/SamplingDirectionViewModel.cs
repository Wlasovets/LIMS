using System;

namespace Web.Models
{
    public class SamplingDirectionViewModel
    {
        public int SampleId { get; set; }
        public DateTime DirectionDate { get; set; }
        public string SampleName { get; set; }
        public string Applicant { get; set; }
        public string ContactPhone { get; set; }
    }
}