using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Web.Models
{
    public class TechnicalRegulationsViewModel
    {
        [Required(ErrorMessage = "Введите обозначение")]
        public string TrDesignation { get; set; }

        [Required(ErrorMessage = "Введите наименование")]
        public string TrName { get; set; }
        public string TrNote { get; set; }
        public IEnumerable<TechnicalRegulation> TechnicalRegulations { get; set; }
    }
}