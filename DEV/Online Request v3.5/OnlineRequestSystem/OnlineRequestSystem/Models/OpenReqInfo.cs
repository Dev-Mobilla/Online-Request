using System.Collections.Generic;

namespace OnlineRequestSystem.Models
{
    public class OpenReqInfo
    {
        public List<OpenReqViewModel> _OpenInfo { get; set; }
        public string returnUrl { get; set; }
        public string office { get; set; }
        public string approver { get; set; }
        public string POurl { get; set; }
    }

    public class CloseReqInfo
    {
        public List<CloseReqViewModel> _CloseInfo { get; set; }
    }
}