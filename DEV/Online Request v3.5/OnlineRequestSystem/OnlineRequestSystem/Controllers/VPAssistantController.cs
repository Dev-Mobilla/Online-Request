using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class VPAssistantController : Controller
    {
        private Helper help = new Helper();

        [Route("branch-requests")]
        public ActionResult BranchRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            var Info = new OpenReqInfo();
            var GetMethod = new OpenRequestController();
            var db = new ORtoMySql();

            using (var conn = db.getConnection())
            {
                var Culture = new CultureInfo("en-US", false).TextInfo;
                var cmd = conn.CreateCommand();
                var data = new List<OpenReqViewModel>();
               
                string zone = string.Empty;

                if ((new[] { "VISMIN", "VISAYAS", "MINDANAO" }).Contains(ss.s_zonecode))
                    zone = "VIS";
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "openReq_VPAssistantBranch";
                cmd.Parameters.AddWithValue("@_type", zone);

                conn.Open();
                var read = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (read.Read())
                {
                    var o = new OpenReqViewModel();
                    o.reqNumber = read["reqNumber"].ToString().Trim();
                    o.reqCreator = Culture.ToTitleCase(read["reqCreator"].ToString().Trim().ToLower());
                    o.reqDescription = read["reqDescription"].ToString().Trim();
                    o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(read["reqDate"].ToString()));
                    o.TypeID = read["TypeID"].ToString().Trim();
                    o.TotalItems = read["TotalCount"].ToString().Trim();
                    o.itemDescription = read["Description"].ToString().Trim();
                    string typeName = "";
                    typeName = help.GetTypeName(o.TypeID);
                    if (typeName.Length > 17)
                    {
                        o.TypeName = typeName.Substring(0, 17) + "..";
                    }
                    else
                    {
                        o.TypeName = typeName;
                    }

                    o.BranchCode = read["BranchCode"].ToString().Trim();
                    o.ZoneCode = read["Zonecode"].ToString().Trim();
                    o.Region = read["Region"].ToString().Trim().ToUpper();
                    o.isDivRequest = read["isDivRequest"].ToString().Trim();
                    o.DeptCode = read["DeptCode"].ToString().Trim();
                    o.forPresident = read["forPresident"].ToString().Trim();
                    o.BranchName = Culture.ToTitleCase(GetMethod.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");

                    if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                    {
                        o.isApprovedAM = Convert.ToInt32(read["isApprovedAM"]);
                        o.isApprovedRM = Convert.ToInt32(read["isApprovedRM"]);
                        o.reqAM = Convert.ToInt32(read["isAMApproval"]);
                        o.reqRM = Convert.ToInt32(read["isRMApproval"]);
                    }

                    o.isApprovedLocalDiv = Convert.ToInt32(read["isApprovedLocalDiv"]);

                    o.isApprovedDM = Convert.ToInt32(read["isApprovedDM"]);
                    o.isApprovedGM = Convert.ToInt32(read["isApprovedGM"]);
                    o.isApprovedVPAssistant = Convert.ToInt32(read["isApprovedVPAssistant"]);
                    o.isApprovedDiv1 = Convert.ToInt32(read["isApprovedDiv1"]);
                    o.isApprovedDiv2 = Convert.ToInt32(read["isApprovedDiv2"]);
                    o.isApprovedDiv3 = Convert.ToInt32(read["isApprovedDiv3"]);
                    o.isApprovedPres = Convert.ToInt32(read["isApprovedPres"]);

                    o.DivCode1 = read["DivCode1"].ToString().Trim();
                    o.DivCode2 = read["DivCode2"].ToString().Trim();
                    o.DivCode3 = read["DivCode3"].ToString().Trim();
                    o.reqDM = Convert.ToInt32(read["isDMApproval"]);
                    o.reqGM = Convert.ToInt32(read["isGMApproval"]);
                    o.reqDiv1 = Convert.ToInt32(read["isDivManApproval"]);
                    o.reqDiv2 = Convert.ToInt32(read["isDivManApproval2"]);
                    o.reqDiv3 = Convert.ToInt32(read["isDivManApproval3"]);
                    o.reqPres = Convert.ToInt32(read["isPresidentApproval"]);
                    o.MMD_Processed = Convert.ToInt32(read["isMMDProcessed"]);
                    o.MMD_ForDelivery = Convert.ToInt32(read["isDelivered"]);
                    o.MMD_InTransit = Convert.ToInt32(read["isMMDTransit"]);

                    data.Add(o);
                }

                Info.office = "branch";
                Info.returnUrl = "branch-requests";
                Info._OpenInfo = data;
            }
            return View(Info);
        }
    }
}