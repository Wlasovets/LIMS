using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ResearchSampleViewModel
    {
        public int SampleId { get; set; }
        public string SampleName { get; set; }

        [Required(ErrorMessage = "Введите показатель")]
        public string Indicator { get; set; }

        [Required(ErrorMessage = "Введите единицы измерения")]
        public string Units { get; set; }

        [Required(ErrorMessage = "Введите результат")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double Result { get; set; }

        [Required(ErrorMessage = "Введите допустимый уровень")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double AdmissibleLever { get; set; }

        [Required(ErrorMessage = "Введите обозначение методики")]
        public string TechniqueOfTestDesignation { get; set; }

        [Required(ErrorMessage = "Введите наименование методики")]
        public string TechniqueOfTestName { get; set; }

        [Required(ErrorMessage = "Введите обозначение нормирующего документа")]
        public string NormativeDocumentDesignation { get; set; }

        [Required(ErrorMessage = "Введите наименование нормирующего документа")]
        public string NormativeDocumentName { get; set; }

        [Required(ErrorMessage = "Введите температуру")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double Temperature { get; set; }

        [Required(ErrorMessage = "Введите влажность")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double Humidity { get; set; }

        [Required(ErrorMessage = "Введите атмосферное давление")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double Pressure { get; set; }

        [Required(ErrorMessage = "Введите МЭД")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double Radiation { get; set; }

        [Required(ErrorMessage = "Введите имя сотрудника")]
        public string EmployeeFirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию сотрудника")]
        public string EmployeeLastName { get; set; }

        [Required(ErrorMessage = "Введите отчество сотрудника")]
        public string EmployeeMiddleName { get; set; }

        [Required(ErrorMessage = "Введите должность сотрудника")]
        public string EmployeePost { get; set; }

        [Required(ErrorMessage = "Введите наименование СИ")]
        public string GageName { get; set; }

        [Required(ErrorMessage = "Введите заводской номер СИ")]
        public string GageSerialNumber { get; set; }
    }
}