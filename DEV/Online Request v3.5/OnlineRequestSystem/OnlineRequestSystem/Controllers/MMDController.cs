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

        public ActionResult viewMMDStatus(string selected, string type, string retUrl, string office)
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
                    if (type == "PO")
                    {
                        if ((new[] { "VISMIN", "VISAYAS", "MINDANAO" }).Contains(mySession.s_zonecode))
                        {
                            switch (selected)
                            {
                                case "PROCESSED PO":

                                    #region PP Query

                                    cmd.CommandText = "SELECT a.reqNumber, a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID  " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                                  " AND b.isPO_Approved = 1 " +
                                  " AND b.isDelivered = 0 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion PP Query

                                    break;

                                case "RECEIVED FROM SUPPLIER":

                                    #region RFS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE  a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                                  " AND b.isPO_Approved = 1 " +
                                  " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion RFS Query

                                    break;

                                case "IN TRANSIT-SDC":

                                    #region ITS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                                  " AND b.isPO_Approved = 1 " +
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

                                    cmd.CommandText = "SELECT a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount,  a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                                  " AND b.isPO_Approved = 1 " +
                                  " AND b.isDelivered = 0 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion PP Query

                                    break;

                                case "RECEIVED FROM SUPPLIER":

                                    #region RFS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                                  " AND b.isPO_Approved = 1 " +
                                  " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion RFS Query

                                    break;

                                case "IN TRANSIT-SDC":

                                    #region ITS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                                  " AND b.isPO_Approved = 1 " +
                                  " AND b.isDelivered = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion ITS Query

                                    break;

                                default:
                                    break;
                            }
                        }

                        Info.POurl = "PO";

                    }
                    else
                    {
                        if ((new[] { "VISMIN", "VISAYAS", "MINDANAO" }).Contains(mySession.s_zonecode))
                        {
                            switch (selected)
                            {
                                case "PROCESSED PO":

                                    #region PP Query

                                    cmd.CommandText = "SELECT a.reqNumber, a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +

                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID  " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                                  " AND b.isPO_Approved = 2 " +
                                  " AND b.isDelivered = 0 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion PP Query

                                    break;

                                case "RECEIVED FROM SUPPLIER":

                                    #region RFS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE  a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                                  " AND b.isPO_Approved = 2 " +
                                  " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'VISMIN' OR a.ZoneCode = 'VISAYAS' OR a.ZoneCode = 'MINDANAO') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion RFS Query

                                    break;

                                case "IN TRANSIT-SDC":

                                    #region ITS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                                  " AND b.isPO_Approved = 2 " +
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

                                    cmd.CommandText = "SELECT a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount,  a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDProcessed = 1 AND b.isMMDTransit = 0 " +
                                  " AND b.isPO_Approved = 2 " +
                                  " AND b.isDelivered = 0 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion PP Query

                                    break;

                                case "RECEIVED FROM SUPPLIER":

                                    #region RFS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isDelivered = 0 AND b.isMMDTransit = 1 " +
                                  " AND b.isPO_Approved = 2 " +
                                  " AND b.isMMDProcessed = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion RFS Query

                                    break;

                                case "IN TRANSIT-SDC":

                                    #region ITS Query

                                    cmd.CommandText = "SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.OverallTotalPrice, a.reqDate, a.TypeID , " +
                                  " (SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.TotalQty, a.isDivRequest, a.DeptCode, " +
                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                  " WHERE a.isDivRequest = @office AND b.isMMDTransit = 1 AND b.isMMDProcessed = 1 " +
                                  " AND b.isPO_Approved = 2 " +
                                  " AND b.isDelivered = 1 AND (a.ZoneCode = 'LUZON' OR a.ZoneCode = 'LNCR' OR a.ZoneCode = 'NCR') " +
                                  " GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";

                                    #endregion ITS Query

                                    break;

                                default:
                                    break;
                            }
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
                            o.OverallTotalPrice = rdr["OverallTotalPrice"].ToString().Trim();
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
                ViewBag.ProcessedPO = 1;
                Info.returnUrl = retUrl;

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

        public ActionResult ProcessedPO(string ReqNo, string OverallTotal, List<string> TotalP, List<string> desc, string allStockStat)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            if (ss == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                using (var conn = db.getConnection())
                {
                    if (allStockStat == "0")
                    {
                        var cmd = conn.CreateCommand();
                        conn.Open();

                        for (var i = 0; i < TotalP.Count; i++)
                        {

                            var totalP = Convert.ToDecimal(TotalP[i]);
                            var descc = desc[i].ToString().Trim();

                            cmd.CommandText = "UPDATE OnlineRequest.requestItems SET TotalPrice = @TotalPrice, StatusOfStock = @StatusOfStock WHERE reqNumber = @reqNumber1 AND itemDescription = @desc";
                            cmd.Parameters.AddWithValue("@TotalPrice", totalP);
                            cmd.Parameters.AddWithValue("@StatusOfStock", 0);
                            cmd.Parameters.AddWithValue("@desc", descc);
                            cmd.Parameters.AddWithValue("@reqNumber1", ReqNo);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Open SET OverallTotalPrice = @OverallTotalPrice WHERE reqNumber = @reqNumber2";
                        cmd.Parameters.AddWithValue("@OverallTotalPrice", Convert.ToDecimal(OverallTotal));
                        cmd.Parameters.AddWithValue("@reqNumber2", ReqNo);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed WHERE reqNumber = @ReqNo3";
                        cmd.Parameters.AddWithValue("@ReqNo3", ReqNo);
                        cmd.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isMMDProcessed", 1);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    else
                    {
                        var cmd = conn.CreateCommand();
                        conn.Open();

                        for (var i = 0; i < desc.Count; i++)
                        {
                            var descc = desc[i].ToString().Trim();

                            cmd.CommandText = "UPDATE OnlineRequest.requestItems SET StatusOfStock = @StatusOfStock WHERE reqNumber = @reqNumber1 AND itemDescription = @desc";
                            cmd.Parameters.AddWithValue("@StatusOfStock", 1);
                            cmd.Parameters.AddWithValue("@desc", descc);
                            cmd.Parameters.AddWithValue("@reqNumber1", ReqNo);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed, isPO_Approved = @isPO_Approved WHERE reqNumber = @ReqNo3";
                        cmd.Parameters.AddWithValue("@ReqNo3", ReqNo);
                        cmd.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                        cmd.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@isMMDProcessed", 1);
                        cmd.Parameters.AddWithValue("@isPO_Approved", 2);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
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
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
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

        [HttpPost]
        public JsonResult StoreComments(string ReqNo, string comment)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "INSERT INTO OnlineRequest.storedComments (reqNumber, comments, commCreator, commCreatorID, commcreated)" +
                        "VALUES (@reqNumber, @comments, @commCreator, @commCreatorID, @commcreated )";
                    cmd.Parameters.AddWithValue("@reqNumber", ReqNo);
                    cmd.Parameters.AddWithValue("@comments", comment);
                    cmd.Parameters.AddWithValue("@commCreator", ss.s_fullname);
                    cmd.Parameters.AddWithValue("@commCreatorID", ss.s_usr_id);
                    cmd.Parameters.AddWithValue("@commcreated", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Comment added  || request no: " + ReqNo + " || Creator: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Your comment was posted."
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

        //FOR ITEM PRICING: SEARCH PRICE
        public JsonResult SearchItemPrice(string searchCriteria)
        {
            var ss = (ORSession)Session["UserSession"];

            try
            {
                var xlist = new List<ItemsInfo>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/SearchItem?itemDetails=" + searchCriteria);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var resData = JsonConvert.DeserializeObject<ListOfItemsResponse>(x);

                Session["ItemsInfo"] = resData;
                return Json(new
                {
                    data = resData,
                    status = true,
                    msg = "Success!"
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
                throw;
            }
        }

        [HttpPost]
        public ActionResult SplitRequest(string ReqNo, List<string> PricePerItem, string InStock, string OutOfStock)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            CreateReqModels model = new CreateReqModels();

            try
            {
                var InStockObj = JsonConvert.DeserializeObject<List<object>>(InStock);
                var OutOfStockObj = JsonConvert.DeserializeObject<List<object>>(OutOfStock);
                var db = new ORtoMySql();
                var toTC = new CultureInfo("en-US", false).TextInfo;

                var isDiag = set.DiagnosticCheck(ReqNo);

                string[] tables = { "OROpen", "RAS", "OREscalation" };
                List<string> list = new List<string>();
                list.AddRange(tables);

                if (isDiag == 1)
                {
                    list.Add("ORDiag");
                }

                String[] tbl = list.ToArray();

                var table_Name = string.Empty;

                var newReqNo = ReqNo + "-A";

                using (var con = db.getConnection())
                {
                    if (InStockObj.Count != 0 && OutOfStockObj.Count != 0)
                    {
                        using (MySqlCommand cmd = con.CreateCommand())
                        {
                            con.Open();

                            try
                            {
                                for (int i = 0; i < OutOfStockObj.Count; i++)
                                {
                                    string desc = OutOfStockObj[i].ToString();
                                    var totalP = Convert.ToDecimal(PricePerItem[i]);

                                    cmd.CommandText = "UPDATE OnlineRequest.requestItems " +
                                                      "SET reqNumber = @newReqNo, TotalPrice = @TotalPrice, StatusOfStock = @StatusOfStock " +
                                                      "WHERE reqNumber = @ReqNo AND itemDescription = @desc";

                                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                                    cmd.Parameters.AddWithValue("@newReqNo", newReqNo);
                                    cmd.Parameters.AddWithValue("@desc", desc);
                                    cmd.Parameters.AddWithValue("@TotalPrice", totalP);
                                    cmd.Parameters.AddWithValue("@StatusOfStock", 0);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }

                                for (int j = 0; j < InStockObj.Count; j++)
                                {
                                    string desc = InStockObj[j].ToString();

                                    cmd.CommandText = "UPDATE OnlineRequest.requestItems " +
                                                      "SET StatusOfStock = @StatusOfStock " +
                                                      "WHERE reqNumber = @ReqNo2 AND itemDescription = @desc";

                                    cmd.Parameters.AddWithValue("@ReqNo2", ReqNo);
                                    cmd.Parameters.AddWithValue("@desc", desc);
                                    cmd.Parameters.AddWithValue("@StatusOfStock", 1);
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();
                                }

                                con.Close();
                            }
                            catch (MySqlException x)
                            {
                                log.Fatal(x.Message, x);
                                con.Close();

                                return Json(new
                                {
                                    status = false,
                                    msg = x.Message
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        using (MySqlCommand cmd2 = con.CreateCommand())
                        {
                            con.Open();

                            try
                            {
                                for (var i = 0; i < tbl.Length; i++)
                                {
                                    if (tbl[i] == "OROpen")
                                    {
                                        table_Name = "OnlineRequest.onlineRequest_Open";
                                    }
                                    else if (tbl[i] == "RAS")
                                    {
                                        table_Name = "OnlineRequest.requestApproverStatus";
                                    }
                                    else if (tbl[i] == "OREscalation")
                                    {
                                        table_Name = "OnlineRequest.onlineRequest_Escalation";
                                    }
                                    else
                                    {
                                        table_Name = "OnlineRequest.diagnosticFiles";
                                    }

                                    cmd2.CommandText = "INSERT INTO " + table_Name + " ( " + SplitReqQueries(tbl[i]) + " ) " +
                                                      "SELECT CONCAT( reqNumber , '-A') AS " + SplitReqQueries(tbl[i]) +
                                                      " FROM " + table_Name + " WHERE reqNumber = @ReqNo2";

                                    cmd2.Parameters.AddWithValue("@ReqNo2", ReqNo);
                                    cmd2.ExecuteNonQuery();
                                    cmd2.Parameters.Clear();
                                }

                                con.Close();

                                return Json(new
                                {
                                    status = true,
                                    msg = "Success"
                                }, JsonRequestBehavior.AllowGet);
                            }
                            catch (MySqlException x)
                            {
                                log.Fatal(x.Message, x);
                                con.Close();

                                return Json(new
                                {
                                    status = false,
                                    msg = x.Message
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        //using (MySqlCommand cmd3 = con.CreateCommand())
                        //{
                        //    con.Open();

                        //    try
                        //    {
                        //        cmd3.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed, isPO_Approved = @isPO_Approved WHERE reqNumber = @ReqNo3";
                        //        cmd3.Parameters.AddWithValue("@ReqNo3", ReqNo);
                        //        cmd3.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                        //        cmd3.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                        //        cmd3.Parameters.AddWithValue("@isMMDProcessed", 1);
                        //        cmd3.Parameters.AddWithValue("@isPO_Approved", 2);

                        //        cmd3.ExecuteNonQuery();
                        //        cmd3.Parameters.Clear();

                        //        con.Close();

                        //        return Json(new
                        //        {
                        //            status = true,
                        //            msg = "Success"
                        //        }, JsonRequestBehavior.AllowGet);
                        //    }
                        //    catch (MySqlException x)
                        //    {
                        //        log.Fatal(x.Message, x);
                        //        con.Close();

                        //        return Json(new
                        //        {
                        //            status = false,
                        //            msg = x.Message
                        //        }, JsonRequestBehavior.AllowGet);
                        //    }
                        //}
                    }
                    //else if (InStockObj.Count == 0 && OutOfStockObj.Count != 0)
                    //{
                    //    using (MySqlCommand cmd = con.CreateCommand())
                    //    {
                    //        con.Open();

                    //        try
                    //        {
                    //            for (int i = 0; i < OutOfStockObj.Count; i++)
                    //            {
                    //                string desc = OutOfStockObj[i].ToString();
                    //                cmd.CommandText = "UPDATE OnlineRequest.requestItems " +
                    //                                  "SET reqNumber = CONCAT(reqNumber, '-A') " +
                    //                                  "WHERE reqNumber = @ReqNo AND itemDescription = @desc";

                    //                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    //                cmd.Parameters.AddWithValue("@desc", desc);
                    //                cmd.ExecuteNonQuery();
                    //                cmd.Parameters.Clear();
                    //            }

                    //            con.Close();
                    //        }
                    //        catch (MySqlException x)
                    //        {
                    //            log.Fatal(x.Message, x);
                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = false,
                    //                msg = x.Message
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }

                    //    }
                    //    using (MySqlCommand cmd2 = con.CreateCommand())
                    //    {
                    //        con.Open();

                    //        try
                    //        {
                    //            cmd2.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed WHERE reqNumber = @ReqNo2";
                    //            cmd2.Parameters.AddWithValue("@ReqNo2", ReqNo);
                    //            cmd2.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                    //            cmd2.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    //            cmd2.Parameters.AddWithValue("@isMMDProcessed", 1);

                    //            cmd2.ExecuteNonQuery();
                    //            cmd2.Parameters.Clear();

                    //            con.Close();
                    //        }
                    //        catch (MySqlException x)
                    //        {
                    //            log.Fatal(x.Message, x);
                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = false,
                    //                msg = x.Message
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                    //    using (MySqlCommand cmd3 = con.CreateCommand())
                    //    {
                    //        con.Open();

                    //        try
                    //        {
                    //            for (var i = 0; i < tbl.Length; i++)
                    //            {
                    //                if (tbl[i] == "OROpen")
                    //                {
                    //                    table_Name = "OnlineRequest.onlineRequest_Open";
                    //                }
                    //                else if (tbl[i] == "RAS")
                    //                {
                    //                    table_Name = "OnlineRequest.requestApproverStatus";
                    //                }
                    //                else
                    //                {
                    //                    table_Name = "OnlineRequest.onlineRequest_Escalation";
                    //                }

                    //                cmd3.CommandText = "UPDATE " + table_Name + " " +
                    //                                   "SET reqNumber = CONCAT(reqNumber, '-A') " +
                    //                                   "WHERE reqNumber = @ReqNo3";

                    //                cmd3.Parameters.AddWithValue("@ReqNo3", ReqNo);
                    //                cmd3.ExecuteNonQuery();
                    //                cmd3.Parameters.Clear();
                    //            }

                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = true,
                    //                msg = "Success"
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //        catch (MySqlException x)
                    //        {
                    //            log.Fatal(x.Message, x);
                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = false,
                    //                msg = x.Message
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //    }
                    //}
                    //else if (InStockObj.Count != 0 && OutOfStockObj.Count == 0)
                    //{
                    //    using (MySqlCommand cmd = con.CreateCommand())
                    //    {
                    //        con.Open();

                    //        try
                    //        {
                    //            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET MMD_Processor = @MMDProcessor, MMD_Processed_Date = @ProcessedDate, isMMDProcessed = @isMMDProcessed, isPO_Approved = @isPO_Approved WHERE reqNumber = @ReqNo";
                    //            cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    //            cmd.Parameters.AddWithValue("@MMDProcessor", ss.s_usr_id);
                    //            cmd.Parameters.AddWithValue("@ProcessedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    //            cmd.Parameters.AddWithValue("@isMMDProcessed", 1);
                    //            cmd.Parameters.AddWithValue("@isPO_Approved", 2);

                    //            cmd.ExecuteNonQuery();
                    //            cmd.Parameters.Clear();

                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = true,
                    //                msg = "Success"
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }
                    //        catch (MySqlException x)
                    //        {
                    //            log.Fatal(x.Message, x);
                    //            con.Close();

                    //            return Json(new
                    //            {
                    //                status = false,
                    //                msg = x.Message
                    //            }, JsonRequestBehavior.AllowGet);
                    //        }

                    //    }
                    //}
                    else
                    {
                        string err = "Unable to process request.";
                        log.Fatal(err);
                        return Json(new
                        {
                            status = false,
                            msg = err
                        }, JsonRequestBehavior.AllowGet);
                    }
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
                throw;
            }

        }

        public string GetID(string table, string desc, string ReqNo)
        {
            string id = string.Empty;
            var db = new ORtoMySql();
            try
            {
                using (MySqlConnection con = db.getConnection())
                {
                    con.Open();

                    switch (table)
                    {
                        case "OROpen":
                            using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @ReqNo", con))
                            {
                                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                rdr.Read();
                                id = rdr["id"].ToString();
                            }
                            break;

                        case "RAS":
                            using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM OnlineRequest.requestApproverStatus WHERE reqNumber = @ReqNo", con))
                            {
                                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                rdr.Read();
                                id = rdr["id"].ToString();
                            }
                            break;

                        case "OREscalation":
                            using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM OnlineRequest.onlineRequest_Escalation WHERE reqNumber = @ReqNo", con))
                            {
                                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                rdr.Read();
                                id = rdr["id"].ToString();
                            }
                            break;

                        case "reqItems":
                            using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo", con))
                            {
                                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                                MySqlDataReader rdr = cmd.ExecuteReader();
                                rdr.Read();
                                id = rdr["id"].ToString();
                            }
                            break;
                    }
                    con.Close();
                }

                return id;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return x.Message;
            }
        }
        public string SplitReqQueries(string table)
        {
            string query = string.Empty;

            switch (table)
            {
                case "OROpen":
                    query = "reqNumber,reqDate,reqCreator,TypeID,reqDescription,OverallTotalPrice,reqTotal, " +
                            "BranchCode,Region,Area,AreaCode,DivCode,Zonecode,reqStatus,isApproved,syscreated,syscreator,sysmodified, " +
                            "sysmodifier,isDivRequest,TotalQty,TotalQty_MMD,TotalQty_SDC,TotalQty_Branch,TotalQty_Div,DeptCode,issuanceNo, " +
                            "forPresident,pendingDate,pendingDateExpire";
                    break;

                case "RAS":
                    query = "reqNumber,DM_Approver,DM_Approved_Date,isApprovedDM,LocalDiv_Approver,LocalDiv_Approved_Date,isApprovedLocalDiv, " +
                            "AM_Approver,AM_Approved_Date,isApprovedAM,RM_Approver,RM_Approved_Date,isApprovedRM,VPAssistant_Approver, " +
                            "VPAssistant_Approved_Date,isApprovedVPAssistant,GM_Approver,GM_Approved_Date,isApprovedGM,Pres_Approver, " +
                            "Pres_Approved_Date,isApprovedPres,Div_Approver1,DivCode1,Div_Approved_Date1,isApprovedDiv1,Div_Approver2,DivCode2, " +
                            "Div_Approved_Date2,isApprovedDiv2,Div_Approver3,DivCode3,Div_Approved_Date3,isApprovedDiv3,VPO_PO_Approver, " +
                            "VPO_PO_Approved_Date,isVPO_PO_Approved,Pres_PO_Approver,Pres_PO_Approved_Date,isPres_PO_Approved,isPO_Approved, " +
                            "MMD_Approver,MMD_Approved_Date,isApprovedMMD,MMD_Processor,MMD_Processed_Date,isMMDProcessed,MMD_Deliverer, " +
                            "MMD_Delivered_Date,isDelivered,MMD_Transitor,MMD_Transit_Date,isMMDTransit,RM_Receiver,RM_Received_Date,isRMReceived, " +
                            "RM_Transitor,RM_Transit_Date,isRMTransit";

                    break;

                case "OREscalation":
                    query = "reqNumber,EscalationAM_Name,EscalationAM_Date,EscalationAM_Remarks,EscalationRM_Name,EscalationRM_Date, " +
                            "EscalationRM_Remarks,EscalationVPAssistant,EscalationVPAssistant_Date,EscalationVPAssistant_Remarks, " +
                            "EscalationGM_Name,EscalationGM_Date,EscalationGM_Remarks,EscalationPres_Name,EscalationPres_Date, " +
                            "EscalationPres_Remarks,EscalationDiv_Name,EscalationDiv_Date,EscalationDiv_Remarks,EscalationDiv2_Name, " +
                            "EscalationDiv2_Date,EscalationDiv2_Remarks,EscalationDiv3_Name,EscalationDiv3_Date,EscalationDiv3_Remarks, " +
                            "EscalationDM_Name,EscalationDM_Date,EscalationDM_Remarks,EscalationLocalDiv_Name,EscalationLocalDiv_Date, " +
                            "EscalationLocalDiv_Remarks,EscalationVPO_PO_Name,EscalationVPO_PO_Date,EscalationVPO_PO_Remarks, " +
                            "EscalationPres_PO_Name,EscalationPres_PO_Date,EscalationPres_PO_Remarks";
                    break;

                case "ORDiag":
                    query = "reqNumber,diagnostic,syscreated,syscreator";
                    break;

                default:
                    break;
            }

            return query;
        }
        public bool UpdateStatusOfStocks(string ReqNo, string description, string status)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            string desc = description.Trim();

            try
            {
                using (var conn = db.getConnection())
                {
                    conn.Open();

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestItems SET StatusOfStock = @StatusOfStock WHERE reqNumber = @ReqNo AND itemDescription = @description;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@description", desc);
                    cmd.Parameters.AddWithValue("@StatusOfStock", Convert.ToInt32(status));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    conn.Close();
                }

                log.Info("Status of stocks has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);

                return true;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message);
                return false;
            }
        }

        public string GetInStockStat(string ReqNo)
        {
            var db = new ORtoMySql();
            string stat = string.Empty;

            try
            {
                using (var conn = db.getConnection())
                {
                    conn.Open();

                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT (IF(COUNT(StatusOfStock = '1') = COUNT(itemDescription), 'TRUE', 'FALSE')) " +
                                      "AS Stat FROM OnlineRequest.requestItems WHERE reqNumber = @ReqNo ;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.CommandTimeout = 0;

                    using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        read.Read();
                        stat = read["Stat"].ToString();
                    }

                    conn.Close();
                }

                return stat;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message);
                return "Err";
            }
        }
    }
}
