using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class UmfViewModel
    {
        [Required(ErrorMessage = "Введите число альфа-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundAlphaDecays1 { get; set; }

        [Required(ErrorMessage = "Введите число альфа-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundAlphaDecays2 { get; set; }

        [Required(ErrorMessage = "Введите число альфа-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundAlphaDecays3 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundBetaDecays1 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundBetaDecays2 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов фона")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountOfBackgroundBetaDecays3 { get; set; }

        [Required(ErrorMessage = "Введите число альфа-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountAlphaDecaysOfSample1 { get; set; }

        [Required(ErrorMessage = "Введите число альфа-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountAlphaDecaysOfSample2 { get; set; }

        [Required(ErrorMessage = "Введите число альфа-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountAlphaDecaysOfSample3 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountBetaDecaysOfSample1 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountBetaDecaysOfSample2 { get; set; }

        [Required(ErrorMessage = "Введите число бета-распадов пробы")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int AmountBetaDecaysOfSample3 { get; set; }

        [Required(ErrorMessage = "Введите время измерения")]
        [Range(0, int.MaxValue, ErrorMessage = "Введите целое число")]
        public int Time { get; set; }

        public string AlphaActivity { get; set; }
        public string BetaActivity { get; set; }
        public string AbsoluteUncertaintyForAlpha { get; set; }
        public string AbsoluteUncertaintyForBeta { get; set; }
    }
}