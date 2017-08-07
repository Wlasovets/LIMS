using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class GenerateReportViewModel
    {
        [Required(ErrorMessage = "*")]
        public Department Department { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        [DataType(DataType.Date, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime FirstDate { get; set; }

        [Required(ErrorMessage = "Введите дату")]
        [DataType(DataType.Date, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime LastDate { get; set; }
    }
}