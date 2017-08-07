using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Введите должность")]
        public string Post { get; set; }

        [Required(ErrorMessage = "Введите отдел")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль подтвержден неверно")]
        public string ConfirmPassword { get; set; }
    }
}