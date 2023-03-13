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
    public class PendingController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PendingController));
        private PendingQueries que = new PendingQueries();
        private OpenRequestController oReq = new OpenRequestController();
        private Helper help = new Helper();

        [Route("pending-requests")]
        public ActionResult PendingRequests(OpenReqInfo Info)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            
            try
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var OpenReqList = new List<OpenReqViewModel>();
                    using (var cmd = conn.CreateCommand())
                    {
                        if (ss.s_costcenter == "0001MMD")
                        {
                            que.Pending_MMD_Vismin(ss, Info);
                            return View(Info);
                        }
                        if (ss.s_costcenter == "0002MMD")
                        {
                            que.Pending_MMD_Luzon(ss, Info);
                            return View(Info);
                        }
                        if (ss.s_job_title == "AREA MANAGER")
                        {
                            que.Pending_AM(ss, Info);
                            return View(Info);
                        }
                        else if (ss.s_job_title == "REGIONAL MAN")
                        {
                            que.Pending_RM(ss, Info);
                            return View(Info);
                        }
                        else if (ss.s_isDepartmentApprover == 1)
                        {
                            que.Pending_Dept_Approver(ss, Info);
                            return View(Info);
                        }
                        else if (ss.s_isDivisionApprover == 1 && ss.s_task != "GMO-GENMAN" && ss.s_usr_id != "LHUI1011873")
                        {
                            que.Pending_Div_Approver(ss, Info);
                            return View(Info);
                        }
                        else if (ss.s_task == "GMO-GENMAN" || ss.s_isVPAssistant == 1)
                        {
                            que.Pending_GMO(ss, Info);
                            return View(Info);
                        }
                        else if (ss.s_usr_id == "LHUI1011873")
                        {
                            que.Pending_President(ss, Info);
                            return View(Info);
                        }
                        else
                        {
                            cmd.CommandText = " SELECT " +
                                              " a.reqNumber,  a.reqCreator, a.reqDescription, COUNT(d.itemDescription) AS TotalCount, a.reqDate, a.TypeID , a.reqTotal, a.BranchCode, a.Region, a.DivCode, " +
                                              " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                              " a.Zonecode, a.reqStatus, a.isDivRequest,b.isApprovedLocalDiv, a.DeptCode, a.forPresident," +
                                              " b.isApprovedDM, c.isDMApproval, " +
                                              " b.isApprovedAM, c.isAMApproval, " +
                                              " b.isApprovedRM ,c.isRMApproval, " +
                                              " b.isApprovedGM, c.isGMApproval, b.isApprovedVPAssistant," +
                                              " b.isApprovedDiv1, c.isDivManApproval, " +
                                              " b.isApprovedDiv2, c.isDivManApproval2, " +
                                              " b.isApprovedDiv3, c.isDivManApproval3, " +
                                              " b.isApprovedPres, c.isPresidentApproval, " +
                                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, c.DivCode1, c.DivCode2, c.DivCode3 " +
                                              " FROM onlineRequest_Open a " +
                                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                              " INNER JOIN requestType c ON a.TypeID = c.TypeID " +
                                              " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber  " +
                                              " WHERE a.reqStatus = 'PENDING' AND (a.BranchCode = @BranchCode AND a.Region = @Region AND a.ZoneCode = @ZoneCode) " +
                                              " AND a.reqCreator = @Creator GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                            cmd.Parameters.AddWithValue("@Region", ss.s_region);
                            cmd.Parameters.AddWithValue("@Creator", ss.s_fullname);
                            cmd.Parameters.AddWithValue("@BranchCode", ss.s_comp);
                            cmd.Parameters.AddWithValue("@ZoneCode", ss.s_zonecode);
                        }

                        conn.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var o = new OpenReqViewModel();
                                o.reqNumber = rdr["reqNumber"].ToString().Trim();
                                o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                o.itemDescription = rdr["Description"].ToString().Trim();
                                o.reqDescription = rdr["reqDescription"].ToString().Trim();
                                o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                                o.TypeID = rdr["TypeID"].ToString().Trim();
                                o.TypeName = help.GetTypeName(o.TypeID);
                                o.TotalItems = rdr["TotalCount"].ToString().Trim();
                                o.BranchCode = rdr["BranchCode"].ToString().Trim();
                                o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                                o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                                o.DeptCode = rdr["DeptCode"].ToString().Trim();
                                o.Region = rdr["Region"].ToString().Trim().ToUpper();
                                if (o.Region == "HO")
                                {
                                    o.Region = oReq.GetOR_DivisionName(o.DeptCode).ToUpper();
                                }
                                o.forPresident = rdr["forPresident"].ToString().Trim();
                                if (o.isDivRequest == "1")
                                {
                                    o.BranchName = oReq.getBranchname(o.BranchCode, "HO", o.ZoneCode);
                                }
                                else
                                {
                                    o.BranchName = toTC.ToTitleCase(oReq.getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
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

                                OpenReqList.Add(o);
                            }
                        }
                        Info._OpenInfo = OpenReqList;
                    }
                }
                return View(Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        public ActionResult PendingDetails(string Region, string ZoneCode, string ReqNo, string BranchCode, CreateReqModels model)
        {
            ViewBag.ReqNo = ReqNo;
            var ss = (ORSession)Session["UserSession"];
            if (ss == null)
            {
                return RedirectToAction("Logout", "Userlogin");
            }
            try
            {
                var itemLists = new List<RequestItems>();
                var db = new ORtoMySql();
                var toTC = new CultureInfo("en-US", false).TextInfo;
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
                                rdr.Read();
                                model.RequestNo = rdr["reqNumber"].ToString().Trim();
                                model.ReqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                                model.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                TypeName = rdr["TypeID"].ToString().Trim();
                                ViewBag.requestName = oReq.getRequestType(TypeName);
                                model.RequestType = rdr["TypeID"].ToString().Trim();
                                model.Description = rdr["reqDescription"].ToString().Trim();
                                model.reqTotal = String.Format("{0:n}", rdr["reqTotal"]);
                                model.BranchCode = rdr["BranchCode"].ToString().Trim();
                                model.DivisionCode = rdr["DivCode"].ToString().Trim();
                                model.Area = rdr["Area"].ToString().Trim();
                                model.Region = rdr["Region"].ToString().Trim();
                                model.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                                model.reqStatus = rdr["reqStatus"].ToString().Trim();
                                model.isDivRequest = Convert.ToInt32(rdr["isDivRequest"]);
                                model.req_DeptCode = rdr["DeptCode"].ToString().Trim();
                                model.Bednrm = oReq.GetRequestDetails_DivOrBranchName(model.isDivRequest, model.BranchCode, Region, ZoneCode, model.req_DeptCode);
                                model.forPresident = Convert.ToInt32(rdr["forPresident"]);                                
                                model.hasDiagnostic = help.DiagnosticCheck(ReqNo);
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
                            }
                            con.Close();
                            rdr.Close();
                        }
                        cmd.CommandText = "SELECT * FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo2 ORDER BY id ASC";
                        cmd.Parameters.AddWithValue("@ReqNo2", ReqNo);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var i = new RequestItems();
                                i.ItemDescription = rdr["itemDescription"].ToString().Trim();
                                i.ItemQty = rdr["qty"].ToString().Trim();
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
                            }
                            con.Close();
                            rdr.Close();
                        }
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
                        }
                        model.ReqItems = itemLists;

                        // Activate Dropdown
                        if (ss.s_MMD == 1)
                        {
                            ViewBag.MMD = "dropdown";
                        }
                        else
                        {
                            ViewBag.MMD = "";
                        }

                        if (ss.s_SDCApprover == 1 && model.Sts_MMD_isDelivered == "1")
                        {
                            ViewBag.SDC = "dropdown";
                        }
                        else
                        {
                            ViewBag.SDC = "";
                        }

                        if ((new[] { "001", "002" }).Contains(ss.s_comp) && ss.s_MMD != 1 && model.Sts_MMD_isDelivered == "1" && ss.s_fullname == model.reqCreator)
                        {
                            ViewBag.Division = "dropdown";
                        }
                        else
                        {
                            ViewBag.Division = "";
                        }

                        if (!(new[] { "001", "002" }).Contains(ss.s_comp) && ss.s_SDCApprover == 0 || ss.s_fullname == model.reqCreator)
                        {
                            ViewBag.Branch = "dropdown";
                        }
                        else
                        {
                            ViewBag.Branch = "";
                        }
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            return View(model);
        }

        private string FontAwesomeSelector(string p)
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
    }
}