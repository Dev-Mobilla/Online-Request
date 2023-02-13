namespace OnlineRequestSystem.Models
{
    public class OpenReqViewModel
    {
        #region Open Request View Models

        public string reqNumber { get; set; }
        public string reqCreator { get; set; }
        public string reqDescription { get; set; }
        public decimal OverallTotalPrice { get; set; }
        public string reqDate { get; set; }
        public string reqStatus { get; set; }
        public string TypeID { get; set; }
        public string TypeName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ZoneCode { get; set; }
        public string Region { get; set; }
        public string isDivRequest { get; set; }
        public string TotalItems { get; set; }
        public string itemDescription { get; set; }
        

        public string DeptCode { get; set; }
        public string forPresident { get; set; }

        public int isApprovedDM { get; set; }
        public int isApprovedLocalDiv { get; set; }
        public int isApprovedAM { get; set; }
        public int isApprovedRM { get; set; }
        public int isApprovedGM { get; set; }
        public int isApprovedVPAssistant { get; set; }
        public int isApprovedDiv1 { get; set; }
        public int isApprovedDiv2 { get; set; }
        public int isApprovedDiv3 { get; set; }
        public int isApprovedPres { get; set; }

        public int reqDM { get; set; }
        public int reqAM { get; set; }
        public int reqRM { get; set; }
        public int reqGM { get; set; }
        public int reqDiv1 { get; set; }
        public int reqDiv2 { get; set; }
        public int reqDiv3 { get; set; }
        public int reqPres { get; set; }
        public string DivCode1 { get; set; }
        public string DivCode2 { get; set; }
        public string DivCode3 { get; set; }

        public int MMD_Processed { get; set; }
        public int MMD_ForDelivery { get; set; }
        public int MMD_InTransit { get; set; }

        #endregion Open Request View Models

        public int RM_Received { get; set; }

        public int RM_Transit { get; set; }

        public int result_DM { get; set; }
        public int result_AM { get; set; }
        public int result_RM { get; set; }
        public int result_Div1 { get; set; }
        public int result_Div2 { get; set; }
        public int result_Div3 { get; set; }
        public int result_GM { get; set; }
        public int result_Pres { get; set; }
    }

    public class Finalresult
    {
        #region Final Results

        public int res_AM { get; set; }
        public int res_RM { get; set; }
        public int res_Div1 { get; set; }
        public int res_Div2 { get; set; }
        public int res_Div3 { get; set; }
        public int res_GM { get; set; }
        public int res_Pres { get; set; }

        #endregion Final Results
    }
}