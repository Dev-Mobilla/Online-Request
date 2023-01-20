using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineRequestSystem.Models
{
    public class ReportViewModel
    {
        public string branch { get; set; }
        public string division { get; set; }
        public string area { get; set; }
        public string region { get; set; }

        public string Date { get; set; }
        public string reportType { get; set; }
        public string BeginFormMethod { get; set; }
        public IEnumerable<SelectListItem> ListBranches { get; set; }
        public IEnumerable<SelectListItem> ListDivision { get; set; }
        public IEnumerable<SelectListItem> ListArea { get; set; }
        public IEnumerable<SelectListItem> ListRegions { get; set; }
        public string NoDataFound { get; set; }
    }
}