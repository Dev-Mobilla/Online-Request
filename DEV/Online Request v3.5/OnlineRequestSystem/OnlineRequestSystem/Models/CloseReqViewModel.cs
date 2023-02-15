using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineRequestSystem.Models
{
    #region Close Request View Models

    public class CloseReqViewModel
    {
        public string reqNumber { get; set; }
        public string reqCreator { get; set; }
        public string reqDescription { get; set; }
        public string reqDate { get; set; }
        public string reqStatus { get; set; }
        public string closedDate { get; set; }
        public string TypeID { get; set; }
        public string TotalCount { get; set; }
        public string itemDescription { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ZoneCode { get; set; }
        public string Region { get; set; }
        public string TypeName { get; set; }
        public string TotalQuantity { get; set; }
        public string isDivRequest { get; set; }
        public string DeptCode { get; set; }

        public int? isApprovedAM { get; set; }
        public int isApprovedRM { get; set; }
        public int isApprovedGM { get; set; }
        public int isApprovedDiv1 { get; set; }
        public int isApprovedDiv2 { get; set; }
        public int isApprovedDiv3 { get; set; }
        public int isApprovedPres { get; set; }

        public int reqAM { get; set; }
        public int reqRM { get; set; }
        public int reqGM { get; set; }
        public int reqDiv1 { get; set; }
        public int reqDiv2 { get; set; }
        public int reqDiv3 { get; set; }
        public int reqPres { get; set; }

        public int MMD_Processed { get; set; }
        public int MMD_ForDelivery { get; set; }
        public int MMD_InTransit { get; set; }

        public int result_AM { get; set; }
        public int result_RM { get; set; }
        public int result_Div1 { get; set; }
        public int result_Div2 { get; set; }
        public int result_Div3 { get; set; }
        public int result_GM { get; set; }
        public int result_Pres { get; set; }
    }

    #endregion Close Request View Models

    #region Close request : Request approver status

    public class CloseRequest : ReqApproverStatus
    {
        public string reqNumber { get; set; }
        public string reqDate { get; set; }
        public string reqCreator { get; set; }
        public string TypeID { get; set; }
        public string reqDescription { get; set; }
        public string reqTotal { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string AreaCode { get; set; }
        public string DivCode { get; set; }
        public string DeptCode { get; set; }

        public string ZoneCode { get; set; }
        public string reqStatus { get; set; }
        public string isApproved { get; set; }
        public int isDivRequest { get; set; }
        public int forPresident { get; set; }

        public string TotalQty { get; set; }
        public string TotalQty_Branch { get; set; }
        public string TotalQty_Div { get; set; }
        public string TotalQty_MMD { get; set; }
        public string TotalQty_SDC { get; set; }

        public string Approver { get; set; }
        public int ForPO { get; set; }
        public string ClosedDate { get; set; }
        public string ClosedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Remarks { get; set; }

        public string E_DMName { get; set; }
        public string E_DMDate { get; set; }
        public string E_DMRemarks { get; set; }

        public string E_LocalDivName { get; set; }
        public string E_LocalDivDate { get; set; }
        public string E_LocalDivRemarks { get; set; }

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

        public string E_DivName { get; set; }
        public string E_DivDate { get; set; }
        public string E_DivRemarks { get; set; }

        public string E_VPO_POName { get; set; }
        public string E_VPO_PODate { get; set; }
        public string E_VPO_PORemarks { get; set; }
        public string E_Pres_POName { get; set; }
        public string E_Pres_PODate { get; set; }
        public string E_Pres_PORemarks { get; set; }

        public string E_Div2Name { get; set; }

        [DataType(DataType.Date)]
        public string E_Div2Date { get; set; }

        public string E_Div2Remarks { get; set; }

        public string E_Div3Name { get; set; }

        [DataType(DataType.Date)]
        public string E_Div3Date { get; set; }

        public string E_Div3Remarks { get; set; }

        public List<RequestItems> ReqItems { get; set; }
    }

    #endregion Close request : Request approver status
}