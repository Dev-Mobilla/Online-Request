using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineRequestSystem.Areas.Administrator.Models;

namespace OnlineRequestSystem.Areas.Administrator.Controllers
{
    public class OpenRequestController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(OpenRequestController));
        // GET: Administrator/OpenRequest
        public ActionResult OpenRequest()
        {
            if (Convert.ToInt32(Session["Utility"]) == 1)
            {
                return View();
            }
            else
            {
                return View("_Unauthorized");
            }

        }
        public JsonResult SearchReqNumber(string reqno, string zone)
        {
            try
            {
                if (reqno == string.Empty || zone == string.Empty)
                    return Json(new { status = true, rescode = "0", msg = "No data found", }, JsonRequestBehavior.AllowGet);

                ORtoMySql db = new ORtoMySql();
                List<RequestModels> list = new List<RequestModels>();
                using (MySqlConnection conn = db.getConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = conn.CreateCommand())
                    {
                        string Query = "SELECT * FROM onlinerequest_open WHERE reqnumber = @reqno and ZoneCode = @zone";
                        cmd.Parameters.AddWithValue("@reqno", reqno);
                        cmd.Parameters.AddWithValue("@zone", zone);
                        cmd.CommandText = Query;
                        using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (read.HasRows)
                            {
                                while (read.Read())
                                {
                                    RequestModels x = new RequestModels();
                                    x.id = Convert.ToInt32(read["id"].ToString().Trim());
                                    x.reqNumber = read["reqNumber"].ToString().Trim();
                                    x.reqDate = read["reqDate"].ToString().Trim();
                                    x.reqCreator = read["reqCreator"].ToString().Trim();
                                    x.TypeID = read["TypeID"].ToString().Trim();
                                    x.BranchCode = read["BranchCode"].ToString().Trim();
                                    x.Region = read["Region"].ToString().Trim();
                                    x.Area = read["Area"].ToString().Trim();
                                    x.AreaCode = read["AreaCode"].ToString().Trim();
                                    x.DivCode = read["DivCode"].ToString().Trim();
                                    x.ZoneCode = read["ZoneCode"].ToString().Trim();
                                    x.Status = read["reqStatus"].ToString().Trim();
                                    x.SysCreated = read["syscreated"].ToString().Trim();
                                    x.SysCreator = read["syscreator"].ToString().Trim();
                                    x.ForPresident = read["forPresident"].ToString().Trim();
                                    list.Add(x);
                                }

                                return Json(new
                                {
                                    status = true,
                                    data = list,
                                    rescode = "200",
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    status = false,
                                    rescode = "400",
                                    msg = "No data found",
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    rescode = "500",
                    msg = "Error",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult OpenExecuteQue(string query)
        {
            try
            {
                ORtoMySql db = new ORtoMySql();
                object divecodeResult = "";
                using (MySqlConnection con = db.getConnection())
                {
                    con.Open();
                    MySqlCommand cmd = con.CreateCommand();
                    cmd.CommandText = query;
                    var result = cmd.ExecuteNonQuery();
                }
                return Json(new
                {
                    status = true,
                    rescode = "200",
                    msg = "Success",
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    rescode = "500",
                    msg = "Error",
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}