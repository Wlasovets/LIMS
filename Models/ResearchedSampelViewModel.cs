using System;

namespace Web.Models
{
    public class ResearchedSampelViewModel
    {
        public int ResultId { get; set; }
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public DateTime ResultDate { get; set; }
        public double ResultValue { get; set; }
        public double LevelValue { get; set; }
        public string Units { get; set; }
    }
}