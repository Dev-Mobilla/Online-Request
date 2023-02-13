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
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class SDCController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SDCController));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private Helper que = new Helper();
        private string format = "yyyy/MM/dd HH:mm:ss";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SDCReport()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            var objReportType = new List<SelectListItem>() {
                new SelectListItem { Text="Daily", Value="Daily"},
                new SelectListItem { Text="Monthly", Value="Monthly"}
            };
            ViewBag.ReportType = objReportType;
            return View();
        }

        public ActionResult SDC()
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            try
            {
                var xlist = new List<SDCInfo>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetListOfSDC/?CostCenter=" + ss.s_costcenter);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var resData = JsonConvert.DeserializeObject<ListOfSDCResponse>(x);
                foreach (var item in resData.ListOfSDC)
                {
                    xlist.Add(new SDCInfo
                    {
                        ZoneCode = item.ZoneCode,
                        Region = item.Region,
                        RM = item.RM,
                        BranchCode = item.BranchCode,
                        BranchName = item.BranchName,
                    });
                }

                return View(resData);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        [Route("sdc")]
        public ActionResult viewSDCStatus(string selected, OpenReqInfo Info)
        {
            ORtoMySql db = new ORtoMySql();
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                using (MySqlConnection conn = db.getConnection())
                {
                    var toTC = new CultureInfo("en-US", false).TextInfo;
                    var OpenReqList = new List<OpenReqViewModel>();
                    using (var cmd = conn.CreateCommand())
                    {

                        switch (selected)
                        {
                            case "TO BE RECEIVED":
                                cmd.CommandText = " SELECT  a.reqNumber,  COUNT(d.itemDescription) AS TotalCount, a.reqCreator , a.reqDescription, a.reqDate, a.TypeID , " +
                                                  " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.isDivRequest, a.DeptCode," +
                                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                                  " WHERE b.isRMReceived = 0 AND b.isDelivered = 1 " +
                                                  " AND isRMTransit = 0 AND a.isDivRequest = 0 AND a.ZoneCode = @zoneCode AND a.region = @region GROUP BY a.reqNumber ORDER BY a.sysmodified ASC ";
                                cmd.Parameters.AddWithValue("@zoneCode", ss.s_zonecode);
                                cmd.Parameters.AddWithValue("@region", ss.s_region);
                                break;

                            case "RECEIVED":
                                cmd.CommandText = " SELECT  a.reqNumber,  COUNT(d.itemDescription) AS TotalCount,  a.reqCreator , a.reqDescription, a.reqDate, a.TypeID , " +
                                                  " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.isDivRequest, a.DeptCode," +
                                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                                  " WHERE b.isDelivered = 1 AND b.isRMReceived = 1 " +
                                                  " AND isRMTransit = 0 AND a.isDivRequest = 0 AND a.ZoneCode = @zoneCode AND a.region = @region GROUP BY a.reqNumber ORDER BY a.sysmodified ASC ";
                                cmd.Parameters.AddWithValue("@zoneCode", ss.s_zonecode);
                                cmd.Parameters.AddWithValue("@region", ss.s_region);
                                break;

                            case "IN TRANSIT-BRANCH":
                                cmd.CommandText = " SELECT  a.reqNumber,  COUNT(d.itemDescription) AS TotalCount, a.reqCreator , a.reqDescription, a.reqDate, a.TypeID , " +
                                                  " ( SELECT itemDescription FROM requestItems WHERE reqnumber = a.reqnumber ORDER BY itemDescription ASC LIMIT 1  ) AS Description, " +
                                                  " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.isDivRequest, a.DeptCode, a.TotalQty," +
                                                  " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                                  " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                                  " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                                  " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber " +
                                                  " WHERE b.isDelivered = 1 AND b.isRMReceived = 1 AND " +
                                                  " b.isRMTransit = 1  AND a.isDivRequest = 0 AND a.ZoneCode = @zoneCode AND a.region = @region GROUP BY a.reqNumber ORDER BY a.sysmodified ASC ";
                                cmd.Parameters.AddWithValue("@zoneCode", ss.s_zonecode);
                                cmd.Parameters.AddWithValue("@region", ss.s_region);
                                break;
                        }

                        conn.Open();
                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var o = new OpenReqViewModel();
                                o.reqNumber = rdr["reqNumber"].ToString().Trim();
                                o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                o.reqDescription = rdr["reqDescription"].ToString().Trim();
                                o.reqDate = rdr["reqDate"].ToString().Trim();
                                o.TypeID = rdr["TypeID"].ToString().Trim();
                             
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

                                o.TotalItems = rdr["TotalCount"].ToString().Trim();
                                o.itemDescription = rdr["Description"].ToString().Trim();

                                o.BranchCode = rdr["BranchCode"].ToString().Trim();
                                o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                                o.Region = rdr["Region"].ToString().Trim();
                                o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                                o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower());
                                o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                                o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                                o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);
                                o.RM_Received = Convert.ToInt32(rdr["isRMReceived"]);
                                o.RM_Transit = Convert.ToInt32(rdr["isRMTransit"]);

                                OpenReqList.Add(o);
                            }
                        }

                        Info._OpenInfo = OpenReqList;
                    }
                }

                ViewBag.Msg = selected;
                return View(Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        public string getBranchname(string BranchCode, string Region, string ZoneCode)
        {
            string b = "";
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
                var resData = JsonConvert.DeserializeObject<BranchNameResponse>(x);
                b = resData.BranchName;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return b;
            }
            return b;
        }

        public ActionResult Received(string RM_Receiver, string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET RM_Receiver = @RMReceiver , RM_Received_Date = @ReceivedDate , isRMReceived = @isRMReceived WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@RMReceiver", ss.s_usr_id);
                    cmd.Parameters.AddWithValue("@ReceivedDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@isRMReceived", 1);
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
            que.SetDateModified(ReqNo, ss.s_usr_id);
            log.Info("A request is received at SDC and ready for delivery to requesting branch || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
            return Json(new
            {
                status = true,
                rescode = "2001",
                msg = "Successfully saved."
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InTransitBranch(string RM_Transitor, string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null)
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET RM_Transitor = @RMTransitor , RM_Transit_Date = @RMTransitDate , isRMTransit = @isRMTransit WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@RMTransitor", ss.s_usr_id);
                    cmd.Parameters.AddWithValue("@RMTransitDate", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@isRMTransit", 1);
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
            que.SetDateModified(ReqNo, ss.s_usr_id);
            log.Info("A request ready for delivery to requesting branch || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
            return Json(new
            {
                status = true,
                rescode = "2001",
                msg = "Successfully saved."
            }, JsonRequestBehavior.AllowGet);
        }

        [Route("satellite-distribution-center")]
        public ActionResult viewReceived(OpenReqInfo Info)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var toTC = new CultureInfo("en-US", false).TextInfo;
                    var OpenReqList = new List<OpenReqViewModel>();
                    using (var cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = " SELECT  a.reqNumber,  a.reqCreator, COUNT(d.itemDescription) AS TotalCount, a.reqDescription, a.reqDate, a.TypeID , " +
                                          " a.reqTotal, a.BranchCode, a.Region, a.DivCode,  a.Zonecode, a.reqStatus, a.isDivRequest, a.DeptCode," +
                                          " b.isMMDProcessed, b.isDelivered, b.isMMDTransit, b.isRMReceived, b.isRMTransit FROM onlineRequest_Open a   " +
                                          " INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber  " +
                                          " INNER JOIN requestType c  ON a.TypeID = c.TypeID " +
                                          " INNER JOIN requestItems d ON d.reqNumber = a.reqNumber" +
                                          " WHERE b.isRMReceived = 0 AND b.isDelivered = 1 " +
                                          " AND a.isDivRequest = 0 AND a.ZoneCode = @zoneCode AND a.region = @region GROUP BY a.reqNumber ORDER BY a.syscreated ASC ";
                        cmd.Parameters.AddWithValue("@region", ss.s_region);
                        cmd.Parameters.AddWithValue("@zoneCode", ss.s_zonecode);

                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (rdr.Read())
                            {
                                var o = new OpenReqViewModel();
                                o.reqNumber = rdr["reqNumber"].ToString().Trim();
                                o.reqCreator = toTC.ToTitleCase(rdr["reqCreator"].ToString().Trim().ToLower());
                                o.reqDescription = rdr["reqDescription"].ToString().Trim();
                                o.reqDate = rdr["reqDate"].ToString().Trim();
                                o.TypeID = rdr["TypeID"].ToString().Trim();
                                o.TotalItems = rdr["TotalCount"].ToString().Trim();
                                o.TypeName = que.GetTypeName(o.TypeID);
                                o.BranchCode = rdr["BranchCode"].ToString().Trim();
                                o.ZoneCode = rdr["Zonecode"].ToString().Trim();
                                o.Region = rdr["Region"].ToString().Trim();
                                o.isDivRequest = rdr["isDivRequest"].ToString().Trim();
                                o.BranchName = toTC.ToTitleCase(getBranchname(o.BranchCode, o.Region, o.ZoneCode).ToLower());
                                o.MMD_Processed = Convert.ToInt32(rdr["isMMDProcessed"]);
                                o.MMD_ForDelivery = Convert.ToInt32(rdr["isDelivered"]);
                                o.MMD_InTransit = Convert.ToInt32(rdr["isMMDTransit"]);
                                o.RM_Received = Convert.ToInt32(rdr["isRMReceived"]);
                                o.RM_Transit = Convert.ToInt32(rdr["isRMTransit"]);

                                OpenReqList.Add(o);
                            }
                        }
                        Info._OpenInfo = OpenReqList;
                    }
                }
                ViewBag.Msg = "TO BE RECEIVED";
                return View(Info);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        public ActionResult DownloadMonthly(string monthlyDate, string filename)
        {
            var ss = (ORSession)Session["UserSession"];
            var date = DateTime.Now.ToString("MM/dd/yyyy");
            string monthDate = Session["monthlyDate"].ToString();
            string name = ss.s_fullname.ToUpper().ToString();
            string title = "Monthly Inventory Report";
            var db = new ORtoMySql();
            var con = db.getConnection();

            try
            {
                ReportDataset dt = (ReportDataset)Session["monthlyData"];
                ReportDocument rpt = new ReportDocument();
                var MonthlyInventoryReport = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\inventoryReport.rpt");
                rpt.Load(MonthlyInventoryReport);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("date", date);
                rpt.SetParameterValue("generatedBy", name);
                rpt.SetParameterValue("reportType", title);

                System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                return File(stream, "application/pdf", "Monthly-Inventory-Report" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
            }
            catch (Exception e)
            {
                log.Error("Error in saving monthly inventory report." + e.ToString());
                ViewBag.Error = e.ToString();
                return View("Error");
            }
        }

        public ReportDataset getMonData(DateTime monthlyDate)
        {
            string month = monthlyDate.ToString("MM");
            string year = monthlyDate.ToString("yyyy");

            var db = new ORtoMySql();
            var mySession = (ORSession)Session["UserSession"];

            var con = db.getConnection();
            String sql = "  SELECT m.ItemDescription,SUM(m.qty) AS qty,SUM(m.actualQtySDC) AS actualQtySDC FROM ( " +
                         "  SELECT a.itemDescription,a.qty,a.actualQtySDC " +
                         "  FROM OnlineRequest.requestItems a " +
                         "  INNER JOIN OnlineRequest.requestApproverStatus b ON a.reqNumber=b.reqNumber  " +
                         "  INNER JOIN OnlineRequest.onlineRequest_Open z ON a.reqNumber=z.reqNumber " +
                         "  WHERE b.isRMReceived=1 AND MONTH(b.RM_Received_Date)=@month AND YEAR(b.RM_Received_Date)=@year AND z.ZoneCode=@zonecode " +
                         "  AND z.Region=@region " +
                         "  UNION ALL " +
                         "  SELECT aa.itemDescription,aa.qty,aa.actualQtySDC " +
                         "  FROM OnlineRequest.requestItems aa  " +
                         "  INNER JOIN OnlineRequest.requestApproverStatus bb ON aa.reqNumber=bb.reqNumber  " +
                         "  INNER JOIN OnlineRequest.onlineRequest_Close zz ON aa.reqNumber=zz.reqNumber " +
                         "  WHERE bb.isRMReceived=1 AND MONTH(bb.RM_Received_Date)=@month AND YEAR(bb.RM_Received_Date)=@year AND zz.ZoneCode=@zonecode " +
                         "  AND zz.Region=@region ) m GROUP BY m.ItemDescription;";
            try
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@zonecode", mySession.s_zonecode);
                    cmd.Parameters.AddWithValue("@region", mySession.s_region);
                    MySqlDataAdapter adapter = new MySqlDataAdapter();

                    cmd.Connection = con;
                    adapter.SelectCommand = cmd;

                    using (ReportDataset ds = new ReportDataset())
                    {
                        adapter.Fill(ds, "DataTable3");
                        return ds;
                    }

                }
            }
            catch (Exception e)
            {
                log.Error("Error in fetching data!" + e.ToString());
                throw;
            }
        }

        public JsonResult exportMonthly(DateTime monthlyDate)
        {
            Session.Add("monthlyDate", monthlyDate);
            ReportDataset dt = getMonData(monthlyDate);
            if (dt.DataTable3.Count == 0)
            {
                return Json(new
                {
                    status = false,
                    msg = "Sorry, No records found."
                });
            }
            Session["monthlyData"] = dt;
            try
            {
                return Json(new
                {
                    respcode = 1,
                    status = true,
                    msg = "Successfully and saved monthly inventory report!",
                    respmessage = "MonthlyInventory"
                });
            }
            catch (Exception e)
            {
                log.Error("Error in saving monthly inventory report." + e.ToString());
                return Json(new
                {
                    status = false,
                    msg = "Error, Unable to process requests."
                });
            }
        }

        public ReportDataset getDayData(DateTime dailyDate)
        {
            string day = dailyDate.ToString("yyyy-MM-dd");

            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            var con = db.getConnection();
            var data = new ReportDataset();
            String sql = " SELECT m.ItemDescription,SUM(m.qty) AS qty,SUM(m.actualQtySDC) AS actualQtySDC FROM ( " +
                         " SELECT a.itemDescription,a.qty,a.actualQtySDC " +
                         " FROM OnlineRequest.requestItems a " +
                         " INNER JOIN OnlineRequest.requestApproverStatus b ON a.reqNumber=b.reqNumber " +
                         " INNER JOIN OnlineRequest.onlineRequest_Open z ON a.reqNumber=z.reqNumber " +
                         " WHERE b.isRMReceived=1 AND DATE(b.RM_Received_Date)=@day AND z.ZoneCode=@zonecode " +
                         " AND z.Region=@region " +
                         " UNION ALL " +
                         " SELECT aa.itemDescription,aa.qty,aa.actualQtySDC " +
                         " FROM OnlineRequest.requestItems aa  " +
                         " INNER JOIN OnlineRequest.requestApproverStatus bb ON aa.reqNumber=bb.reqNumber  " +
                         " INNER JOIN OnlineRequest.onlineRequest_Close zz ON aa.reqNumber=zz.reqNumber " +
                         " WHERE bb.isRMReceived=1 AND DATE(bb.RM_Received_Date)=@day AND zz.ZoneCode=@zonecode " +
                         " AND zz.Region=@region )m GROUP BY m.ItemDescription;";
            try
            {
                con.Open();
                using (var cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@zonecode", ss.s_zonecode);
                    cmd.Parameters.AddWithValue("@region", ss.s_region);
                    var adapter = new MySqlDataAdapter();

                    cmd.Connection = con;
                    adapter.SelectCommand = cmd;
                    using (ReportDataset ds = new ReportDataset())
                    {
                        adapter.Fill(data, "DataTable3");
                    }

                    return data;
                }
            }
            catch (Exception e)
            {
                log.Error("Error in fetching data!" + e.ToString());
                throw;
            }
        }

        public ActionResult DownloadDaily(string dailyDate, string filename)
        {
            var ss = (ORSession)Session["UserSession"];
            var date = DateTime.Now.ToString("MM/dd/yyyy");
            string name = ss.s_fullname.ToUpper().ToString();
            string title = "Daily Inventory Report";
            string dateDaily = Session["dailyDate"].ToString();
            var db = new ORtoMySql();
            var con = db.getConnection();

            try
            {
                ReportDataset dt = (ReportDataset)Session["dailyData"];
                ReportDocument rpt = new ReportDocument();
                var DailyInventoryReport = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\inventoryReport.rpt");
                rpt.Load(DailyInventoryReport);
                rpt.SetDataSource(dt);
                rpt.SetParameterValue("date", date);
                rpt.SetParameterValue("generatedBy", name);
                rpt.SetParameterValue("reportType", title);
                System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                return File(stream, "application/pdf", "Daily-Inventory-Report" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
            }
            catch (Exception e)
            {
                log.Error("Error in saving monthly daily report." + e.ToString());
                ViewBag.Error = e.ToString();
                return View("Error");
            }
        }

        public JsonResult exportDaily(DateTime dailyDate)
        {
            Session.Add("dailyDate", dailyDate);
            ReportDataset dt = getDayData(Convert.ToDateTime(dailyDate));
            if (dt.DataTable3.Count == 0)
            {
                return Json(new
                {
                    status = false,
                    msg = "Sorry, No records found."
                });
            }
            Session["dailyData"] = dt;
            try
            {
                return Json(new
                {
                    respcode = 1,
                    status = true,
                    msg = "Successfully and saved daily inventory report!",
                    respmessage = "DailyInventory"
                });
            }
            catch (Exception e)
            {
                log.Error("Error in saving monthly daily report." + e.ToString());
                return Json(new
                {
                    status = false,
                    msg = "Error, Unable to process requests."
                });
            }
        }
    }
}