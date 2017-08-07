using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class CertificateViewModel
    {
        [Required(ErrorMessage = "Введите номер свидетельства")]
        public string CertificateNumber { get; set; }

        [Required(ErrorMessage = "Введите дату поверки")]
        [DataType(DataType.DateTime, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime VerificationDate { get; set; }

        [Required(ErrorMessage = "Введите дату завершения срока поверки")]
        [DataType(DataType.DateTime, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime EndVerificationDate { get; set; }

        public int GageId { get; set; }
        public string GageName { get; set; }
        public string SerialNumber { get; set; }
    }
}