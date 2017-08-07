using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class GageViewModel
    {
        [Required(ErrorMessage = "Введите наименование")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите заводской номер")]
        public string SerialNumber { get; set; }
    }
}