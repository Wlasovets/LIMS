using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class RegistrationSampleViewModel
    {
        [Required (ErrorMessage = "Введите наименование пробы.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите заявителя.")]
        public string Applicant { get; set; }
        public string PhoneNumber { get; set; }
    }
}