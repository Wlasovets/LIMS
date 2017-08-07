using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class SelectedSampleViewModel
    {
        public int SampleId { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string EmployeeFirstName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        public string EmployeeLastName { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        public string EmployeeMiddleName { get; set; }

        [Required(ErrorMessage = "Введите должность")]
        public string EmployeePost { get; set; }

        [Required(ErrorMessage = "Введите обозначение ТНПА")]
        public string TrDesignation { get; set; }

        [Required(ErrorMessage = "Введите наименование ТНПА")]
        public string TrName { get; set; }

        [Required(ErrorMessage = "Введите количество отобранной пробы")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public float NumberOfSelectedSample { get; set; }

        [Required(ErrorMessage = "Введите единицы измерения")]
        public string Units { get; set; }

        [Required(ErrorMessage = "Введите изготовителя")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "Введите место отбора")]
        public string SamplingPlace { get; set; }
    }
}