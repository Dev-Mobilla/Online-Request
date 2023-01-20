namespace OnlineRequestSystem.Models
{
    public class ReqApproverStatus : RecommendedApproval
    {
        public string Sts_DM_Approver { get; set; }
        public string Sts_DM_Approved_Date { get; set; }
        public string Sts_DM_isApproved { get; set; }

        public string Sts_LocalDiv_Approver { get; set; }
        public string Sts_LocalDiv_Approved_Date { get; set; }
        public string Sts_LocalDiv_isApproved { get; set; }

        public string Sts_AM_Approver { get; set; }
        public string Sts_AM_Approved_Date { get; set; }
        public string Sts_AM_isApproved { get; set; }

        public string Sts_RM_Approver { get; set; }
        public string Sts_RM_Approved_Date { get; set; }
        public string Sts_RM_isApproved { get; set; }

        public string Sts_VPAssistant_Approver { get; set; }
        public string Sts_VPAssistant_Date { get; set; }
        public string Sts_VPAssistant_isApproved { get; set; }

        public string Sts_GM_Approver { get; set; }
        public string Sts_GM_Approved_Date { get; set; }
        public string Sts_GM_isApproved { get; set; }

        public string Sts_Pres_Approver { get; set; }
        public string Sts_Pres_Approved_Date { get; set; }
        public string Sts_Pres_isApproved { get; set; }

        public string Sts_Div1_Approver { get; set; }
        public string Sts_Div1Code { get; set; }
        public string Sts_Div1_Approved_Date { get; set; }
        public string Sts_Div1_isApproved { get; set; }

        public string Sts_Div2_Approver { get; set; }
        public string Sts_Div2Code { get; set; }
        public string Sts_Div2_Approved_Date { get; set; }
        public string Sts_Div2_isApproved { get; set; }

        public string Sts_Div3_Approver { get; set; }
        public string Sts_Div3Code { get; set; }
        public string Sts_Div3_Approved_Date { get; set; }
        public string Sts_Div3_isApproved { get; set; }
        public string Sts_MMD_Approver { get; set; }
        public string Sts_MMD_Approved_Date { get; set; }
        public string Sts_MMD_isApproved { get; set; }
        public string Sts_MMD_Processor { get; set; }
        public string Sts_MMD_Processed_Date { get; set; }
        public string Sts_MMD_isProcessed { get; set; }
        public string Sts_MMD_Deliverer { get; set; }
        public string Sts_MMD_Delivered_Date { get; set; }
        public string Sts_MMD_isDelivered { get; set; }
        public string Sts_MMD_Transitor { get; set; }
        public string Sts_MMD_Transit_Date { get; set; }
        public string Sts_MMD_isTransit { get; set; }

        public string Sts_RM_Receiver { get; set; }
        public string Sts_RM_Received_Date { get; set; }
        public string Sts_RM_isReceived { get; set; }
        public string Sts_RM_Transitor { get; set; }
        public string Sts_RM_Transit_Date { get; set; }
        public string Sts_RM_isTransit { get; set; }
    }

    public class RecommendedApproval
    {
        public int? isDMApproval { get; set; }
        public int? isLocalDivManager { get; set; }
        public int? isAMApproval { get; set; }
        public int? isRMApproval { get; set; }
        public int? isGMApproval { get; set; }
        public int? isDivManApproval { get; set; }
        public int? isDivManApproval2 { get; set; }
        public int? isDivManApproval3 { get; set; }
        public string DivCode1 { get; set; }
        public string DivCode2 { get; set; }
        public string DivCode3 { get; set; }
        public int? isPresidentApproval { get; set; }
    }
}