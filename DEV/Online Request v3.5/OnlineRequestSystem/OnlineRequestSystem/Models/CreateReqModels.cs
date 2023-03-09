using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OnlineRequestSystem.Models
{
    public class CreateReqModels : ReqApproverStatus
    {
        #region Create request models

        public string returnUrl { get; set; }
        public string RequestNo { get; set; }
        public string Author { get; set; }
        public string Jobtitle { get; set; }
        public string Bednrm { get; set; }
        public string BranchCode { get; set; }
        public string Division { get; set; }
        public string DivisionCode { get; set; }
        public string AreaCode { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string ZoneCode { get; set; }
        public string ReqDate { get; set; }
        public int hasDiagnostic { get; set; }
        public string reqTrigger { get; set; }
        public string reqStat { get; set; }

        public string RequestType { get; set; }
        public string Description { get; set; }
        public String reqTotal { get; set; }

        public string OverallTotalPrice { get; set; }

        public string Approver { get; set; }
        public int ForPO { get; set; }
        public string BranchManager { get; set; }
        public string AreaManager { get; set; }
        public string RegionalManager { get; set; }
        public string DepartmentManager { get; set; }
        public string DivisionManager { get; set; }

        #endregion Create request models

        #region for View All Open Requests

        // for View All Open Requests
        public string reqCreator { get; set; }
        public string office { get; set; }
        public string reqStatus { get; set; }
        public int isDivRequest { get; set; }
        public string req_DeptCode { get; set; }
        public int forPresident { get; set; }
        public string TotalQty_MMD { get; set; }
        public string TotalQty_SDC { get; set; }
        public string TotalQty_Branch { get; set; }
        public string TotalQty_Div { get; set; }

        #endregion for View All Open Requests

        #region for Request Items Model

        public string TotalQty { get; set; }
        public string TotalUnitCost { get; set; }
        public string OverallTotalCost { get; set; }
        public int isApproved { get; set; }
        public string sysCreated { get; set; }
        public string sysCreator { get; set; }
        public string sysModified { get; set; }
        public string sysModifier { get; set; }



        #endregion for Request Items Model

        #region for Escalation

        public string E_DMName { get; set; }
        public string E_DMDate { get; set; }
        public string E_DMRemarks { get; set; }

        public string E_AMName { get; set; }
        public string E_AMDate { get; set; }
        public string E_AMRemarks { get; set; }

        public string E_RMName { get; set; }
        public string E_RMDate { get; set; }
        public string E_RMRemarks { get; set; }

        public string E_VPAssistantName { get; set; }
        public string E_VPAssistantDate { get; set; }
        public string E_VPAssistantRemarks { get; set; }

        public string E_GMName { get; set; }
        public string E_GMDate { get; set; }
        public string E_GMRemarks { get; set; }

        public string E_PresName { get; set; }
        public string E_PresDate { get; set; }
        public string E_PresRemarks { get; set; }

        public string E_LocalDivName { get; set; }
        public string E_LocalDivDate { get; set; }
        public string E_LocalDivRemarks { get; set; }

        public string E_DivName { get; set; }
        public string E_DivDate { get; set; }
        public string E_DivRemarks { get; set; }

        public string E_Div2Name { get; set; }

        [DataType(DataType.Date)]
        public string E_Div2Date { get; set; }

        public string E_Div2Remarks { get; set; }

        public string E_Div3Name { get; set; }

        [DataType(DataType.Date)]
        public string E_Div3Date { get; set; }

        public string E_Div3Remarks { get; set; }

        public string E_VPO_POName { get; set; }
        public string E_VPO_PODate { get; set; }
        public string E_VPO_PORemarks { get; set; }
        public string E_Pres_POName { get; set; }
        public string E_Pres_PODate { get; set; }
        public string E_Pres_PORemarks { get; set; }
        public string commentSection { get; set; }

        #endregion for Escalation

        #region for approver status w/ IEnumerables

        public List<RequestItems> ReqItems { get; set; }
        public List<ShowAllComments> ShowComments { get; set; }
        public IEnumerable<SelectListItem> ListReqtype { get; set; }

        public IEnumerable<SelectListItem> ListRequestUnits
        {
            get
            {
                List<SelectListItem> units = new List<SelectListItem>();
                var data = new[]{
                 new SelectListItem{ Value="pc/s",Text="pc/s"},
                 new SelectListItem{ Value="pad/s",Text="pad/s"},
                 new SelectListItem{ Value="bot/s",Text="bot/s"},
                 new SelectListItem{ Value="dozen",Text="dozen"},
                 new SelectListItem{ Value="box/s",Text="box/s"},
                 new SelectListItem{ Value="unit/s",Text="unit/s"},
                 new SelectListItem{ Value="roll/s",Text="roll/s"}
             };
                units = data.ToList();
                return units.ToList();
            }
        }

        public List<RequestItems> Items { get; set; }

        #endregion for approver status w/ IEnumerables
    }

    public class ShowAllComments
    {
        #region Show Comments
        public string comments { get; set; }
        public string commCreator { get; set; }
        public string commCreatorID { get; set; }
        public string commcreated { get; set; }
        #endregion Show Comments
    }

    public class RequestItems
    {
        #region Request items

        public string ItemDescription { get; set; }
        public string ItemQty { get; set; }
        public string ItemUnitCost { get; set; }
        public string ItemTotalCost { get; set; }
        public string TotalPrice { get; set; }
        public string ItemUnit { get; set; }
        public string ItemStatus { get; set; }

        public string actualQtyMMD { get; set; }
        public string actualQtySDC { get; set; }
        public string actualQtyBranch { get; set; }
        public string actualQtyDiv { get; set; }

        public int isCheckedMMD { get; set; }
        public int isCheckedSDC { get; set; }
        public int isCheckedBranch { get; set; }
        public int isCheckedDiv { get; set; }

        public string MMDstatus { get; set; }
        public string SDCStatus { get; set; }
        public string BranchStatus { get; set; }
        public string DivStatus { get; set; }
        public string StatusOfStock { get; set; }


        #endregion Request items
    }
}