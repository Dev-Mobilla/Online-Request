using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRequestSystem.Models
{
    public class ListOfItemsResponse : Resp
    {
        public List<ItemsInfo> ListOfItems { get; set; }
    }
    public class ItemsInfo
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemPrice { get; set; }
    }
}