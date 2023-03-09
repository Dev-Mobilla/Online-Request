using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Dataset;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineRequestSystem.Controllers
{
    public class OpenRequestController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OpenRequestController));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";
        private Helper help = new Helper();

        [Route("open-requests")]
        public ActionResult ViewOpenRequest()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                string baseReturnurl = "open-requests";
                var Info = new OpenReqInfo();
                var Culture = new CultureInfo("en-US", false).TextInfo;
                var db = new ORtoMySql();
                var que = new OpenQueries();
                using (var conn = db.getConnection())
                {
                    var OpenReqList = new List<OpenReqViewModel>();
                    using (var cmd = conn.CreateCommand())
                    {
                        if (ss.s_costcenter == "0001MMD" || ss.s_costcenter == "0002MMD")
                        {
                            return RedirectToAction("MMD_BranchRequests");
                        }
                        if (ss.s_job_title == "AREA MANAGER")
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_AM";
                            cmd.Parameters.AddWithValue("@_area", ss.s_area);
                        }
                        else if (ss.s_job_title == "REGIONAL MAN")
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_RM";
                            cmd.Parameters.AddWithValue("@_region", ss.s_region);
                        }
                        else if (ss.s_isDepartmentApprover == 1)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_DeptApprover";
                            cmd.Parameters.AddWithValue("@_costcenter", ss.s_costcenter);
                        }
                        else if (ss.s_isDivisionApprover == 1 && ss.s_task != "GMO-GENMAN" && ss.s_usr_id != "LHUI1011873")
                        {
                            return RedirectToAction("Div_HORequests");
                        }
                        else if (ss.s_task == "GMO-GENMAN")
                        {
                            return RedirectToAction("GMO_BranchRequests");
                        }
                        else if (ss.s_usr_id == "LHUI1011873")
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_Pres";
                        }
                        else
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_NonApprover";
                            cmd.Parameters.AddWithValue("@_region", ss.s_region);
                            cmd.Parameters.AddWithValue("@_comp", ss.s_comp);
                        }

                        cmd.Parameters.AddWithValue("@_fullname", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@_zonecode", ss.s_zonecode);
                        conn.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var o = new OpenReqViewModel();

                                o.reqNumber = rdr["reqNumber"].ToString().Trim();
                                o.reqCreator = Culture.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                o.reqDescription = rdr["reqDescription"].ToString().Trim();

                                if (ss.s_usr_id == "LHUI1011873")
                                {
                                    o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                                }

                                o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                                o.TypeID = rdr["TypeID"].ToString().Trim();

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

                                o.TotalItems = Convert.ToString(rdr["TotalCount"].ToString().Trim());
                                o.itemDescription = rdr["Description"].ToString().Trim();
                                o.BranchCode = rdr["BranchCode"].ToString().Trim();
                                o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                                o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                                o.reqStatus = rdr["reqStatus"].ToString().Trim();
                                o.DeptCode = rdr["DeptCode"].ToString().Trim();
                                o.Region = rdr["Region"].ToString().Trim().ToUpper();
                                if (o.Region == "HO")
                                {
                                    o.Region = GetOR_DivisionName(o.DeptCode).ToUpper();
                                }
                                o.forPresident = rdr["forPresident"].ToString().Trim();
                                if (o.isDivRequest == "1")
                                {
                                    o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                                }
                                else
                                {
                                    o.BranchName = Culture.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                                }

                                if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                                {
                                    o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                                    o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                                    o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                                    o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                                }

                                o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                                o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                                o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                                o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                                o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                                o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                                o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                                o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                                o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                                o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                                o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                                o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                                o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                                o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                                o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                                o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                                o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                                o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                                o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                                o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                                if (ss.s_usr_id == "LHUI1011873")
                                {
                                    o.VPO_PO_Approved = (rdr["isVPO_PO_Approved"] is DBNull) ? 0 : Convert.ToInt32(rdr["isVPO_PO_Approved"]);

                                    o.Pres_PO_Approved = (rdr["isPres_PO_Approved"] is DBNull) ? 0 : Convert.ToInt32(rdr["isPres_PO_Approved"]);
                                }

                                OpenReqList.Add(o);
                            }
                            rdr.Close();
                            conn.Close();
                        }
                    }

                    Info._OpenInfo = OpenReqList;
                }

                ViewBag.headTxt = "OPEN";
                Info.returnUrl = baseReturnurl;
                return View(Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("PO-requests")]
        public ActionResult ViewPORequest()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                string baseReturnurl = "PO-requests";
                var Info = new OpenReqInfo();
                var Culture = new CultureInfo("en-US", false).TextInfo;
                var db = new ORtoMySql();
                var que = new OpenQueries();
                using (var conn = db.getConnection())
                {
                    var OpenReqList = new List<OpenReqViewModel>();
                    using (var cmd = conn.CreateCommand())
                    {
                        if (ss.s_usr_id == "LHUI1011873")
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "openReq_Pres";
                        }
                        else
                        {
                            string errMsg = "User: " + ss.s_usr_id + " is not recognized as PO approver.";
                            log.Fatal(errMsg);
                            ViewBag.Error = errMsg;
                            return View("Error");
                        }

                        cmd.Parameters.AddWithValue("@_fullname", ss.s_fullname);
                        cmd.Parameters.AddWithValue("@_zonecode", ss.s_zonecode);
                        conn.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var o = new OpenReqViewModel();

                                o.reqNumber = rdr["reqNumber"].ToString().Trim();
                                o.reqCreator = Culture.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                o.reqDescription = rdr["reqDescription"].ToString().Trim();
                                o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                                o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                                o.TypeID = rdr["TypeID"].ToString().Trim();

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

                                o.TotalItems = Convert.ToString(rdr["TotalCount"].ToString().Trim());
                                o.itemDescription = rdr["Description"].ToString().Trim();
                                o.BranchCode = rdr["BranchCode"].ToString().Trim();
                                o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                                o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                                o.reqStatus = rdr["reqStatus"].ToString().Trim();
                                o.DeptCode = rdr["DeptCode"].ToString().Trim();
                                o.Region = rdr["Region"].ToString().Trim().ToUpper();
                                if (o.Region == "HO")
                                {
                                    o.Region = GetOR_DivisionName(o.DeptCode).ToUpper();
                                }
                                o.forPresident = rdr["forPresident"].ToString().Trim();
                                if (o.isDivRequest == "1")
                                {
                                    o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                                }
                                else
                                {
                                    o.BranchName = Culture.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                                }

                                if (!(new[] { "001", "002" }).Contains(o.BranchCode))
                                {
                                    o.isApprovedAM = Convert.ToInt32(rdr["isApprovedAM"]);
                                    o.isApprovedRM = Convert.ToInt32(rdr["isApprovedRM"]);
                                    o.reqAM = Convert.ToInt32(rdr["isAMApproval"]);
                                    o.reqRM = Convert.ToInt32(rdr["isRMApproval"]);
                                }

                                o.isApprovedLocalDiv = Convert.ToInt32(rdr["isApprovedLocalDiv"]);
                                o.isApprovedDM = Convert.ToInt32(rdr["isApprovedDM"]);
                                o.isApprovedGM = Convert.ToInt32(rdr["isApprovedGM"]);
                                o.isApprovedVPAssistant = Convert.ToInt32(rdr["isApprovedVPAssistant"]);
                                o.isApprovedDiv1 = Convert.ToInt32(rdr["isApprovedDiv1"]);
                                o.isApprovedDiv2 = Convert.ToInt32(rdr["isApprovedDiv2"]);
                                o.isApprovedDiv3 = Convert.ToInt32(rdr["isApprovedDiv3"]);
                                o.isApprovedPres = Convert.ToInt32(rdr["isApprovedPres"]);

                                o.DivCode1 = rdr["DivCode1"].ToString().Trim();
                                o.DivCode2 = rdr["DivCode2"].ToString().Trim();
                                o.DivCode3 = rdr["DivCode3"].ToString().Trim();
                                o.reqDM = Convert.ToInt32(rdr["isDMApproval"]);
                                o.reqGM = Convert.ToInt32(rdr["isGMApproval"]);
                                o.reqDiv1 = Convert.ToInt32(rdr["isDivManApproval"]);
                                o.reqDiv2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                                o.reqDiv3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                                o.reqPres = Convert.ToInt32(rdr["isPresidentApproval"]);
                                o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                                o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                                o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);
                                o.VPO_PO_Approved = (rdr["isVPO_PO_Approved"] is DBNull) ? 0 : Convert.ToInt32(rdr["isVPO_PO_Approved"]);
                                o.Pres_PO_Approved = (rdr["isPres_PO_Approved"] is DBNull) ? 0 : Convert.ToInt32(rdr["isPres_PO_Approved"]);

                                OpenReqList.Add(o);
                            }
                        }

                        if (ss.s_usr_id == "LHUI1011873")
                        {
                            var cmd2 = conn.CreateCommand();
                            conn.Open();

                            foreach (var item in OpenReqList.ToList())
                            {
                                cmd2.CommandText = "SELECT COUNT(*) AS numOfNotify FROM OnlineRequest.storedComments WHERE reqNumber = @reqNo AND (isViewedBy IS NULL OR isViewedOn IS NULL) AND commCreator <> 'Michael L. Lhuillier'";
                                cmd2.Parameters.AddWithValue("@reqNo", item.reqNumber);
                                cmd2.Parameters.AddWithValue("@viewer", ss.s_fullname);
                                using (var read = cmd2.ExecuteReader())
                                {
                                    cmd2.Parameters.Clear();
                                    read.Read();
                                    item.numOfNotifs = Convert.ToInt32(read["numOfNotify"]);
                                }
                            }
                        }

                        Info._OpenInfo = OpenReqList;
                    }
                }

                ViewBag.headTxt = "PO";
                Info.returnUrl = baseReturnurl;
                Info.POurl = "PO";
                return View("ViewOpenRequest", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }


        [Route("request-details")]
        public ActionResult RequestDetails(string Region, string ZoneCode, string ReqNo, string BranchCode, CreateReqModels model, string retUrl, string office, string PO)
        {
            ViewBag.ReqNo = ReqNo;
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var itemLists = new List<RequestItems>();
                var commLists = new List<ShowAllComments>();
                var db = new ORtoMySql();
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var MMD = new MMDController();

                using (var con = db.getConnection())
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @ReqNo1";
                        cmd.Parameters.AddWithValue("@ReqNo1", ReqNo);
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            string TypeName = "";
                            if (rdr.HasRows)
                            {
                                #region Read Online Request Open

                                rdr.Read();
                                model.RequestNo = rdr["reqNumber"].ToString().Trim();
                                model.ReqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                                model.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                TypeName = rdr["TypeID"].ToString().Trim();
                                ViewBag.requestName = getRequestType(TypeName);
                                model.RequestType = rdr["TypeID"].ToString().Trim();
                                model.Description = rdr["reqDescription"].ToString().Trim();
                                model.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
                                model.BranchCode = rdr["BranchCode"].ToString().Trim();
                                model.DivisionCode = rdr["DivCode"].ToString().Trim();
                                model.Area = rdr["Area"].ToString().Trim();
                                model.reqStatus = rdr["reqStatus"].ToString().Trim();
                                model.Region = rdr["Region"].ToString().Trim();
                                model.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                                model.isDivRequest = Convert.ToInt32(rdr["isDivRequest"]);
                                model.req_DeptCode = rdr["DeptCode"].ToString().Trim();
                                model.Bednrm = GetRequestDetails_DivOrBranchName(model.isDivRequest, model.BranchCode, Region, ZoneCode, model.req_DeptCode);
                                model.forPresident = Convert.ToInt32(rdr["forPresident"]);
                                model.hasDiagnostic = help.DiagnosticCheck(ReqNo);

                                string req = model.RequestNo;
                                string reqq = req[req.Length - 2].ToString() + req[req.Length - 1].ToString();
                                if (reqq == "-A")
                                {
                                    model.reqTrigger = "1";
                                }
                                else
                                {
                                    model.reqTrigger = "0";
                                }

                                #endregion Read Online Request Open
                            }
                            else
                            {
                                return RedirectToAction("ViewOpenRequest");
                            }
                            con.Close();
                            rdr.Close();
                        }

                        string RequestApproverStatus = "SELECT * FROM OnlineRequest.requestApproverStatus WHERE reqNumber = @ReqNo4";
                        cmd.Parameters.AddWithValue("@ReqNo4", ReqNo);
                        cmd.CommandText = RequestApproverStatus;
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();

                                #region Read Approver Status

                                model.Sts_DM_Approver = rdr["DM_Approver"].ToString().Trim();
                                model.Sts_DM_Approved_Date = rdr["DM_Approved_Date"].ToString().Trim();
                                model.Sts_DM_isApproved = rdr["isApprovedDM"].ToString().Trim();
                                model.Sts_LocalDiv_Approver = rdr["LocalDiv_Approver"].ToString().Trim();
                                model.Sts_LocalDiv_Approved_Date = rdr["LocalDiv_Approved_Date"].ToString().Trim();
                                model.Sts_LocalDiv_isApproved = rdr["isApprovedLocalDiv"].ToString().Trim();
                                model.Sts_AM_Approver = rdr["AM_Approver"].ToString().Trim();
                                model.Sts_AM_Approved_Date = rdr["AM_Approved_Date"].ToString().Trim();
                                model.Sts_AM_isApproved = rdr["isApprovedAM"].ToString().Trim();
                                model.Sts_RM_Approver = rdr["RM_Approver"].ToString().Trim();
                                model.Sts_RM_Approved_Date = rdr["RM_Approved_Date"].ToString().Trim();
                                model.Sts_RM_isApproved = rdr["isApprovedRM"].ToString().Trim();
                                model.Sts_VPAssistant_Approver = rdr["VPAssistant_Approver"].ToString().Trim();
                                model.Sts_VPAssistant_Date = rdr["VPAssistant_Approved_Date"].ToString().Trim();
                                model.Sts_VPAssistant_isApproved = rdr["isApprovedVPAssistant"].ToString().Trim();

                                model.Sts_VPO_PO_Approver = rdr["VPO_PO_Approver"].ToString().Trim();
                                model.Sts_VPO_PO_Date = rdr["VPO_PO_Approved_Date"].ToString().Trim();
                                model.Sts_VPO_PO_isApproved = rdr["isVPO_PO_Approved"].ToString().Trim();
                                model.Sts_Pres_PO_Approver = rdr["Pres_PO_Approver"].ToString().Trim();
                                model.Sts_Pres_PO_Date = rdr["Pres_PO_Approved_Date"].ToString().Trim();
                                model.Sts_Pres_PO_isApproved = rdr["isPres_PO_Approved"].ToString().Trim();
                                model.PO_Approved = rdr["isPO_Approved"].ToString().Trim();

                                model.Sts_GM_Approver = rdr["GM_Approver"].ToString().Trim();
                                model.Sts_GM_Approved_Date = rdr["GM_Approved_Date"].ToString().Trim();
                                model.Sts_GM_isApproved = rdr["isApprovedGM"].ToString().Trim();
                                model.Sts_Pres_Approver = rdr["Pres_Approver"].ToString().Trim();
                                model.Sts_Pres_Approved_Date = rdr["Pres_Approved_Date"].ToString().Trim();
                                model.Sts_Pres_isApproved = rdr["isApprovedPres"].ToString().Trim();

                                model.Sts_Div1_Approver = rdr["Div_Approver1"].ToString().Trim();
                                model.Sts_Div1Code = rdr["DivCode1"].ToString().Trim();
                                model.Sts_Div1_Approved_Date = rdr["Div_Approved_Date1"].ToString().Trim();
                                model.Sts_Div1_isApproved = rdr["isApprovedDiv1"].ToString().Trim();
                                model.Sts_Div2_Approver = rdr["Div_Approver2"].ToString().Trim();
                                model.Sts_Div2Code = rdr["DivCode2"].ToString().Trim();
                                model.Sts_Div2_Approved_Date = rdr["Div_Approved_Date2"].ToString().Trim();
                                model.Sts_Div2_isApproved = rdr["isApprovedDiv2"].ToString().Trim();
                                model.Sts_Div3_Approver = rdr["Div_Approver3"].ToString().Trim();
                                model.Sts_Div3Code = rdr["DivCode3"].ToString().Trim();
                                model.Sts_Div3_Approved_Date = rdr["Div_Approved_Date3"].ToString().Trim();
                                model.Sts_Div3_isApproved = rdr["isApprovedDiv3"].ToString().Trim();
                                model.Sts_MMD_Approver = rdr["MMD_Approver"].ToString().Trim();
                                model.Sts_MMD_Approved_Date = rdr["MMD_Approved_Date"].ToString().Trim();
                                model.Sts_MMD_isApproved = rdr["isApprovedMMD"].ToString().Trim();
                                model.Sts_MMD_Processor = rdr["MMD_Processor"].ToString().Trim();
                                model.Sts_MMD_Processed_Date = rdr["MMD_Processed_Date"].ToString().Trim();
                                model.Sts_MMD_isProcessed = rdr["isMMDProcessed"].ToString().Trim();
                                model.Sts_MMD_Deliverer = rdr["MMD_Deliverer"].ToString().Trim();
                                model.Sts_MMD_Delivered_Date = rdr["MMD_Delivered_Date"].ToString().Trim();
                                model.Sts_MMD_isDelivered = rdr["isDelivered"].ToString().Trim();
                                model.Sts_MMD_Transitor = rdr["MMD_Transitor"].ToString().Trim();
                                model.Sts_MMD_Transit_Date = rdr["MMD_Transit_Date"].ToString().Trim();
                                model.Sts_MMD_isTransit = rdr["isMMDTransit"].ToString().Trim();
                                model.Sts_RM_Receiver = rdr["RM_Receiver"].ToString().Trim();
                                model.Sts_RM_Received_Date = rdr["RM_Received_Date"].ToString().Trim();
                                model.Sts_RM_isReceived = rdr["isRMReceived"].ToString().Trim();
                                model.Sts_RM_Transitor = rdr["RM_Transitor"].ToString().Trim();
                                model.Sts_RM_Transit_Date = rdr["RM_Transit_Date"].ToString().Trim();
                                model.Sts_RM_isTransit = rdr["isRMTransit"].ToString().Trim();

                                if (model.Sts_MMD_isProcessed == "1")
                                {

                                }


                                #endregion Read Approver Status
                            }
                            con.Close();
                            rdr.Close();
                        }
                        //cmd.CommandText = "SELECT * FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo2 ORDER BY id ASC";
                        cmd.CommandText = "SELECT * FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo2 ORDER BY itemDescription ASC";
                        cmd.Parameters.AddWithValue("@ReqNo2", ReqNo);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var i = new RequestItems();
                                i.ItemDescription = rdr["itemDescription"].ToString().Trim();
                                i.ItemQty = rdr["qty"].ToString().Trim();
                                i.TotalPrice = rdr["TotalPrice"].ToString().Trim();
                                i.ItemUnit = rdr["unit"].ToString().Trim();
                                i.ItemStatus = rdr["itemStatus"].ToString().Trim();
                                i.isCheckedSDC = Convert.ToInt32(rdr["isCheckedSDC"].ToString().Trim());
                                i.actualQtySDC = rdr["actualQtySDC"].ToString().Trim();
                                if (string.IsNullOrEmpty(i.actualQtySDC))
                                {
                                    i.actualQtySDC = "0";
                                }

                                i.isCheckedMMD = Convert.ToInt32(rdr["isCheckedMMD"].ToString().Trim());
                                i.actualQtyMMD = rdr["actualQtyMMD"].ToString().Trim();
                                if (string.IsNullOrEmpty(i.actualQtyMMD))
                                {
                                    i.actualQtyMMD = "0";
                                }

                                i.isCheckedBranch = Convert.ToInt32(rdr["isCheckedBranch"].ToString().Trim());
                                i.actualQtyBranch = rdr["actualQtyBranch"].ToString().Trim();
                                if (string.IsNullOrEmpty(i.actualQtyBranch))
                                {
                                    i.actualQtyBranch = "0";
                                }

                                i.isCheckedDiv = Convert.ToInt32(rdr["isCheckedDiv"].ToString().Trim());
                                i.actualQtyDiv = rdr["actualQtyDiv"].ToString();
                                if (string.IsNullOrEmpty(i.actualQtyDiv))
                                {
                                    i.actualQtyDiv = "0";
                                }

                                i.MMDstatus = FontAwesomeSelector(rdr["MMDstatus"].ToString().Trim());
                                i.SDCStatus = FontAwesomeSelector(rdr["SDCStatus"].ToString().Trim());
                                i.BranchStatus = FontAwesomeSelector(rdr["BranchStatus"].ToString().Trim());
                                i.DivStatus = FontAwesomeSelector(rdr["DivStatus"].ToString().Trim());

                                i.StatusOfStock = rdr["StatusOfStock"].ToString();

                                itemLists.Add(i);
                            }
                            con.Close();
                            rdr.Close();
                        }

                        cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Escalation WHERE reqNumber = @ReqNo3";
                        cmd.Parameters.AddWithValue("@ReqNo3", ReqNo);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                #region Read Escalation Table

                                rdr.Read();
                                model.E_DMName = rdr["EscalationDM_Name"].ToString().Trim();
                                model.E_DMDate = rdr["EscalationDM_Date"].ToString().Trim();
                                model.E_DMRemarks = rdr["EscalationDM_Remarks"].ToString().Trim();
                                model.E_AMName = rdr["EscalationAM_Name"].ToString().Trim();
                                model.E_AMDate = rdr["EscalationAM_Date"].ToString().Trim();
                                model.E_AMRemarks = rdr["EscalationAM_Remarks"].ToString().Trim();
                                model.E_LocalDivName = rdr["EscalationLocalDiv_Name"].ToString().Trim();
                                model.E_LocalDivDate = rdr["EscalationLocalDiv_Date"].ToString().Trim();
                                model.E_LocalDivRemarks = rdr["EscalationLocalDiv_Remarks"].ToString().Trim();
                                model.E_RMName = rdr["EscalationRM_Name"].ToString().Trim();
                                model.E_RMDate = rdr["EscalationRM_Date"].ToString().Trim();
                                model.E_RMRemarks = rdr["EscalationRM_Remarks"].ToString().Trim();
                                model.E_VPAssistantName = rdr["EscalationVPAssistant"].ToString().Trim();
                                model.E_VPAssistantDate = rdr["EscalationVPAssistant_Date"].ToString().Trim();
                                model.E_VPAssistantRemarks = rdr["EscalationVPAssistant_Remarks"].ToString().Trim();
                                model.E_GMName = rdr["EscalationGM_Name"].ToString().Trim();
                                model.E_GMDate = rdr["EscalationGM_Date"].ToString().Trim();
                                model.E_GMRemarks = rdr["EscalationGM_Remarks"].ToString().Trim();
                                model.E_DivName = rdr["EscalationDiv_Name"].ToString().Trim();
                                model.E_DivDate = rdr["EscalationDiv_Date"].ToString().Trim();
                                model.E_DivRemarks = rdr["EscalationDiv_Remarks"].ToString().Trim();
                                model.E_Div2Name = rdr["EscalationDiv2_Name"].ToString().Trim();
                                model.E_Div2Date = rdr["EscalationDiv2_Date"].ToString().Trim();
                                model.E_Div2Remarks = rdr["EscalationDiv2_Remarks"].ToString().Trim();
                                model.E_Div3Name = rdr["EscalationDiv3_Name"].ToString().Trim();
                                model.E_Div3Date = rdr["EscalationDiv3_Date"].ToString().Trim();
                                model.E_Div3Remarks = rdr["EscalationDiv3_Remarks"].ToString().Trim();
                                model.E_PresName = rdr["EscalationPres_Name"].ToString().Trim();
                                model.E_PresDate = rdr["EscalationPres_Date"].ToString().Trim();
                                model.E_PresRemarks = rdr["EscalationPres_Remarks"].ToString().Trim();
                                model.E_VPO_POName = rdr["EscalationVPO_PO_Name"].ToString().Trim();
                                model.E_VPO_PODate = rdr["EscalationVPO_PO_Date"].ToString().Trim();
                                model.E_VPO_PORemarks = rdr["EscalationVPO_PO_Remarks"].ToString().Trim();
                                model.E_Pres_POName = rdr["EscalationPres_PO_Name"].ToString().Trim();
                                model.E_Pres_PODate = rdr["EscalationPres_PO_Date"].ToString().Trim();
                                model.E_Pres_PORemarks = rdr["EscalationPres_PO_Remarks"].ToString().Trim();

                                #endregion Read Escalation Table
                            }
                            con.Close();
                            rdr.Close();
                        }

                        cmd.CommandText = "SELECT * FROM OnlineRequest.storedComments WHERE reqNumber = @ReqNo ORDER BY commcreated DESC";
                        cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var i = new ShowAllComments();
                                i.comments = System.Web.HttpUtility.UrlDecode(rdr["comments"].ToString().Trim());
                                i.commCreator = rdr["commCreator"].ToString().Trim();
                                i.commCreatorID = rdr["commCreatorID"].ToString().Trim();
                                i.commcreated = rdr["commcreated"].ToString().Trim();

                                commLists.Add(i);
                            }
                            con.Close();
                            rdr.Close();
                        }

                        model.ShowComments = commLists;

                        cmd.CommandText = "SELECT * FROM requestType WHERE TypeID = @TypeID";
                        cmd.Parameters.AddWithValue("@TypeID", model.RequestType);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                model.isDMApproval = Convert.ToInt32(rdr["isDMApproval"]);
                                model.isAMApproval = Convert.ToInt32(rdr["isAMApproval"]);
                                model.isRMApproval = Convert.ToInt32(rdr["isRMApproval"]);
                                model.isGMApproval = Convert.ToInt32(rdr["isGMApproval"]);
                                model.isDivManApproval = Convert.ToInt32(rdr["isDivManApproval"]);
                                model.isDivManApproval2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                                model.isDivManApproval3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                                model.DivCode1 = rdr["DivCode1"].ToString().Trim();
                                model.DivCode2 = rdr["DivCode2"].ToString().Trim();
                                model.DivCode3 = rdr["DivCode3"].ToString().Trim();
                                model.isPresidentApproval = Convert.ToInt32(rdr["isPresidentApproval"]);
                            }

                            con.Close();
                            rdr.Close();
                        }
                        model.ReqItems = itemLists;
                        TempData["reqItems"] = itemLists;

                        // need to do some functions to disable and enable the 
                        // checkbox , dropdown and serve option for
                        // MMD, SDC, Branch and Division users

                        if (ss.s_MMD == 1)
                        {
                            ViewBag.MMD = "dropdown";
                            ViewBag.hover = "hoverable";
                            if (model.reqCreator == ss.s_fullname)
                            {
                                ViewBag.Division = "dropdown";
                                ViewBag.hover = "hoverable";
                            }
                        }
                        else
                        {
                            ViewBag.MMD = "";
                        }

                        if (ss.s_SDCApprover == 1 && model.Sts_MMD_isDelivered == "1")
                        {
                            ViewBag.SDC = "dropdown";
                            ViewBag.hover = "hoverable";
                        }
                        else
                        {
                            ViewBag.SDC = "";
                        }


                        if ((new[] { "001", "002" }).Contains(ss.s_comp) && ss.s_MMD != 1 && model.Sts_MMD_isDelivered == "1")
                        {
                            ViewBag.Division = "dropdown";
                            ViewBag.hover = "hoverable";
                        }
                        else
                        {
                            ViewBag.Division = "";
                        }

                        if (!(new[] { "001", "002" }).Contains(ss.s_comp) && ss.s_SDCApprover == 0 || ss.s_fullname == model.reqCreator)
                        {
                            ViewBag.Branch = "dropdown";
                            ViewBag.hover = "hoverable";
                        }
                        else
                        {
                            ViewBag.Branch = "";
                        }

                        if (ss.s_usr_id == "LHUI1011873" || ss.s_MMD == 1)
                        {
                            string commCreator = "";
                            cmd.CommandText = "SELECT commCreator FROM storedComments WHERE reqNumber = @ReqNo AND isViewedBy IS NULL";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                            con.Open();

                            using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                            {
                                if (rdr.HasRows)
                                {
                                    rdr.Read();
                                    commCreator = rdr["commCreator"].ToString();

                                }
                                con.Close();
                                rdr.Close();
                            }

                            if (commCreator != ss.s_fullname)
                            {
                                if (ss.s_MMD == 0)
                                {
                                    UpdateCommentViewed(ReqNo, "pres");
                                }
                                else
                                {
                                    UpdateCommentViewed(ReqNo, "mmd");
                                }
                            }
                        }
                    }
                }

                model.reqStat = MMD.GetInStockStat(ReqNo);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            model.office = office;
            model.returnUrl = retUrl;
            model.POurl = PO;
            return View(model);
        }

        [Route("my-requests")]
        public ActionResult MyRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.MyRequests(ss);
                ViewBag.headTxt = "MY REQUESTS";
                Info.returnUrl = "my-requests";
                return View("MyRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("my-PO-requests")]
        public ActionResult MyPORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.MyRequests(ss);
                ViewBag.headTxt = "MY PO REQUESTS";
                Info.returnUrl = "my-PO-requests";
                Info.POurl = "PO";
                return View("MyRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("branch-open-requests")]
        public ActionResult Div_BranchRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.BranchRequests(ss, "Div");
                ViewBag.headTxt = "BRANCH REQUESTS";
                Info.returnUrl = "branch-open-requests";
                return View("Div_OpenRequest", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("head-office-open-requests")]
        public ActionResult Div_HORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.HeadOfficeRequests(ss, "Div");
                ViewBag.headTxt = "HEAD OFFICE REQUESTS";
                Info.returnUrl = "head-office-open-requests";
                return View("Div_OpenRequest", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }



        [Route("mmd-ho-requests")]
        public ActionResult MMD_HORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "HEAD OFFICE REQUESTS";
                Info = que.MMD_HORequests(ss, "");
                Info.office = "division";
                Info.returnUrl = "mmd-ho-requests";
                return View("MMD_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("mmd-ho-PO-requests")]
        public ActionResult MMD_HO_PORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "HEAD OFFICE PO REQUESTS";
                Info = que.MMD_HORequests(ss, "PO");
                Info.office = "division";
                Info.returnUrl = "mmd-ho-PO-requests";
                Info.POurl = "PO";
                return View("MMD_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("mmd-branch-requests")]
        public ActionResult MMD_BranchRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "BRANCH REQUESTS";
                Info = que.MMD_BranchRequests(ss, "");
                Info.returnUrl = "mmd-branch-requests";
                Info.office = "branch";
                return View("MMD_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("mmd-branch-PO-requests")]
        public ActionResult MMD_Branch_PORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "BRANCH PO REQUESTS";
                Info = que.MMD_BranchRequests(ss, "PO");
                Info.returnUrl = "mmd-branch-PO-requests";
                Info.office = "branch";
                Info.POurl = "PO";
                return View("MMD_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("gmo-branch-requests")]
        public ActionResult GMO_BranchRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "BRANCH REQUESTS";
                Info = que.GMO_BranchRequests(ss);
                Info.returnUrl = "gmo-branch-requests";
                Info.office = "branch";
                return View("GMO_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("gmo-branch-PO-requests")]
        public ActionResult GMO_BranchPORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "BRANCH PO REQUESTS";
                Info = que.GMO_BranchRequests(ss);
                Info.returnUrl = "gmo-branch-PO-requests";
                Info.office = "branch";
                Info.POurl = "PO";
                return View("GMO_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("gmo-ho-requests")]
        public ActionResult GMO_HORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "HEAD OFFICE REQUESTS";
                Info = que.GMO_HORequests(ss);
                Info.returnUrl = "gmo-ho-requests";
                Info.office = "branch";
                return View("GMO_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("gmo-ho-PO-requests")]
        public ActionResult GMO_HOPORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                ViewBag.headTxt = "HEAD OFFICE PO REQUESTS";
                Info = que.GMO_HORequests(ss);
                Info.returnUrl = "gmo-ho-PO-requests";
                Info.office = "branch";
                Info.POurl = "PO";
                return View("GMO_OpenRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("local-open-requests")]
        public ActionResult LocalRequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                string a = "";
                int b = ss.s_isDepartmentApprover;
                int c = ss.s_isDivisionApprover;

                if (b == 1)
                {
                    a = "dept";
                }
                else if (c == 1)
                {
                    a = "div";
                }

                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.LocalRequests(ss, a);
                Info.approver = a;
                Info.returnUrl = "local-open-requests";
                return View("LocalRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("local-PO-requests")]
        public ActionResult LocalPORequests()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                string a = "";
                int b = ss.s_isDepartmentApprover;
                int c = ss.s_isDivisionApprover;

                if (b == 1)
                {
                    a = "dept";
                }
                else if (c == 1)
                {
                    a = "div";
                }

                var que = new OpenQueries();
                var Info = new OpenReqInfo();
                var OpenReqList = new List<OpenReqViewModel>();
                Info._OpenInfo = OpenReqList;
                Info = que.LocalRequests(ss, a);
                Info.approver = a;
                Info.returnUrl = "local-PO-requests";
                Info.POurl = "PO";
                return View("LocalRequests", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        public string FontAwesomeSelector(string p)
        {
            string FAclass = "";
            switch (p)
            {
                case "Open":
                    FAclass = "fa fa-caret-square-o-down";
                    break;

                case "Served":
                    FAclass = "fa fa-check-circle txtSuccess";
                    break;

                case "Pending":
                    FAclass = "fa fa-share-square-o txtWarning";
                    break;

                case "Cancelled":
                    FAclass = "fa fa-window-close txtDanger";
                    break;

                default:
                    break;
            }
            return FAclass;
        }

        public JsonResult MovetoPending(string RequestNo)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Open SET reqStatus = 'PENDING', pendingDate = @date  WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@date", syscreated.ToString(format));
                    cmd.Parameters.AddWithValue("@ReqNo", RequestNo);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request has been move to pending..  || request no: " + RequestNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully moved to pending."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult RequestDetailsPDF(string ReqNo, string RegionName, string isDivRequest, string Requestor, string ReqTypeName, string Description, string Office)
        {
            var ss = (ORSession)Session["UserSession"];
            var date = DateTime.Now.ToString("MM/dd/yyyy");
            string totalQty = "", reqCreator = "", region = "", zoneCode = "";
            string name = ss.s_fullname.ToUpper();
            var db = new ORtoMySql();
            var con = db.getConnection();
            try
            {
                var cmd = new MySqlCommand("SELECT reqCreator, TotalQty, Zonecode FROM onlineRequest_Open where reqNumber=@ReqNo", con);
                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                cmd.CommandTimeout = 0;
                con.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        zoneCode = rdr["Zonecode"].ToString().Trim();
                        reqCreator = rdr["reqCreator"].ToString().Trim();
                        totalQty = String.Format("{0:#,##0}", rdr["TotalQty"]);
                    }
                }
                if (RegionName == "HO")
                {
                    region = "Head Office - " + zoneCode;
                }
                else
                {
                    region = RegionName;
                }
                var dt = GetRequestPDFData(ReqNo);
                var rpt = new ReportDocument();
                var IssuanceSlipDocument = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\request.rpt");
                rpt.Load(IssuanceSlipDocument);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("RequestNo", ReqNo);
                rpt.SetParameterValue("reqCreator", Requestor);
                rpt.SetParameterValue("date", date);
                rpt.SetParameterValue("requestType", ReqTypeName);
                rpt.SetParameterValue("Description", Description);
                rpt.SetParameterValue("Office", Office);

                rpt.SetParameterValue("Region", region);
                System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                return File(stream, "application/pdf", "REQUEST-" + ReqNo + "-" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
            }
            catch (Exception e)
            {
                log.Error("Error in printing and saving issuance slip." + e.ToString());
                ViewBag.Error = e.ToString();
                return View("Error");
            }
        }

        private ReportDataset GetRequestPDFData(string ReqNo)
        {
            try
            {
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    var data = new ReportDataset();
                    var ret = new List<IssuanceSlipModel>();
                    String sql = "select qty, unit, itemDescription as description from requestItems where reqNumber='" + ReqNo + "' ORDER BY syscreated ASC";
                    var cmd = new MySqlCommand(sql);
                    con.Open();
                    using (var adapter = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        adapter.SelectCommand = cmd;

                        var ds = new ReportDataset();
                        adapter.Fill(data, "DataTable2");
                    }
                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error in fetching data!" + e.ToString());
                throw (e);
            }
        }

        public string GetOR_DivisionName(string DeptCode)
        {
            var db = new ORtoMySql();
            string d = "";
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT DivName FROM division WHERE CostCenter =  @deptCode";
                cmd.Parameters.AddWithValue("@deptCode", DeptCode);
                cmd.CommandTimeout = 0;
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (read.HasRows)
                    {
                        read.Read();
                        d = read["DivName"].ToString().Trim();
                    }
                    else
                    {
                        d = "No division name";
                    }
                }
                return d;
            }
        }

        public string getRequestType(string requestType)
        {
            try
            {
                string t = "";
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.requestType WHERE TypeID = @TypeID";
                    cmd.Parameters.AddWithValue("@TypeID", requestType);
                    con.Open();
                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            read.Read();
                            t = read["reqType"].ToString().Trim();
                        }
                        else
                        {
                            return t = "No Request type has found";
                        }
                    }
                    return t;
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return requestType = "No Request Type";
            }
        }

        public string getBranchname(string BranchCode, string Region, string ZoneCode)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetBranchName?BranchCode=" + BranchCode + "&Region=" + Region + "&ZoneCode=" + ZoneCode);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                {
                    return "Service Unavailable";
                }
                var Response = JsonConvert.DeserializeObject<BranchNameResponse>(x);
                Result = Response.BranchName;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Result;
            }
            return Result;
        }

        public string GetRequestDetails_DivOrBranchName(int? isDivRequest, string BranchCode, string Region, string ZoneCode, string DeptCode)
        {
            try
            {
                string BranchName = "";
                if (isDivRequest == 1)
                {
                    BranchName = GetOR_DivisionName(DeptCode);
                }
                else
                {
                    string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                    var source = new System.Uri(w + "/GetBranchName?BranchCode=" + BranchCode + "&Region=" + Region + "&ZoneCode=" + ZoneCode);
                    var reqHandler = new RequestHandler(source, "GET", "application/json");
                    string x = reqHandler.HttpGetRequest();
                    if (x == "Error")
                    {
                        return "Service Unavailable";
                    }
                    var Response = JsonConvert.DeserializeObject<BranchNameResponse>(x);
                    BranchName = Response.BranchName;
                }
                return BranchName;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }


        public string UpdateCommentViewed(string reqNumber, string user)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            var userr = string.Empty;

            try
            {
                switch (user)
                {
                    case "pres":
                        userr = "UPDATE OnlineRequest.storedComments SET isViewedOn = @isViewedOn, isViewedBy = @viewer where reqNumber = @reqNumber";
                        break;

                    case "mmd":
                        userr = "UPDATE OnlineRequest.storedComments SET isViewedOn = @isViewedOn, isViewedBy = @viewer where reqNumber = @reqNumber AND commCreator = 'Michael L. Lhuillier'";
                        break;
                };

                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = userr;
                    cmd.Parameters.AddWithValue("@reqNumber", reqNumber);
                    cmd.Parameters.AddWithValue("@viewer", ss.s_fullname);
                    cmd.Parameters.AddWithValue("@isViewedOn", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return "Success!";
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "Failed!";

            }
        }
    }
}