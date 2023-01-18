using System;

namespace OnlineRequestSystem.Models
{
    public class Resp
    {
        public string resCode { get; set; }
        public string resMsg { get; set; }
    }

    #region ORSession - Handles user data

    public class ORSession
    {
        public String s_fullname { get; set; }
        public String s_usr_id { get; set; }
        public String s_res_id { get; set; }
        public String s_comp { get; set; }
        public String s_costcenter { get; set; }
        public String s_zonecode { get; set; }
        public String s_DivCode { get; set; }
        public String s_DivAcro { get; set; }
        public String s_Division { get; set; }
        public String s_DivManager { get; set; }
        public String s_DepartManager { get; set; }
        public String s_job_title { get; set; }
        public String s_task { get; set; }
        public String s_blocked { get; set; }
        public String s_bedrnm { get; set; }
        public String s_area { get; set; }
        public String s_region { get; set; }
        public String s_areaCode { get; set; }
        public String s_regionCode { get; set; }
        public String s_BMName { get; set; }
        public String s_AMName { get; set; }
        public String s_LPTLName { get; set; }
        public String s_RMName { get; set; }

        public string s_LayoutControl { get; set; }
        public int? s_login { get; set; }
        public int s_isDivisionApprover { get; set; }
        public string s_DivApprover_ResID { get; set; }
        public string s_DivisionID { get; set; }
        public int s_isVPAssistant { get; set; }

        public int isDivRequest { get; set; }
        public string s_DepartmentCode { get; set; }
        public string s_Department_ResID { get; set; }
        public int s_isDepartmentApprover { get; set; }
        public int s_SDCApprover { get; set; }
        public int s_MMD { get; set; }
    }

    #endregion ORSession - Handles user data

    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}