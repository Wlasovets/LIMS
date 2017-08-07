using System;

namespace Web.Models
{
    public class ProtocolViewModel
    {
        public int SampleId { get; set; }
        public string SampleName { get; set; }
        public string ResultDate { get; set; }
        public string Applicant { get; set; }
        public string Manufacturer { get; set; }
        public string SamplingDate { get; set; }
        public string SamplingTechniqueDesignation { get; set; }
        public string SamplingTechniqueName { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double Radiation { get; set; }
        public string TechniqueOfTestsDesignation { get; set; }
        public string TechniqueOfTestsName { get; set; }
        public string IndicatorName { get; set; }
        public double ResultValue { get; set; }
        public double LevelValue { get; set; }
        public string Units { get; set; }
        public string TechnicalRegulationDesignation { get; set; }
        public string TechnicalRegulationName { get; set; }
        public string GageName { get; set; }
        public string SerialNumber { get; set; }
    }
}