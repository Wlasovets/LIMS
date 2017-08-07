using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RobotronViewModel
    {
        public string Activity { get; set; }
        public string AbsoluteError { get; set; }

        [Required(ErrorMessage = "Введите число зарегистрированных гамма-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите значение целого числа")]
        public int NumberOfDecaysSample { get; set; }

        [Required(ErrorMessage = "Введите число зарегистрированных гамма-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите значение целого числа")]
        public int NumberOfDecaysBackground { get; set; }

        [Required(ErrorMessage = "Введите массу пробы")]
        [Range(0, float.MaxValue, ErrorMessage = "Введите числовое значение")]
        public double SampleMass { get; set; }
    }
}