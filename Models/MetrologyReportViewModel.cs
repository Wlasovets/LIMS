using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Web.Models
{
    public class MetrologyReportViewModel
    {
        public int SelectedGage { get; set; }

        [Required(ErrorMessage = "Введите дату начала отчетного периода")]
        [DataType(DataType.Date, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime FirstDate { get; set; }

        [Required(ErrorMessage = "Введите дату начала отчетного периода")]
        [DataType(DataType.Date, ErrorMessage = "Дата введена в неверном формате")]
        public DateTime LastDate { get; set; }

        public List<Gage> Gages { get; set; }
        public List<GageGroupeForMetrologyReport> GageGroups { get; set; }
    }
}