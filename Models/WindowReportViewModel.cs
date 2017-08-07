using System.Collections.Generic;

namespace Web.Models
{
    public class WindowReportViewModel
    {
        public GenerateReportViewModel GenerateReport { get; set; }
        public IEnumerable<CountOfResultsViewModel> CountOfResults { get; set; }
    }
}