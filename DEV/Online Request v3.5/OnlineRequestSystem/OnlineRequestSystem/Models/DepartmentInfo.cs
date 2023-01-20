using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineRequestSystem.Models
{
    public class DepartmentInfo
    {
        public List<DepartmentInfo> _info { get; set; }

        #region Department Information

        public string deptID { get; set; }
        public string deptCode { get; set; }
        public string deptManager { get; set; }
        public string approver_resID { get; set; }
        public string division { get; set; }
        public string divcode { get; set; }
        public string divacro { get; set; }
        public string ZoneCode { get; set; }

        #endregion Department Information

        public IEnumerable<SelectListItem> IRDivisions { get; set; }

        public IEnumerable<SelectListItem> Employees
        {
            get
            {
                return new[] { new SelectListItem { Value = "0", Text = " " } };
            }
        }
    }
}