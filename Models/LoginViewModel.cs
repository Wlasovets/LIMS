using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class LoginViewModel
    {
        [Required (ErrorMessage = "Введите логин.")]
        public string LoginName { get; set; }

        [Required (ErrorMessage = "Ввделите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}