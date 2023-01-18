using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRequestSystem.Areas.Administrator.Models
{
    public class RequestModels
    {
        public List<RequestModels> ReqList { get; set;  }
        public int id { get; set; }
        public string reqNumber { get; set; }
        public string reqDate { get; set; }
        public string reqCreator { get; set; }
        public string TypeID { get; set; }
        public string BranchCode { get; set; }
        public string Region { get; set; }
        public string Area { get; set; }
        public string AreaCode { get; set; }
        public string DivCode { get; set; }
        public string ZoneCode { get; set; }
        public string Status { get; set; }
        public string SysCreated { get; set; }
        public string SysCreator { get; set; }
        public string ForPresident { get; set; }

    }
}