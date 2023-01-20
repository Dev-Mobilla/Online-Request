using System.Collections.Generic;

namespace OnlineRequestSystem.Models
{
    public class VPassistantInfo
    {
        public int vp_id { get; set; }
        public string VPAssistant { get; set; }
        public string Division { get; set; }
        public string Zonecode { get; set; }
        public string UserID { get; set; }

        public List<VPassistantInfo> _info { get; set; }
    }
}