using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class CloseRequestController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CloseRequestController));
        private string format = "yyyy/MM/dd HH:mm:ss";
        private OpenRequestController open = new OpenRequestController();
        private Helper que = new Helper();
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;

        public ActionResult closeRequest(string ReqNo, string Remarks, string ReceivedDate)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var mod = new CloseRequest();
                ORtoMySql db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo";
                        cmd.Parameters.AddWithValue("@reqNo", ReqNo);
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            rdr.Read();
                            mod.reqNumber = rdr["reqNumber"].ToString().Trim();
                            mod.reqDate = rdr["reqDate"].ToString().Trim();
                            mod.reqCreator = rdr["reqCreator"].ToString().Trim();
                            mod.TypeID = rdr["TypeID"].ToString().Trim();
                            mod.reqDescription = rdr["reqDescription"].ToString().Trim();
                            mod.BranchCode = rdr["BranchCode"].ToString().Trim();
                            mod.Area = rdr["Area"].ToString().Trim();
                            mod.AreaCode = rdr["AreaCode"].ToString().Trim();
                            mod.Region = rdr["Region"].ToString().Trim();
                            mod.DivCode = rdr["DivCode"].ToString().Trim();
                            mod.DeptCode = rdr["DeptCode"].ToString().Trim();
                            mod.ZoneCode = rdr["Zonecode"].ToString().Trim();
                            mod.reqStatus = rdr["reqStatus"].ToString().Trim();
                            mod.isApproved = rdr["isApproved"].ToString().Trim();
                            mod.isDivRequest = Convert.ToInt32(rdr["isDivRequest"]);
                            mod.forPresident = Convert.ToInt32(rdr["forPresident"]);
                        }

                        if (InsertCloseTable(mod, Remarks, ReceivedDate) == "success")
                        {
                            cmd.CommandText = "DELETE FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo ";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@reqNo", ReqNo);
                            conn.Open();
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                log.Info("A request has been closed by: " + mySession.s_fullname + " || request no: " + ReqNo);
                                que.SetDateModified(ReqNo, mySession.s_usr_id);
                                return Json(new { status = true, rescode = "2000", msg = "Request Successfully Closed." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            log.Error("Database Connection Error");
                            return Json(new { status = false, rescode = "2000", msg = "Database Connection Error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new { status = false, rescode = "2000", msg = "Error." }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("ViewOpenRequest", "OpenRequest");
        }

        public ActionResult closeRequestDetails(string Region, string ZoneCode, string ReqNo, string BranchCode, CloseRequest model)
        {
            ViewBag.ReqNo = ReqNo;

            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var itemLists = new List<RequestItems>();
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    using (var cmd = con.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE reqNumber = @ReqNo1";
                        cmd.Parameters.AddWithValue("@ReqNo1", ReqNo);
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            string TypeName = "";
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                model.reqNumber = rdr["reqNumber"].ToString().Trim();
                                model.reqDate = rdr["reqDate"].ToString().Trim();
                                model.reqCreator = rdr["reqCreator"].ToString().Trim();
                                TypeName = rdr["TypeID"].ToString().Trim();
                                ViewBag.requestName = getRequestType(TypeName);
                                model.TypeID = rdr["TypeID"].ToString().Trim();
                                model.reqDescription = rdr["reqDescription"].ToString().Trim();
                                model.BranchCode = rdr["BranchCode"].ToString().Trim();
                                model.DivCode = rdr["DivCode"].ToString().Trim();
                                model.DeptCode = rdr["DeptCode"].ToString().Trim();
                                model.Region = rdr["Region"].ToString().Trim();
                                model.Area = rdr["Area"].ToString().Trim();
                                model.AreaCode = rdr["AreaCode"].ToString().Trim();
                                model.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                                model.reqStatus = rdr["reqStatus"].ToString().Trim();
                                model.isDivRequest = Convert.ToInt32(rdr["isDivRequest"]);
                                model.ClosedDate = rdr["ClosedDate"].ToString().Trim();
                                model.ClosedBy = rdr["ClosedBy"].ToString().Trim();
                                model.BranchName = open.GetRequestDetails_DivOrBranchName(model.isDivRequest, model.BranchCode, Region, ZoneCode, model.DeptCode);
                                if (model.reqStatus != "DISAPPROVED")
                                {
                                    model.ReceivedDate = Convert.ToDateTime(rdr["ReceivedDate"]);
                                }
                                model.Remarks = rdr["Remarks"].ToString().Trim();
                                model.forPresident = Convert.ToInt32(rdr["forPresident"]);
                            }
                        }
                        cmd.CommandText = "SELECT * FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo2";
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

                                i.MMDstatus = open.FontAwesomeSelector(rdr["MMDstatus"].ToString().Trim());
                                i.SDCStatus = open.FontAwesomeSelector(rdr["SDCStatus"].ToString().Trim());
                                i.BranchStatus = open.FontAwesomeSelector(rdr["BranchStatus"].ToString().Trim());
                                i.DivStatus = open.FontAwesomeSelector(rdr["DivStatus"].ToString().Trim());

                                itemLists.Add(i);
                            }
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
                                model.E_VPO_POName = rdr["EscalationVPO_PO_Name"].ToString().Trim();
                                model.E_VPO_PODate = rdr["EscalationVPO_PO_Date"].ToString().Trim();
                                model.E_VPO_PORemarks = rdr["EscalationVPO_PO_Remarks"].ToString().Trim();
                                model.E_Pres_POName = rdr["EscalationPres_PO_Name"].ToString().Trim();
                                model.E_Pres_PODate = rdr["EscalationPres_PO_Date"].ToString().Trim();
                                model.E_Pres_PORemarks = rdr["EscalationPres_PO_Remarks"].ToString().Trim();
                            }
                        }
                        cmd.CommandText = "SELECT * FROM OnlineRequest.requestApproverStatus WHERE reqNumber = @ReqNo4";
                        cmd.Parameters.AddWithValue("@ReqNo4", ReqNo);
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

                                model.Sts_VPO_PO_Approver = rdr["VPO_PO_Approver"].ToString().Trim();
                                model.Sts_VPO_PO_Date = rdr["VPO_PO_Approved_Date"].ToString().Trim();
                                model.Sts_VPO_PO_isApproved = rdr["isVPO_PO_Approved"].ToString().Trim();
                                model.Sts_Pres_PO_Approver = rdr["Pres_PO_Approver"].ToString().Trim();
                                model.Sts_Pres_PO_Date = rdr["Pres_PO_Approved_Date"].ToString().Trim();
                                model.Sts_Pres_PO_isApproved = rdr["isPres_PO_Approved"].ToString().Trim();
                                model.PO_Approved = rdr["isPO_Approved"].ToString().Trim();
                            }
                        }
                        cmd.CommandText = "SELECT * FROM requestType WHERE TypeID = @TypeID";
                        cmd.Parameters.AddWithValue("@TypeID", model.TypeID);
                        con.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                model.isAMApproval = Convert.ToInt32(rdr["isAMApproval"]);
                                model.isRMApproval = Convert.ToInt32(rdr["isRMApproval"]);
                                model.isGMApproval = Convert.ToInt32(rdr["isGMApproval"]);
                                model.isDivManApproval = Convert.ToInt32(rdr["isDivManApproval"]);
                                model.isDivManApproval2 = Convert.ToInt32(rdr["isDivManApproval2"]);
                                model.isDivManApproval3 = Convert.ToInt32(rdr["isDivManApproval3"]);
                                model.isPresidentApproval = Convert.ToInt32(rdr["isPresidentApproval"]);
                            }
                        }
                        model.ReqItems = itemLists;
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
                    return "Service Unavailable";

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

        public string getRequestType(string requestType)
        {
            try
            {
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    string t = "";
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.requestType WHERE TypeID = @TypeID";
                    cmd.Parameters.AddWithValue("@TypeID", requestType);
                    con.Open();
                    var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        t = rdr["reqType"].ToString().Trim();
                    }
                    else
                    {
                        return t = "No Request type has found";
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

        public string InsertCloseTable(CloseRequest mod, string Remarks, string ReceivedDate)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return "error";

            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO OnlineRequest.onlineRequest_Close  " +
                            " (reqNumber, reqDate, reqCreator, TypeID, reqDescription, BranchCode, Region, Area, AreaCode ,DivCode, ZoneCode, " +
                            " reqStatus, isApproved, isDivRequest, ClosedDate, ClosedBy, ReceivedDate, Remarks, syscreated, syscreator, forPresident, deptCode ) " +
                            " VALUES (@reqNumber, @reqDate, @reqCreator, @TypeID, @reqDescription, @BranchCode, @Region, @Area, @AreaCode, @DivCode, @ZoneCode,  " +
                            " @reqStatus, @isApproved, @isDivRequest, @ClosedDate, @ClosedBy, @ReceivedDate, @Remarks, @syscreated, @syscreator, @forPresident, @deptCode)";
                        cmd.Parameters.AddWithValue("@reqNumber", mod.reqNumber);
                        cmd.Parameters.AddWithValue("@reqDate", Convert.ToDateTime(mod.reqDate));
                        cmd.Parameters.AddWithValue("@reqCreator", mod.reqCreator);
                        cmd.Parameters.AddWithValue("@TypeID", mod.TypeID);
                        cmd.Parameters.AddWithValue("@reqDescription", mod.reqDescription);
                        cmd.Parameters.AddWithValue("@BranchCode", mod.BranchCode);
                        cmd.Parameters.AddWithValue("@Region", mod.Region);
                        cmd.Parameters.AddWithValue("@Area", mod.Area);
                        cmd.Parameters.AddWithValue("@AreaCode", mod.AreaCode);
                        cmd.Parameters.AddWithValue("@DivCode", mod.DivCode);
                        cmd.Parameters.AddWithValue("@ZoneCode", mod.ZoneCode);
                        cmd.Parameters.AddWithValue("@reqStatus", "CLOSED");
                        cmd.Parameters.AddWithValue("@isApproved", mod.isApproved);
                        cmd.Parameters.AddWithValue("@isDivRequest", mod.isDivRequest);
                        cmd.Parameters.AddWithValue("@deptCode", mod.DeptCode);
                        cmd.Parameters.AddWithValue("@ClosedDate", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@ClosedBy", mySession.s_fullname);
                        cmd.Parameters.AddWithValue("@ReceivedDate", Convert.ToDateTime(ReceivedDate));
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@syscreator", mySession.s_usr_id);
                        cmd.Parameters.AddWithValue("@forPresident", mod.forPresident);
                        cmd.ExecuteNonQuery();
                        log.Info("Successfull insert into close table || request no: " + mod.reqNumber);
                        return "success";
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return "error";
            }
        }

        [Route("close-requests")]
        public ActionResult ViewCloseRequest(CloseReqInfo Info, string c)
        {
            var User = (ORSession)Session["UserSession"];
            if (User == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var CloseReqList = new List<CloseReqViewModel>();
                var db = new ORtoMySql();

                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    if (User.s_job_title == "AREA MANAGER")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems  b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c AND (a.Area = @Area AND a.Region = @Region AND a.ZoneCode = @ZoneCode OR a.reqCreator = @Creator) " +
                                          " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                        cmd.Parameters.AddWithValue("@Region", User.s_region);
                        cmd.Parameters.AddWithValue("@Creator", User.s_fullname);
                        cmd.Parameters.AddWithValue("@Area", User.s_area);
                        cmd.Parameters.AddWithValue("@ZoneCode", User.s_zonecode);
                    }
                    else if (User.s_job_title == "REGIONAL MAN")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems  b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c AND (a.Region = @Region AND a.ZoneCode = @ZoneCode OR a.reqCreator = @Creator) " +
                                          " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                        cmd.Parameters.AddWithValue("@Region", User.s_region);
                        cmd.Parameters.AddWithValue("@Creator", User.s_fullname);
                        cmd.Parameters.AddWithValue("@ZoneCode", User.s_zonecode);
                    }
                    else if (User.s_costcenter == "0001MMD")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                          " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                    }
                    else if (User.s_costcenter == "0002MMD")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR' ) " +
                                          " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                    }
                    else if (User.s_isDivisionApprover == 1 && User.s_task != "GMO-GENMAN" && User.s_usr_id != "LHUI1011873")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description" +
                                          " FROM OnlineRequest.onlineRequest_Close a" +
                                          " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus =@c AND a.deptCode = @deptCode " +
                                          " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                        cmd.Parameters.AddWithValue("@Creator", User.s_fullname);
                        cmd.Parameters.AddWithValue("@deptCode", User.s_costcenter);
                    }
                    else if (User.s_task == "GMO-GENMAN" || User.s_isVPAssistant == 1)
                    {
                        if (User.s_costcenter == "0001GMO")
                        {
                            cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                              " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description" +
                                              " FROM OnlineRequest.onlineRequest_Close a " +
                                              " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                              " WHERE a.reqStatus = @c AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                        }
                        else if (User.s_costcenter == "0002GMO")
                        {
                            cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                              " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                              " FROM OnlineRequest.onlineRequest_Close a " +
                                              " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                              " WHERE a.reqStatus = @c AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                        }
                    }
                    else if (User.s_usr_id == "LHUI1011873")
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                    }
                    else
                    {
                        cmd.CommandText = " SELECT * , COUNT(b.reqnumber) AS TotalCount, " +
                                          " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description " +
                                          " FROM OnlineRequest.onlineRequest_Close a " +
                                          " INNER JOIN requestItems b ON a.reqnumber = b.reqnumber " +
                                          " WHERE a.reqStatus = @c AND a.reqCreator = @Creator GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                        cmd.Parameters.AddWithValue("@Region", User.s_region);
                        cmd.Parameters.AddWithValue("@Creator", User.s_fullname);
                        cmd.Parameters.AddWithValue("@BranchCode", User.s_comp);
                        cmd.Parameters.AddWithValue("@ZoneCode", User.s_zonecode);
                        cmd.Parameters.AddWithValue("@deptCode", User.s_costcenter);
                    }
                    cmd.Parameters.AddWithValue("@c", c);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            var o = new CloseReqViewModel();
                            o.reqNumber = rdr["reqNumber"].ToString().Trim();
                            o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                            o.reqDescription = rdr["reqDescription"].ToString().Trim();
                            o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                            o.closedDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["ClosedDate"].ToString()));
                            o.TypeID = rdr["TypeID"].ToString().Trim();

                            o.BranchCode = rdr["BranchCode"].ToString().Trim();
                            o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                            o.Region = rdr["Region"].ToString().Trim().ToUpper();
                            o.DeptCode = rdr["DeptCode"].ToString().Trim();
                            o.reqStatus = rdr["reqStatus"].ToString().Trim();
                            if (o.Region == "HO")
                                o.Region = open.GetOR_DivisionName(o.DeptCode).ToUpper();

                            string typeName = "";
                            typeName = que.GetTypeName(o.TypeID);
                            if (typeName.Length > 17)
                            {
                                o.TypeName = typeName.Substring(0, 17) + "..";
                            }
                            else
                            {
                                o.TypeName = typeName;
                            }
                            o.TotalCount = rdr["TotalCount"].ToString().Trim();
                            o.itemDescription = rdr["Description"].ToString().Trim();

                            o.TotalQuantity = String.Format("{0:#,##0}", rdr["TotalQty"]);
                            o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                            if (o.isDivRequest == "1")
                            {
                                o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                            }
                            else
                            {
                                o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                            }
                            CloseReqList.Add(o);
                        }
                    }
                    Info._CloseInfo = CloseReqList;
                }
                ViewBag.c = c;
                return View(Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }
    }
}