using CrystalDecisions.CrystalReports.Engine;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Dataset;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class ReportsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReportsController));

        public ActionResult ReportsMenu(string type)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                var r = new ReportViewModel();

                if (type == "daily")
                {
                    r.BeginFormMethod = "GenerateDaily";
                }
                else
                {
                    r.BeginFormMethod = "GenerateMonthly";
                }
                r.ListBranches = ListOfBranches(ss.s_zonecode.Trim());
                r.ListDivision = ListOfDivision(ss.s_zonecode.Trim());
                r.ListArea = ListOfArea(ss.s_zonecode.Trim());
                r.ListRegions = ListOfRegions(ss.s_zonecode.Trim());
                r.reportType = type;
                if (TempData["Err"] != null)
                {
                    r.NoDataFound = "NoData";
                }
                return View(r);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        public ActionResult GenerateDaily(ReportViewModel rep, DateTime? dailyFrom, DateTime? dailyTo, string Category, string Selected, string type)
        {
            try
            {
                if (!DataCheck(rep, dailyFrom, dailyTo, Category, Selected))
                {
                    TempData["Err"] = "No Data Found";
                    return RedirectToAction("ReportsMenu", "Reports", new { type });
                }
                var data = getDailyData(rep, dailyFrom, dailyTo, Category, Selected);
                var rpt = new ReportDocument();

                string DateFrom = dailyFrom.HasValue ? dailyFrom.Value.ToString("yyyy-MM-dd") : string.Empty;
                string DateTo = dailyTo.HasValue ? dailyTo.Value.ToString("yyyy-MM-dd") : string.Empty;

                var Daily = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\dailyReport.rpt");
                rpt.Load(Daily);
                rpt.SetDataSource(data);
                if (Selected == "Branch")
                {
                    rpt.SetParameterValue("GroupCategory", rep.branch);
                }
                else if (Selected == "Division")
                {
                    string divisionName = getDivisionName(rep.division);
                    rpt.SetParameterValue("GroupCategory", divisionName);
                }
                else if (Selected == "Area")
                {
                    rpt.SetParameterValue("GroupCategory", rep.area);
                }
                else if (Selected == "Region")
                {
                    rpt.SetParameterValue("GroupCategory", rep.region);
                }

                rpt.SetParameterValue("Selected", Selected);
                rpt.SetParameterValue("Date", DateTime.Now.ToString("MM/dd/yyyy"));
                rpt.SetParameterValue("dateFrom", dailyFrom);
                rpt.SetParameterValue("dateTo", dailyTo);
                System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                return File(stream, "application/pdf", "DAILY REPORT- ONLINE REQUEST" + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                TempData["Err"] = "Unable to Process Request.";
                return RedirectToAction("ReportsMenu", "Reports", new { type });
            }
        }

        public ActionResult GenerateMonthly(ReportViewModel rep, DateTime monthlyDate, string Category, string Selected, string type)
        {
            try
            {
                if (!DataCheckMonthly(rep, monthlyDate, Category, Selected))
                {
                    TempData["Err"] = "No Data Found";
                    return RedirectToAction("ReportsMenu", "Reports", new { type });
                }
                var data = getMonthlyData(rep, monthlyDate, Category, Selected);
                var rpt = new ReportDocument();

                var Monthly = (System.Web.HttpContext.Current.Server.MapPath("../") + "Report\\monthlyReport.rpt");
                rpt.Load(Monthly);
                rpt.SetDataSource(data);

                if (Selected == "Branch")
                {
                    rpt.SetParameterValue("GroupCategory", rep.branch);
                }
                else if (Selected == "Division")
                {
                    string divisionName = getDivisionName(rep.division);
                    rpt.SetParameterValue("GroupCategory", divisionName);
                }
                else if (Selected == "Area")
                {
                    rpt.SetParameterValue("GroupCategory", rep.area);
                }
                else if (Selected == "Region")
                {
                    rpt.SetParameterValue("GroupCategory", rep.region);
                }
                rpt.SetParameterValue("Selected", Selected);
                rpt.SetParameterValue("Date", DateTime.Now.ToString("MM/dd/yyyy"));
                rpt.SetParameterValue("selectedMonth", monthlyDate.ToString("MMMM", CultureInfo.InvariantCulture));
                System.IO.Stream stream = rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

                return File(stream, "application/pdf", "MONTHLY REPORT- ONLINE REQUEST " + DateTime.Now.ToString("yyyy/MM/dd") + ".pdf");
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                TempData["Err"] = "Unable to Process Request.";
                return RedirectToAction("ReportsMenu", "Reports", new { type });
            }
        }

        private ReportDataset getMonthlyData(ReportViewModel Obj, DateTime monthlyDate, string Category, string Selected)
        {
            try
            {
                string monthName = monthlyDate.ToString("MMMM", CultureInfo.InvariantCulture);
                string month = monthlyDate.ToString("MM");
                string mYear = monthlyDate.ToString("yyyy");
                string Code = "";
                if (Obj.branch != null)
                {
                    Code = getBranchCode(Obj.branch);
                }
                var db = new ORtoMySql();
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    string query = "";
                    if (Category == "Close")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "CLOSED");
                    }
                    else if (Category == "Open")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@isMMDProcessed", 0);
                    }
                    else if (Category == "Approved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@isPO_Approved", 1);
                        cmd.Parameters.AddWithValue("@isPO_Approved2", 2);
                    }
                    else if (Category == "Disapproved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "DISAPPROVED");
                    }

                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@mYear", mYear);
                    cmd.CommandText = query;

                    using (var adapter = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        adapter.SelectCommand = cmd;

                        using (var ds = new ReportDataset())
                        {
                            adapter.Fill(ds, "DataTable1");
                            return ds;
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

        private ReportDataset getDailyData(ReportViewModel Obj, DateTime? dailyFrom, DateTime? dailyTo, string Category, string Selected)
        {
            try
            {
                string DateFrom = dailyFrom.HasValue ? dailyFrom.Value.ToString("yyyy-MM-dd") : string.Empty;
                string DateTo = dailyTo.HasValue ? dailyTo.Value.ToString("yyyy-MM-dd") : string.Empty;
                var db = new ORtoMySql();
                string Code = "";
                if (Obj.branch != null)
                {
                    Code = getBranchCode(Obj.branch);
                }
                using (var con = db.getConnection())
                {
                    con.Open();
                    var cmd = con.CreateCommand();
                    string query = "";

                    if (Category == "Close")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "CLOSED");
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }

                    else if (Category == "Approved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);                       
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@isPO_Approved", 1);
                        cmd.Parameters.AddWithValue("@isPO_Approved2", 2);
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }

                    else if (Category == "Disapproved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "DISAPPROVED");
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);                    
                    }

                    else if (Category == "Open")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", Obj.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", Obj.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", Obj.region);
                        }

                        cmd.Parameters.AddWithValue("@isMMDProcessed", 0);
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }

                    cmd.CommandText = query;
                    using (var adapter = new MySqlDataAdapter())
                    {
                        cmd.Connection = con;
                        adapter.SelectCommand = cmd;

                        using (var ds = new ReportDataset())
                        {
                            adapter.Fill(ds, "DataTable1");
                            return ds;
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

        public Boolean DataCheckMonthly(ReportViewModel rep, DateTime monthlyDate, string Category, string Selected)
        {
            try
            {
                string Month = monthlyDate.ToString("MM");
                string mYear = monthlyDate.ToString("yyyy");
                string Code = "";
                if (rep.branch != null)
                {
                    Code = getBranchCode(rep.branch);
                }
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    string query = "";

                    if (Category == "Close")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "CLOSED");
                    }
                    else if (Category == "Open")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@isMMDProcessed", 0);
                    }
                    else if (Category == "Approved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND MONTH(a.reqDate) = @Month AND YEAR(a.reqDate) = @mYear " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@isPO_Approved", 1);
                        cmd.Parameters.AddWithValue("@isPO_Approved2", 2);
                    }
                    else if (Category == "Disapproved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND MONTH(reqDate) = @Month AND YEAR(reqDate) = @mYear ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "DISAPPROVED");
                    }

                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@mYear", mYear);
                    cmd.CommandText = query;

                    using (var read = cmd.ExecuteReader())
                    {
                        if (read.HasRows)
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

        public Boolean DataCheck(ReportViewModel rep, DateTime? dailyFrom, DateTime? dailyTo, string Category, string Selected)
        {
            try
            {
                string DateFrom = dailyFrom.HasValue ? dailyFrom.Value.ToString("yyyy-MM-dd") : string.Empty;
                string DateTo = dailyTo.HasValue ? dailyTo.Value.ToString("yyyy-MM-dd") : string.Empty;
                string Code = "";
                if (rep.branch != null)
                {
                    Code = getBranchCode(rep.branch);
                }
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    string query = "";
                    if (Category == "Close")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND  @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "CLOSED");
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }
                    else if (Category == "Open")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND b.isMMDProcessed = @isMMDProcessed " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@isMMDProcessed", 0);
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }
                    else if (Category == "Approved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.BranchCode = @Code " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Code", Code);
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.DeptCode = @CostCenter " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.Area = @Area " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT a.* FROM OnlineRequest.onlineRequest_Open a " +
                                    "INNER JOIN requestApproverStatus b ON a.reqNumber = b.reqNumber " +
                                    "WHERE a.region = @Region " +
                                    "AND (b.isPO_Approved = @isPO_Approved OR  OR b.isPO_Approved = @isPO_Approved2 ) " +
                                    "AND a.reqDate BETWEEN @DateFrom AND @DateTo " +
                                    "ORDER BY a.reqDate";

                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@isPO_Approved", 1);
                        cmd.Parameters.AddWithValue("@isPO_Approved2", 2);
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }
                    else if (Category == "Disapproved")
                    {
                        if (Selected == "Branch")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE BranchCode = @Code AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Code", Code); 
                        }
                        else if (Selected == "Division")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE DeptCode = @CostCenter AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@CostCenter", rep.division);
                        }
                        else if (Selected == "Area")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE Area = @Area AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Area", rep.area);
                        }
                        else if (Selected == "Region")
                        {
                            query = "SELECT * FROM OnlineRequest.onlineRequest_Close WHERE region = @Region AND reqStatus = @Status AND ClosedDate BETWEEN @DateFrom AND @DateTo ORDER BY reqDate";
                            cmd.Parameters.AddWithValue("@Region", rep.region);
                        }

                        cmd.Parameters.AddWithValue("@Status", "DISAPPROVED");
                        cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        cmd.Parameters.AddWithValue("@DateTo", DateTo);
                    }

                    cmd.CommandText = query;
                    using (var read = cmd.ExecuteReader())
                    {
                        if (read.HasRows)
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

        public string getDivisionName(string costcenter)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetIRDivisionName?costcenter=" + costcenter);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return "Service Unavailable";

                var Response = JsonConvert.DeserializeObject<DivisionNameResponse>(x);
                Result = Response.DivisionName;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Result;
            }
            return Result;
        }

        private string getBranchCode(string branchName)
        {
            string Result = "";
            try
            {
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetBranchCode?branchName=" + branchName);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return "Service Unavailable";

                var Response = JsonConvert.DeserializeObject<BranchCodeResponse>(x);
                Result = Response.BranchCode;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Result;
            }
            return Result;
        }

        private List<SelectListItem> ListOfBranches(string zonecode)
        {
            try
            {
                var xlist = new List<SelectListItem>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetListOfBranches?zonecode=" + zonecode.Trim());
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var Response = JsonConvert.DeserializeObject<ListOfBranchesResponse>(x);
                foreach (var item in Response.ListOfBranches)
                {
                    xlist.Add(new SelectListItem { Text = item.BranchName, Value = item.BranchNameValue });
                }
                return xlist;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        private List<SelectListItem> ListOfDivision(string zonecode)
        {
            try
            {
                var xlist = new List<SelectListItem>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetIRDivisions?zonecode=" + zonecode);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var Response = JsonConvert.DeserializeObject<ListOfIRDivisionResponse>(x);
                foreach (var item in Response.ListIRDivisions)
                {
                    xlist.Add(new SelectListItem { Text = item.DivisionName, Value = item.CostCenter });
                }
                return xlist;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        private List<SelectListItem> ListOfArea(string zonecode)
        {
            try
            {
                var xlist = new List<SelectListItem>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetListOfAreas?zonecode=" + zonecode);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var Response = JsonConvert.DeserializeObject<ListOfAreaResponse>(x);
                foreach (var item in Response.ListOfAreas)
                {
                    xlist.Add(new SelectListItem { Text = item.Area, Value = item.AreaValue });
                }
                return xlist;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        private List<SelectListItem> ListOfRegions(string zonecode)
        {
            try
            {
                var xlist = new List<SelectListItem>();
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/GetListOfRegions?zonecode=" + zonecode);
                var reqHandler = new RequestHandler(source, "GET", "application/json");
                string x = reqHandler.HttpGetRequest();
                if (x == "Error")
                    return null;

                var Response = JsonConvert.DeserializeObject<ListOfRegionResponse>(x);
                foreach (var item in Response.ListOfRegions)
                {
                    xlist.Add(new SelectListItem { Text = item.Region, Value = item.RegionValue });
                }
                return xlist;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }
    }
}
