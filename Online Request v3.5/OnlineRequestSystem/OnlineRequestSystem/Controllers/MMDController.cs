﻿using MySql.Data.MySqlClient;
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

namespace OnlineRequestSystem.Controllers
{
    public class MMDController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MMDController));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private OpenRequestController open = new OpenRequestController();
        private MMDQueries que = new MMDQueries();
        private Helper set = new Helper();
        private string format = "yyyy/MM/dd HH:mm:ss";

        public ActionResult Index()
        {
            return View();
        }

        #region Display MMD Status

        public ActionResult viewMMDStatus(string selected, string retUrl, string office)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var Info = new OpenReqInfo();
                var toTC = new CultureInfo("en-US", false).TextInfo;
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var OpenReqList = new List<OpenReqViewModel>();
                    var cmd = conn.CreateCommand();

                    conn.Open();
                    if ((new[] { "VISMIN", "VISAYAS", "MINDANAO" }).Contains(mySession.s_zonecode))
                    {
                        switch (selected)
                        {
                            case "PROCESSED PO":

                                #region PP Query

                                cmd.CommandText = "SELECT a.reqNumber, a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID  " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                              " AND b.isDelivered = 0 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion PP Query

                                break;

                            case "RECEIVED FROM SUPPLIER":

                                #region RFS Query

                                cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE  a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                              " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion RFS Query

                                break;

                            case "IN TRANSIT-SDC":

                                #region ITS Query

                                cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                              " AND b.isDelivered = 1 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion ITS Query

                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (selected) // LUZON
                        {
                            case "PROCESSED PO":

                                #region PP Query

                                cmd.CommandText = "SELECT a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount,  a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                              " AND b.isDelivered = 0 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion PP Query

                                break;

                            case "RECEIVED FROM SUPPLIER":

                                #region RFS Query

                                cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                              " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion RFS Query

                                break;

                            case "IN TRANSIT-SDC":

                                #region ITS Query

                                cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                              " (SELECT itemDescription FROM requestitems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                              " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                              " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                              " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                              " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                              " INNER JOIN requestitems d ON d.reqNumber = a.reqNumber " +
                              " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                              " AND b.isDelivered = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                              " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                #endregion ITS Query

                                break;

                            default:
                                break;
                        }
                    }

                    int isdivrequest = 0;
                    if (office == "division")
                    {
                        isdivrequest = 1;
                    }
                    cmd.Parameters.AddWithValue("@office", isdivrequest);
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            var o = new OpenReqViewModel();
                            o.reqNumber = rdr["reqNumber"].ToString().Trim();
                            o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                            o.reqDescription = rdr["reqDescription"].ToString().Trim();
                            o.reqDate = string.Format("{0:MM/dd/yyyy}", Convert.ToDateTime(rdr["reqDate"].ToString()));
                            o.TypeID = rdr["TypeID"].ToString().Trim();
                            o.TotalItems = rdr["TotalCount"].ToString().Trim();
                            o.itemDescription = rdr["Description"].ToString().Trim();
                            string typeName = "";
                            typeName = set.GetTypeName(o.TypeID);
                            if (typeName.Length > 17)
                            {
                                o.TypeName = typeName.Substring(0, 17) + "..";
                            }
                            else
                            {
                                o.TypeName = typeName;
                            }
                            o.BranchCode = rdr["BranchCode"].ToString().Trim();
                            o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                            o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                            o.DeptCode = rdr["DeptCode"].ToString().Trim();
                            o.Region = rdr["Region"].ToString().Trim().ToUpper();
                            if (o.Region == "HO")
                                o.Region = open.GetOR_DivisionName(o.DeptCode).ToUpper();

                            if (o.isDivRequest == "1")
                            {
                                o.BranchName = getBranchname(o.BranchCode, "HO", o.ZoneCode);
                            }
                            else
                            {
                                o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower()).Replace("Ml ", "ML ");
                            }
                            o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                            o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                            o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);

                            OpenReqList.Add(o);
                        }
                    }
                    Info.office = office;
                    Info._OpenInfo = OpenReqList;
                }
                ViewBag.Msg = selected;
                ViewBag.headTxt = selected + " REQUESTS";
                return View("~/Views/OpenRequest/MMD_OpenRequests.cshtml", Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        #endregion Display MMD Status

        #region Processed PO

        public ActionResult ProcessedPO(string reqNumber, string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            if (ss == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                    cmd.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@isMMDProcessed", 1);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            set.SetDateModified(ReqNo, ss.s_usr_id);
            log.Info("A request has been (PO)Processed || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
            return Json(new
            {
                status = true,
                rescode = "2001",
                msg = "Successfully saved."
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion Processed PO

        #region In Transit

        public ActionResult InTransit(string MMD_Transitor, string ReqNo)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                if (CheckOverAllStatus(ReqNo) == true)
                    UpdateIsApproved(ReqNo);

                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Transitor = @Transitor, MMD_Transit_Date = @Date , isMMDTransit = @isTransit WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@Transitor", mySession.s_usr_id);
                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@isTransit", 1);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            set.SetDateModified(ReqNo, mySession.s_usr_id);
            log.Info("A request is in Transit || request no: " + ReqNo + " || Processor: " + mySession.s_usr_id);

            return Json(new
            {
                status = true,
                rescode = "2001",
                msg = "Successfully saved."
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion In Transit

        public ActionResult ForDelivery(string MMD_Deliverer, string ReqNo, string RegionName)
        {
            var mySession = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            if (mySession == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Deliverer = @MMDDeliverer , MMD_Delivered_Date = @DeliveredDate , isDelivered = @isDelivered WHERE reqNumber = @ReqNo";
                        cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                        cmd.Parameters.AddWithValue("@MMDDeliverer", mySession.s_usr_id);
                        cmd.Parameters.AddWithValue("@DeliveredDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isDelivered", 1);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            set.SetDateModified(ReqNo, mySession.s_usr_id);
            log.Info("A request is ready for delivery || request no: " + ReqNo + " || Processor: " + mySession.s_usr_id);
            return Json(new { status = true, rescode = "2001", msg = "Successfully saved." }, JsonRequestBehavior.AllowGet);
        }

        #region Generate Issuance Slip Method Commented

        //public ActionResult GenerateIssuanceSlip(string ReqNo, string RegionName, string isDivRequest, string Requestor)
        //{
        //    var mySession = (ORSession)Session["UserSession"];
        //    var date = DateTime.Now.ToString("MM/dd/yyyy");
        //    string issuNo = "";
        //    string totalQty = "";
        //    string name = mySession.s_fullname.ToUpper();
        //    String sql = "";
        //    ORtoMySql db = new ORtoMySql();
        //    MySqlConnection con = db.getConnection();
        //    try
        //    {
        //        sql = "SELECT issuanceNo, TotalQty_MMD FROM onlineRequest_Open where reqNumber=@ReqNo";
        //        using (MySqlCommand cmd = new MySqlCommand(sql, con))
        //        {
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
        //            cmd.CommandTimeout = 0;

        //            using (MySqlDataReader read = cmd.ExecuteReader())
        //            {
        //                while (read.Read())
        //                {
        //                    issuNo = read["issuanceNo"].ToString();
        //                    totalQty = read["TotalQty_MMD"].ToString().Trim();
        //                }
        //            }
        //        }

        //        ReportDataset dt = GetIssuanceSlipData(ReqNo);
        //        ReportDocument rpt = new ReportDocument();
        //        var IssuanceSlipDocument = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\issuanceForm.rpt");
        //        rpt.Load(IssuanceSlipDocument);
        //        rpt.SetDataSource(dt);
        //        rpt.SetParameterValue("issuanceNo", issuNo);
        //        rpt.SetParameterValue("to", RegionName);
        //        rpt.SetParameterValue("from", "MMD");
        //        rpt.SetParameterValue("date", date);
        //        if (isDivRequest == "1")
        //        {
        //            rpt.SetParameterValue("receiver", Requestor);
        //        }
        //        else
        //        {
        //            rpt.SetParameterValue("receiver", GetSDCRegionalManager(ReqNo));
        //        }

        //        rpt.SetParameterValue("name", name);
        //        rpt.SetParameterValue("totalQty", totalQty);
        //        System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

        //        return File(stream, "application/pdf", "IssuanceSlip|" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error("Error in printing and saving issuance slip." + e.ToString());
        //        ViewBag.Error = e.ToString();
        //        return View("Error");
        //    }
        //}

        //private string GetSDCRegionalManager(string ReqNo)
        //{
        //    string Region = que.GetRegionOfTheRequest(ReqNo);
        //    ORtoSQL db = new ORtoSQL();
        //    try
        //    {
        //        string RMName = "";
        //        using (SqlConnection conn = db.getConnection())
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                string GetBranchCode = "SELECT fullname FROM IRRegionalManagers WHERE Class_03 = @region";
        //                cmd.Parameters.AddWithValue("@region", Region);
        //                cmd.CommandText = GetBranchCode;
        //                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
        //                {
        //                    if (rdr.HasRows)
        //                    {
        //                        rdr.Read();
        //                        RMName = rdr["fullname"].ToString().Trim();
        //                    }
        //                    else
        //                    {
        //                        RMName = "No RM Found";
        //                    }
        //                }
        //            }
        //            return RMName;
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        log.Fatal(x.Message, x);
        //        throw;
        //    }
        //}

        #endregion Generate Issuance Slip Method Commented

        private ReportDataset GetIssuanceSlipData(string ReqNo)
        {
            var db = new ORtoMySql();
            var con = db.getConnection();
            var data = new ReportDataset();
            var ret = new List<IssuanceSlipModel>();
            String sql = "select actualQtyMMD as qty, unit, itemDescription as description from requestItems where reqNumber='" + ReqNo + "' ORDER BY syscreated ASC";

            try
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql))
                {
                    MySqlDataAdapter a = new MySqlDataAdapter();

                    cmd.Connection = con;
                    a.SelectCommand = cmd;

                    using (var ds = new ReportDataset())
                    {
                        a.Fill(data, "DataTable2");
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

        #region Checking Overall Status of Request

        public Boolean CheckOverAllStatus(string ReqNo)
        {
            try
            {
                var db = new ORtoMySql();
                var fr = new Finalresult();
                var RTList = new List<AddRequestType>();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT a.reqNumber,  a.reqCreator , a.reqDescription, a.reqDate, a.TypeID , a.reqTotal, " +
                    " a.BranchCode, a.Region, a.DivCode, a.Zonecode, a.reqStatus, " +
                    " IF(b.isApprovedAM = c.isAMApproval, 1, 0) AS AM, " +
                    " IF(b.isApprovedRM = c.isRMApproval, 1, 0) AS RM, " +
                    " IF(b.isApprovedGM = c.isGMApproval, 1, 0) AS GM, " +
                    " IF(b.isApprovedDiv1 = c.isDivManApproval, 1, 0) AS Div1, " +
                    " IF(b.isApprovedDiv2 = c.isDivManApproval2, 1, 0) AS Div2, " +
                    " IF(b.isApprovedDiv3 = c.isDivManApproval3, 1, 0) AS Div3, " +
                    " IF(b.isApprovedPres = c.isPresidentApproval, 1, 0) AS President, " +
                    " b.isMMDProcessed, b.isDelivered, b.isMMDTransit " +
                    " FROM onlineRequest_Open a " +
                    " RIGHT JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                    " RIGHT JOIN requestType c ON a.TypeID = c.TypeID " +
                    " WHERE a.reqNumber = @reqNo";
                    cmd.Parameters.AddWithValue("@reqNo", ReqNo);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        read.Read();
                        fr.res_AM = Convert.ToInt32(read["AM"]);
                        fr.res_RM = Convert.ToInt32(read["RM"]);
                        fr.res_Div1 = Convert.ToInt32(read["Div1"]);
                        fr.res_Div2 = Convert.ToInt32(read["Div2"]);
                        fr.res_Div3 = Convert.ToInt32(read["Div3"]);
                        fr.res_GM = Convert.ToInt32(read["GM"]);
                        fr.res_Pres = Convert.ToInt32(read["President"]);
                    }
                }
                if (fr.res_AM == 1 && fr.res_RM == 1 && fr.res_Div1 == 1 && fr.res_Div2 == 1 && fr.res_Div3 == 1 &&
                    fr.res_GM == 1 && fr.res_Pres == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        #endregion Checking Overall Status of Request

        #region Update isApproved in OnlineRequest Open

        public ActionResult UpdateIsApproved(string ReqNo)
        {
            var mySession = (ORSession)Session["UserSession"];
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Open SET isApproved = @Approved WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@Approved", 1);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            return View(ReqNo);
        }

        #endregion Update isApproved in OnlineRequest Open

        #region Get Branch name

        public string getBranchname(string BranchCode, string Region, string ZoneCode)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetBranchName?BranchCode=" + BranchCode + "&Region=" + Region + "&ZoneCode=" + ZoneCode);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (String.IsNullOrEmpty(x))
                    Result = "No Service";

                BranchNameResponse resData = JsonConvert.DeserializeObject<BranchNameResponse>(x);
                Result = resData.BranchName;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Result;
            }
            return Result;
        }

        #endregion Get Branch name

        #region Get Division Details

        public ActionResult DivisionDetails(string divName)
        {
            try
            {
                var ss = (ORSession)Session["UserSession"];
                var dd = new DivisionDetails();
                string divAcro = que.GetDivisionAcro(divName);
                dd = que.GetDivisionDetails(divAcro);
                dd.DivisionResourceID = que.GetDivisionApproverResourceID(ss.s_zonecode, dd.DivisionManager);
                dd.DivID = que.Get_DivID(divAcro);

                return Json(new
                {
                    status = true,
                    divacroo = dd.DivisionAcro,
                    zonecode = dd.ZoneCode,
                    divmanager = dd.DivisionManager,
                    resourceID = dd.DivisionResourceID,
                    divID = dd.DivID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new { status = false, error = x.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Get Division Details

        #region Retrieve all request types in Request Type Tab

        public ActionResult RequestType(RequestTypeInfo ReqInfo)
        {
            try
            {
                var ss = (ORSession)Session["UserSession"];
                var db = new ORtoMySql();
                var RTList = new List<AddRequestType>();
                using (MySqlConnection conn = db.getConnection())
                {
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.requestType WHERE ZoneCode = @zcode";
                    cmd.Parameters.AddWithValue("@zcode", ss.s_zonecode);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        var a = new AddRequestType();
                        a.IDforUpdate = Convert.ToInt32(rdr["id"].ToString().Trim());
                        a.RequestType = rdr["reqType"].ToString().Trim();
                        a.isAMApproval = Convert.ToInt32(rdr["isAMApproval"].ToString().Trim());
                        a.isRMApproval = Convert.ToInt32(rdr["isRMApproval"].ToString().Trim());
                        a.isGMApproval = Convert.ToInt32(rdr["isGMApproval"].ToString().Trim());
                        a.isDivManApproval = Convert.ToInt32(rdr["isDivManApproval"].ToString().Trim());
                        a.isDivManApproval2 = Convert.ToInt32(rdr["isDivManApproval2"].ToString().Trim());
                        a.isDivManApproval3 = Convert.ToInt32(rdr["isDivManApproval3"].ToString().Trim());
                        a.isPresidentApproval = Convert.ToInt32(rdr["isPresidentApproval"].ToString().Trim());
                        if (a.isDivManApproval == 1)
                        {
                            string val = rdr["DivCode1"].ToString().Trim();
                            a.DivCode1 = val;
                            a.DivName1 = que.GetDivisionName(val);
                        }
                        if (a.isDivManApproval2 == 1)
                        {
                            string val2 = rdr["DivCode2"].ToString().Trim();
                            a.DivCode2 = val2;
                            a.DivName2 = que.GetDivisionName(val2);
                        }
                        if (a.isDivManApproval3 == 1)
                        {
                            string val3 = rdr["DivCode3"].ToString().Trim();
                            a.DivCode3 = val3;
                            a.DivName3 = que.GetDivisionName(val3);
                        }
                        RTList.Add(a);
                    }
                    ReqInfo.ListOfDivision = que.ListOfDivision(ss.s_zonecode).ToList();
                    ReqInfo._RTInfo = RTList;
                    return View(ReqInfo);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        #endregion Retrieve all request types in Request Type Tab

        #region Create new request type

        public ActionResult AddRequestType(RequestTypeInfo ReqData)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                ORtoMySql db = new ORtoMySql();
                if (reqTypeChecking(ReqData.RequestType, mySession.s_zonecode) == true)
                {
                    return Json(new
                    {
                        status = true,
                        rescode = "2000",
                        msg = "Unable to Process. Request type already exists."
                    }, JsonRequestBehavior.AllowGet);
                }
                int isAMApproval = (ReqData.isAMApproval == true) ? 1 : 0;
                int isRMApproval = (ReqData.isRMApproval == true) ? 1 : 0;
                int isGMApproval = (ReqData.isGMApproval == true) ? 1 : 0;
                int isDivManApproval = (ReqData.isDivManApproval == true) ? 1 : 0;
                int isDivManApproval2 = (ReqData.isDivManApproval2 == true) ? 1 : 0;
                int isDivManApproval3 = (ReqData.isDivManApproval3 == true) ? 1 : 0;
                int isPresidentApproval = (ReqData.isPresidentApproval == true) ? 1 : 0;

                using (MySqlConnection conn = db.getConnection())
                {
                    string logTypeID = "";
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO OnlineRequest.requestType " +
                        " (TypeID, reqType, isDMApproval , isAMApproval, isRMApproval, isGMApproval, isDivManApproval, DivCode1 ,isDivManApproval2, DivCode2, isDivManApproval3, DivCode3, isPresidentApproval,ZoneCode, syscreated, syscreator)" +
                        " VALUES (@typeID, @reqType, @DM ,@AM, @RM, @GM, @DivMan,@DivCode1, @DivMan2, @DivCode2, @DivMan3, @DivCode3, @Pres, @zoneCode, @syscreated, @syscreator) ";
                    var TempTypeID = getNewTypeID();
                    string TypeID = "ORS-" + DateTime.Now.Year.ToString().Remove(0, 2) + DateTime.Now.Day.ToString() + "-" + TempTypeID;
                    logTypeID = TypeID;
                    cmd.Parameters.AddWithValue("@typeID", TypeID);
                    cmd.Parameters.AddWithValue("@reqType", ReqData.RequestType);
                    cmd.Parameters.AddWithValue("@DM", 1);
                    cmd.Parameters.AddWithValue("@AM", isAMApproval);
                    cmd.Parameters.AddWithValue("@RM", isRMApproval);
                    cmd.Parameters.AddWithValue("@GM", isGMApproval);
                    cmd.Parameters.AddWithValue("@DivMan", isDivManApproval);
                    cmd.Parameters.AddWithValue("@DivCode1", ReqData.DivCode1);
                    cmd.Parameters.AddWithValue("@DivMan2", isDivManApproval2);
                    cmd.Parameters.AddWithValue("@DivCode2", ReqData.DivCode2);
                    cmd.Parameters.AddWithValue("@DivMan3", isDivManApproval3);
                    cmd.Parameters.AddWithValue("@DivCode3", ReqData.DivCode3);
                    cmd.Parameters.AddWithValue("@Pres", isPresidentApproval);
                    cmd.Parameters.AddWithValue("@zoneCode", mySession.s_zonecode);
                    cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@syscreator", mySession.s_fullname);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    log.Info("New request type has been created || Creator: " + mySession.s_usr_id + " || Request type ID: " + logTypeID);
                    return Json(new
                    {
                        status = true,
                        rescode = "2000",
                        msg = "New request type has been added."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    rescode = "2000",
                    msg = "Unable to process Request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Create new request type

        #region Update Request type

        public ActionResult UpdateReqtype(RequestTypeInfo update, string id4update)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) { return RedirectToAction("Logout", "Userlogin"); }

            try
            {
                var db = new ORtoMySql();
                int isAMApproval = (update.isAMApproval == true) ? 1 : 0;
                int isRMApproval = (update.isRMApproval == true) ? 1 : 0;
                int isGMApproval = (update.isGMApproval == true) ? 1 : 0;
                int isDivManApproval = (update.isDivManApproval == true) ? 1 : 0;
                int isDivManApproval2 = (update.isDivManApproval2 == true) ? 1 : 0;
                int isDivManApproval3 = (update.isDivManApproval3 == true) ? 1 : 0;
                int isPresidentApproval = (update.isPresidentApproval == true) ? 1 : 0;

                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestType SET " +
                    "reqType = @reqtype, isAMApproval = @isAMApproval , isRMApproval = @isRMApproval , isGMApproval = @isGMApproval , " +
                    "isDivManApproval = @isDivManApproval, isDivManApproval2 = @DivMan2 , isDivManApproval3 = @DivMan3, " +
                    "DivCode1 = @DivCode1 , DivCode2 = @DivCode2 , DivCode3 = @DivCode3, isPresidentApproval = @isPresidentApproval, sysmodified  = @sysmodified, sysmodifier = @sysmodifier WHERE ID = @id";
                    cmd.Parameters.AddWithValue("@id", id4update);
                    cmd.Parameters.AddWithValue("@reqtype", update.RequestType);
                    cmd.Parameters.AddWithValue("@isAMApproval", isAMApproval);
                    cmd.Parameters.AddWithValue("@isRMApproval", isRMApproval);
                    cmd.Parameters.AddWithValue("@isGMApproval", isGMApproval);
                    cmd.Parameters.AddWithValue("@isDivManApproval", isDivManApproval);
                    cmd.Parameters.AddWithValue("@DivMan2", isDivManApproval2);
                    cmd.Parameters.AddWithValue("@DivMan3", isDivManApproval3);
                    cmd.Parameters.AddWithValue("@DivCode1", update.DivCode1);
                    cmd.Parameters.AddWithValue("@DivCode2", update.DivCode2);
                    cmd.Parameters.AddWithValue("@DivCode3", update.DivCode3);
                    cmd.Parameters.AddWithValue("@isPresidentApproval", isPresidentApproval);
                    cmd.Parameters.AddWithValue("@sysmodified", sysmodified.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("sysmodifier", ss.s_fullname);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    return Json(new
                    {
                        status = true,
                        rescode = "2000",
                        msg = "Request type has been updated."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    rescode = "2000",
                    msg = "Unable to process Request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Update Request type

        #region Get new type ID

        private string getNewTypeID()
        {
            try
            {
                var db = new ORtoMySql();
                int fromDB;
                int n;
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT RIGHT(TypeID, 5) AS TypeID FROM OnlineRequest.requestType ORDER BY TypeID DESC LIMIT 1";
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    var read = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (read.Read())
                    {
                        fromDB = Convert.ToInt32(read["TypeID"].ToString().Trim());
                        n = fromDB + 1;
                        return n.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        n = 00001;
                        return n.ToString().PadLeft(5, '0');
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        #endregion Get new type ID

        #region Request type Checking if exists

        public Boolean reqTypeChecking(string RequestType, string zonecode)
        {
            try
            {
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT reqType FROM OnlineRequest.requestType WHERE reqType = @reqtype and ZoneCode = @zcode";
                    cmd.Parameters.AddWithValue("@zcode", zonecode);
                    cmd.Parameters.AddWithValue("@reqtype", RequestType.Trim());
                    con.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (rdr.Read())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        #endregion Request type Checking if exists

        [Route("Division")]

        #region Retrieve all division approvers

        public ActionResult Division(DivisionInfo info)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                var aList = new List<AddDivision>();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.division WHERE zonecode = @zcode ORDER BY syscreated ASC";
                    cmd.Parameters.AddWithValue("@zcode", ss.s_zonecode);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            var a = new AddDivision();
                            a.DivisionID = rdr["id"].ToString().Trim();
                            a.DivisionAcro = rdr["DivAcro"].ToString().Trim();
                            a.DivisionName = rdr["DivName"].ToString().Trim();
                            a.DivisionCode = rdr["DivCode"].ToString().Trim();
                            a.ZoneCode = rdr["ZoneCode"].ToString().Trim();
                            a.DivisionManager = rdr["DivManager"].ToString().Trim();
                            a.ApproverResID = rdr["ApproverResID"].ToString().Trim();
                            aList.Add(a);
                        }
                    }
                    info.IRDivisions = que.Get_IRDivisions(ss.s_zonecode).ToList();
                    info._info = aList;
                    return View(info);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        #endregion Retrieve all division approvers

        #region Update division approvers

        public ActionResult UpdateDivision(DivisionInfo dinfo)
        {
            try
            {
                var ss = (ORSession)Session["UserSession"];
                if (que.Check_DivApproverResID(dinfo.ApproverResID, ss.s_zonecode) == true)
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Approver's Resource ID already exists"
                    }, JsonRequestBehavior.AllowGet);
                }

                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();

                    cmd.CommandText = "UPDATE OnlineRequest.division SET DivAcro = @DivAcro, DivCode = @DivCode ,DivName = @DivName ,ZoneCode = @ZoneCode , DivManager = @DivManager , ApproverResID = @ApproverResID, sysmodified = @sysmodified, sysmodifier = @sysmodifier WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", dinfo.DivisionID.Trim());
                    cmd.Parameters.AddWithValue("@DivAcro", dinfo.DivisionAcro.Trim());
                    cmd.Parameters.AddWithValue("@DivCode", dinfo.DivisionCode.Trim());
                    cmd.Parameters.AddWithValue("@DivName", dinfo.DivisionName.Trim());
                    cmd.Parameters.AddWithValue("@ZoneCode", dinfo.ZoneCode.Trim());
                    cmd.Parameters.AddWithValue("@DivManager", dinfo.DivisionManager);
                    cmd.Parameters.AddWithValue("@ApproverResID", dinfo.ApproverResID);
                    cmd.Parameters.AddWithValue("@sysmodified", sysmodified.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@sysmodifier", ss.s_fullname);
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();

                    log.Info("A division approver has been updated || Division name: " + dinfo.DivisionName + " ||Division Approver: " + dinfo.DivisionManager + " || Division approver's resource ID: " + dinfo.ApproverResID);
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully Updated."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = "Unable to process request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Update division approvers

        #region Create new division

        public ActionResult AddDivision(DivisionInfo d)
        {
            try
            {
                var db = new ORtoMySql();
                object divcodeResult = "";
                string costCenter = "";
                var ss = (ORSession)Session["UserSession"];
                if (que.Check_DivApproverResID(d.ApproverResID, ss.s_zonecode) == true)
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Approver's Resource ID already exists"
                    }, JsonRequestBehavior.AllowGet);
                }
                using (var con = db.getConnection())
                {
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "INSERT INTO OnlineRequest.division (DivAcro, DivID, DivName ,DivCode, ZoneCode, CostCenter, DivManager, ApproverResID,syscreated,syscreator) VALUES " +
                        " (@DivAcro, @DivID, @DivName, @DivCode, @ZoneCode, @CostCenter, @DivManager, @ApproverResID,  @syscreated, @syscreator)";

                    divcodeResult = que.getIRDivCode(d.DivisionAcro.Trim());
                    costCenter = que.GetDivisionCostCenter(d.DivisionAcro.Trim());
                    d.DivisionCode = divcodeResult.ToString();

                    cmd.Parameters.AddWithValue("@DivAcro", d.DivisionAcro.Trim());
                    cmd.Parameters.AddWithValue("@DivID", d.DivisionID);
                    cmd.Parameters.AddWithValue("@DivName", d.DivisionName.Trim());
                    cmd.Parameters.AddWithValue("@DivCode", d.DivisionCode);
                    cmd.Parameters.AddWithValue("@ZoneCode", d.ZoneCode.Trim());
                    cmd.Parameters.AddWithValue("@CostCenter", costCenter);
                    cmd.Parameters.AddWithValue("@DivManager", d.DivisionManager.Trim());
                    cmd.Parameters.AddWithValue("@ApproverResID", d.ApproverResID.Trim());
                    cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@syscreator", ss.s_fullname);
                    var divCode = que.CheckingDivCode(d.DivisionAcro, d.DivisionCode);
                    if (divCode == true)
                    {
                        return Json(new
                        {
                            status = false,
                            msg = "Division already exists."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandTimeout = 0;
                }

                log.Info("A new division approver has been added: || Division name: " + d.DivisionName + " || Division manager:" + d.DivisionManager + " || Division approver's resource ID: " + d.ApproverResID);
                return Json(new
                {
                    status = true,
                    msg = "Division approver has been added."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = "Unable to process request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Create new division

        #region Retrieve all department approvers

        public ActionResult Department()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var dp = new DepartmentInfo();
                dp = que.GetDepartmentList(ss.s_zonecode);
                dp.IRDivisions = que.Get_OnlineRequestDivisions(ss.s_zonecode);

                return View(dp);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        #endregion Retrieve all department approvers

        #region Create new department approver

        public ActionResult AddDepartment(DepartmentInfo data)
        {
            var ss = (ORSession)Session["UserSession"];
            if (que.Check_DeptApproverResID(data.approver_resID, ss.s_zonecode) == true)
            {
                return Json(new
                {
                    status = false,
                    msg = "Approver's Resource ID already exists"
                }, JsonRequestBehavior.AllowGet);
            }
            object divcode = que.getIRDivCode(data.divacro);
            data.divcode = divcode.ToString();
            data.deptCode = que.GetDivisionCostCenter(data.divacro);
            var Resp = que.AddNewDepartment(data, ss);

            if (Resp == "success")
            {
                return Json(new
                {
                    status = true,
                    msg = "New Department Approver has been added."
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = false,
                    msg = "Unable to process request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Create new department approver

        #region Update department approver

        public ActionResult UpdateDepartment(DepartmentInfo data)
        {
            var ss = (ORSession)Session["UserSession"];
            if (que.Check_DeptApproverResID(data.approver_resID, ss.s_zonecode) == true)
            {
                return Json(new
                {
                    status = false,
                    msg = "Approver's Resource ID already exists"
                }, JsonRequestBehavior.AllowGet);
            }
            var Resp = que.UpdateDepartment(data, ss);

            if (Resp == "success")
            {
                return Json(new
                {
                    status = true,
                    msg = "Department successfully updated"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = false,
                    msg = "Unable to process request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Update department approver

        #region Get Resource ID

        public JsonResult GetResourceID(string fullname)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                string ResourceID = que.GetDivisionApproverResourceID(ss.s_zonecode, fullname);
                return Json(new
                {
                    status = true,
                    data = ResourceID
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    status = false,
                    msg = "Unable to process request."
                }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }

        #endregion Get Resource ID

        public bool getIssuanceNum()
        {
            var db = new ORtoMySql();
            bool res = false;
            string yr, mo, series;

            yr = DateTime.Now.ToString().Remove(0, 4);
            mo = DateTime.Now.ToString().PadLeft(2, '0');

            using (var con = db.getConnection())
            {
                try
                {
                    var cmd = new MySqlCommand("select series from issuanceNoSettings where year = @yr and month = @mo order by series desc limit 1", con);
                    cmd.Parameters.AddWithValue("@yr", yr);
                    cmd.Parameters.AddWithValue("@mo", mo);
                    cmd.CommandTimeout = 0;
                    con.Open();
                    using (var read = cmd.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            series = read["series"].ToString().Trim();
                            var ser = Convert.ToInt32(series) + 1;
                            string seriess = ser.ToString();
                            Session["issuanceSeries"] = seriess.PadLeft(5, '0');
                            res = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Fatal(e.Message, e);
                    throw;
                }
                finally
                {
                    con.Close();
                }
                return res;
            }
        }

        public bool newIssuanceNum()
        {
            var db = new ORtoMySql();
            string year, month;
            year = DateTime.Now.Year.ToString().Remove(0, 4);
            month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            try
            {
                using (var con = db.getConnection())
                {
                    string series = Session["issuanceSeries"].ToString();
                    var cmd = new MySqlCommand("insert into issuanceNoSettings (month,year,series) values (@month,@year,@series)", con);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@series", series);
                    cmd.CommandTimeout = 0;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                log.Fatal(e.Message, e);
                throw;
            }
        }

        public bool issuanceNum(string reqNo)
        {
            string series = "";
            string seriesNum;
            if (getIssuanceNum() == false)
            {
                Session["issuanceSeries"] = "00001";
            }
            series = Session["issuanceSeries"].ToString();
            newIssuanceNum();
            var db = new ORtoMySql();
            string year, month;
            year = DateTime.Now.Year.ToString().Remove(0, 4);
            month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            try
            {
                using (var con = db.getConnection())
                {
                    seriesNum = year + month + series;
                    int sn = Convert.ToInt32(seriesNum);
                    var cmd = new MySqlCommand("Update onlineRequest_Open SET issuanceNo=@sn where reqNumber=@reqNo", con);
                    cmd.Parameters.AddWithValue("@reqNo", reqNo);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@sn", sn);
                    cmd.CommandTimeout = 0;
                    con.Open();
                    cmd.ExecuteNonQuery();

                    log.Info("Successfully inserted issuance num: " + sn);
                    return true;
                }
            }
            catch (Exception e)
            {
                log.Fatal(e.Message, e);
                throw;
            }
        }

        public ViewResult VPAssistant()
        {
            var info = new VPassistantInfo();
            info = que.GetVPAssistants();
            return View(info);
        }

        public JsonResult UpdateVPAssistant(VPassistantInfo info)
        {
            if (que.UpdateVPAssistant(info) == "success")
            {
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = false,
                    msg = "Error, Unable to process request."
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}