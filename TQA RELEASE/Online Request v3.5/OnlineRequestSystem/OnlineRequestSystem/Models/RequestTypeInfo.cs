using System.Collections.Generic;
using System.Web.Mvc;

namespace OnlineRequestSystem.Models
{
    public class RequestTypeInfo
    {
        public List<AddRequestType> _RTInfo { get; set; }

        public string RequestType { get; set; }
        public bool isAMApproval { get; set; }
        public bool isRMApproval { get; set; }
        public bool isDivManApproval { get; set; }
        public string DivCode1 { get; set; }
        public bool isDivManApproval2 { get; set; }
        public string DivCode2 { get; set; }
        public bool isDivManApproval3 { get; set; }
        public string DivCode3 { get; set; }
        public bool isGMApproval { get; set; }
        public bool isPresidentApproval { get; set; }

        //For Selecting Type

        public IEnumerable<SelectListItem> ListOfDivision { get; set; }
        public IEnumerable<SelectListItem> ListReqtype { get; set; }
    }

    public class AddRequestType
    {
        public int IDforUpdate { get; set; }
        public string RequestType { get; set; }
        public int isAMApproval { get; set; }
        public int isRMApproval { get; set; }
        public int isGMApproval { get; set; }
        public int isDivManApproval { get; set; }
        public int isDivManApproval2 { get; set; }
        public int isDivManApproval3 { get; set; }
        public int isPresidentApproval { get; set; }
        public string DivCode1 { get; set; }
        public string DivCode2 { get; set; }
        public string DivCode3 { get; set; }
        public string DivName1 { get; set; }
        public string DivName2 { get; set; }
        public string DivName3 { get; set; }
    }

    public class MenuModel
    {
        public List<RequestTypeMenu> RequestMenu { get; set; }
    }

    public class GetRequestType
    {
        public List<RequestTypeMenu> SelectType { get; set; }
    }

    public class RequestTypeMenu
    {
        public int IDforUpdate { get; set; }
        public string RequestType { get; set; }
        public int isAMApproval { get; set; }
        public int isRMApproval { get; set; }
        public int isGMApproval { get; set; }
        public int isDivManApproval { get; set; }
        public string Approval_DivCode1 { get; set; }
        public string Approval_DivCode2 { get; set; }
        public string Approval_DivCode3 { get; set; }
        public int isDivManApproval2 { get; set; }
        public int isDivManApproval3 { get; set; }
        public int isPresidentApproval { get; set; }
    }
}