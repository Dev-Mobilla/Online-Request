using System.Collections.Generic;

namespace OnlineRequestSystem.Models
{
    public class ListOfSDCResponse : Resp
    {
        public List<SDCInfo> ListOfSDC { get; set; }
    }

    public class SDCInfo
    {
        public string Region { get; set; }
        public string RM { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ZoneCode { get; set; }
    }
}