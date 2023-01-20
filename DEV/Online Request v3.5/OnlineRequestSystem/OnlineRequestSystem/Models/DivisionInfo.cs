using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineRequestSystem.Models
{
    public class DivisionInfo
    {
        public List<AddDivision> _info { get; set; }

        #region Division Info

        public string DivisionID { get; set; }
        public string DivisionAcro { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string ZoneCode { get; set; }
        public IEnumerable<SelectListItem> ListOfDivisionManagers { get; set; }
        public IEnumerable<SelectListItem> IRDivisions { get; set; }
        public string DivisionManager { get; set; }
        public string ApproverResID { get; set; }

        #endregion Division Info
    }

    public class AddDivision
    {
        #region Add division container

        public string DivisionID { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionAcro { get; set; }
        public string DivisionName { get; set; }
        public string ZoneCode { get; set; }
        public string DivisionManager { get; set; }
        public string ApproverResID { get; set; }

        #endregion Add division container
    }

    public class DivisionDetails
    {
        #region Division Details

        public string DivID { get; set; }
        public string DivisionEscalation { get; set; }
        public string Division { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionAcro { get; set; }
        public string ZoneCode { get; set; }
        public string CostCenter { get; set; }
        public string DivisionManager { get; set; }
        public string DivisionResourceID { get; set; }
        public string ContactNum { get; set; }
        public List<DivisionDetails> divDetails_row { get; set; }

        #endregion Division Details
    }
}