using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ErnViewModel
    {
        [Required(ErrorMessage = "Введите активность радия")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double RadiumActivity { get; set; }

        [Required(ErrorMessage = "Введите активность тория")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double ThoriumActivity { get; set; }

        [Required(ErrorMessage = "Введите активность калия")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double PotassiumActivity { get; set; }

        [Required(ErrorMessage = "Введите погрешность радия")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double RadiumError { get; set; }

        [Required(ErrorMessage = "Введите погрешность тория")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double ThoriumError { get; set; }

        [Required(ErrorMessage = "Введите погрешность калия")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double PotassiumError { get; set; }

        public string Result { get; set; }
    }
}