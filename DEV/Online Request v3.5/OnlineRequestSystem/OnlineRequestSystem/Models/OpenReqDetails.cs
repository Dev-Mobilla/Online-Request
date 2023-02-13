namespace OnlineRequestSystem.Models
{
    public class RequestItemTable
    {
        public string ItemName { get; set; }
        public string ItemQty { get; set; }
        public string ItemUnitCost { get; set; }
        public string TotalCost { get; set; }
        public string TotalPrice { get; set; }
    }

    public class IssuanceSlipModel
    {
        public int qty { get; set; }
        public string unit { get; set; }
        public string description { get; set; }
    }
}