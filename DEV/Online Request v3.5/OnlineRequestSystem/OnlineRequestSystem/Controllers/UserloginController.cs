using AESEncrypt;
using log4net.Config;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class UserloginController : Controller
    {
        private class LoginCred { public string username { get; set; } public string password { get; set; } }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserloginController));
        private Helper que = new Helper();
        private int isLogin = 1;

        [Route("mlhuillier-philippines-online-request")]
        public ActionResult Login()
        {
            if (Request.Browser.Browser != "Chrome")
            {
                ViewBag.Note = "<h4><label>Please use '<b>Google Chrome</b>' as your browser for best performance. </br> Thank you :) </label></h4>";
            }
            return View();
        }

        public ActionResult Authenticate(string username, string password)
        {
            var con = new ORtoMySql();
            var toTC = new CultureInfo("en-US", false).TextInfo;
            XmlConfigurator.Configure();
            var model = new ORSession();
            var encrypt = new AESEncryption();
            try
            {
                var cred = new LoginCred();
                cred.username = username;
                cred.password = password;

                byte[] data = UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cred));
                string w = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                var source = new System.Uri(w + "/Login");
                var reqHandler = new RequestHandler("POST", source, "application/json", data);
                string response = reqHandler.HttpPostRequest();

                string ee = response.Substring(0, 5);

                if (ee == "Error")
                    return Json(new { status = false, msg = "<b>Service not responding.</b>" }, JsonRequestBehavior.AllowGet);

                var Response = JsonConvert.DeserializeObject<LoginResponse>(response);

                if (Response.resCode == "1")
                    return Json(new { status = false, msg = Response.resMsg }, JsonRequestBehavior.AllowGet);


                model = Response.logdata;
                model.s_DivManager = toTC.ToTitleCase(Response.logdata.s_DivManager.ToLower());
                model.s_DepartManager = toTC.ToTitleCase(que.GetDepartmentManager(model.s_costcenter).ToLower());
                model.s_BMName = toTC.ToTitleCase(Response.logdata.s_BMName.ToLower());
                model.s_AMName = toTC.ToTitleCase(Response.logdata.s_AMName.ToLower());
                model.s_LPTLName = toTC.ToTitleCase(Response.logdata.s_LPTLName.ToLower());
                model.s_RMName = toTC.ToTitleCase(Response.logdata.s_RMName.ToLower());
                model.s_login = isLogin;

                object[] DivApproverInfo = isDivisionApprover(model.s_usr_id, model.s_zonecode);
                object[] DeptApproverInfo = isDepartmentApprover(model.s_usr_id, model.s_zonecode);

                string NaviGation = "";
                if (DivApproverInfo != null)
                {
                    model.s_DivisionID = DivApproverInfo[0].ToString();
                    model.s_DivApprover_ResID = DivApproverInfo[1].ToString();
                    model.s_isDivisionApprover = Convert.ToInt32(DivApproverInfo[2]);
                    if (model.s_usr_id != "LHUI1011873")
                    {
                        NaviGation = (model.s_task != "GMO-GENMAN") ? "Div" : "GMO";
                    }
                }
                else
                {
                    model.s_isDivisionApprover = 0;
                }
                if (DeptApproverInfo != null)
                {
                    model.s_DepartmentCode = DeptApproverInfo[0].ToString();
                    model.s_Department_ResID = DeptApproverInfo[1].ToString();
                    model.s_isDepartmentApprover = Convert.ToInt32(DeptApproverInfo[2]);
                }
                else
                {
                    model.s_isDepartmentApprover = 0;
                }

                model.s_isVPAssistant = isVPAssistant(model.s_usr_id);

                Session["userid"] = model.s_usr_id;
                Session["encryptedPassword"] = encrypt.AESEncrypt(password, "kWuYDGElyQDpGKM9");
                Session["isVPAssistant"] = model.s_isVPAssistant;

                // Check for user if branch or division
                model.isDivRequest = (!string.IsNullOrEmpty(model.s_DivCode)) ? 1 : 0;
                Session["DivName"] = model.s_Division;

                // Check if user is SDC Approver / RM
                if (model.s_job_title == "REGIONAL MAN")
                {
                    model.s_SDCApprover = 1;
                    Session["OpenRequest"] = "SDC";
                }
                // Check if user is MMD employee
                model.s_MMD = (model.s_costcenter == "0002MMD" || model.s_costcenter == "0001MMD") ? 1 : 0;

                GraphicalReportFilter(model);
                model.s_LayoutControl = LayoutControl(model);
                Session["UserSession"] = model;
                Session["OpenRequest"] = NaviGation;

                return Json(new { status = true, urlAction = Url.Action("ViewOpenRequest", "OpenRequest") }, JsonRequestBehavior.AllowGet);
            }
            catch (MySqlException x)
            {
                log.Fatal(x.Message, x);
                return Json(new { status = false, msg = "<b>Error establishing a database connection.</b>" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new { status = false, msg = "<b>Sorry, unable to process your request at this time.</b>" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void GraphicalReportFilter(ORSession model)
        {
            int x = 0;
            if (model.s_usr_id == "LHUI1011873")
            {
                Session["Role"] = 5;
                x = 1;
            }
            else if (model.s_task == "GMO-GENMAN")
            {
                Session["Role"] = 4;
                x = 1;
            }
            else if (model.s_MMD == 1)
            {
                Session["Role"] = 3;
                x = 1;
            }
            else if (model.s_isDivisionApprover == 1 || model.s_isDepartmentApprover == 1)
            {
                Session["Role"] = 2;
                x = 1;
            }
            else if (model.s_SDCApprover == 1)
            {
                Session["Role"] = 1;
                x = 1;
            }
            if (x == 1)
            {
                Session["Allowed"] = 1;
                string ReportURL = ConfigurationManager.AppSettings["ReportUrl"].ToString();
                Session["ReportUrl"] = ReportURL + "user=" + Session["userid"] + "&pass=" + Session["encryptedPassword"] + "&role=" + Session["Role"];
                object e = Session["ReportUrl"];
            }
        }

        private object[] isDivisionApprover(string usrid, string ZoneCode)
        {
            try
            {
                string zcode = que.ZoneCodeSelector(ZoneCode);
                object[] g = new object[] { "", "", "" };
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = new MySqlCommand("SELECT DivId, ApproverResID FROM division WHERE ApproverResID = @usrid AND ZoneCode = @zonecode;", conn);
                    cmd.Parameters.AddWithValue("@usrid", usrid);
                    cmd.Parameters.AddWithValue("@zonecode", zcode);
                    conn.Open();
                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (read.HasRows)
                        {
                            read.Read();
                            g[0] = read["DivId"].ToString().Trim();
                            g[1] = read["ApproverResId"].ToString().Trim();
                            g[2] = 1;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return g;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        private object[] isDepartmentApprover(string usrid, string ZoneCode)
        {
            string zcode = que.ZoneCodeSelector(ZoneCode);
            object[] f = new object[] { "", "", "" };
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = new MySqlCommand("SELECT deptCode, approver_resID FROM department WHERE approver_resID = @usrid AND ZoneCode = @zonecode;", conn);
                cmd.Parameters.AddWithValue("@usrid", usrid);
                cmd.Parameters.AddWithValue("@zonecode", zcode);
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (read.HasRows)
                    {
                        read.Read();
                        f[0] = read["deptCode"].ToString().Trim();
                        f[1] = read["approver_resID"].ToString().Trim();
                        f[2] = 1;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return f;
        }

        private int isVPAssistant(string userID)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.CommandText = "SELECT * FROM VPassistant WHERE approver_resid = @userID ";
                cmd.CommandTimeout = 0;
                conn.Open();
                using (MySqlDataReader read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (read.HasRows)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        public ActionResult checkpoint()
        {
            var model = (ORSession)Session["UserSession"];
            if (User.Identity.IsAuthenticated)
            {
                if (model != null)
                {
                    if (model.s_login == 1)
                    {
                        if (Request.UrlReferrer == null)
                        {
                            return RedirectToAction("Logout");
                        }
                        else
                        {
                            return Redirect(Request.UrlReferrer.PathAndQuery);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Logout");
                }
            }
            return RedirectToAction("Logout");
        }

        public string LayoutControl(ORSession model)
        {
            string Layout = "~/Views/Shared/_Layout.cshtml";

            if (model != null)
            {
                if (model.s_costcenter == "0002MMD" || model.s_costcenter == "0001MMD")
                {
                    return Layout = "~/Views/Shared/_MMD.cshtml";
                }
                if (model.s_job_title == "REGIONAL MAN")
                {
                    return Layout = "~/Views/Shared/_RM.cshtml";
                }
            }
            else
            {
                Layout = "~/Views/Shared/_Layout.cshtml";
            }

            return Layout;
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session.Abandon();
            Session.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Login");
        }
    }

}