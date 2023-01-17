using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

using AESEncrypt;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace OnlineRequestSystem.Controllers
{
    public class RequestController : Controller
    {
        #region Inheritance
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestController));
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        private string format = "yyyy/MM/dd HH:mm:ss";
        private FileController file = new FileController();
        private RequestQueries que = new RequestQueries();
        private AutoApproving A_approve = new AutoApproving();
        #endregion

        [Route("create-request")]
        public ActionResult RequestFormView()
        {
            var Model = new CreateReqModels();
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) return RedirectToAction("Logout", "Userlogin");

            var regcode = ss.s_regionCode;
            string newRegcode = ""; var req = "";
            Model.ListReqtype = que.ListOfReqType(ss.s_zonecode).ToList();
            if (regcode.Length > 3)
            {
                newRegcode = regcode.Substring(0, 3);
            }
            else
            {
                newRegcode = regcode;
            }
            var NewCode = ((new[] { "001", "002" }).Contains(ss.s_comp)) ? ss.s_DivAcro : ss.s_comp;

            if (GetORNum() == false)
                Session["reqNo"] = "00001";

            req = Session["reqNo"].ToString();
            ViewBag.RequestNo = "OR-" + newRegcode + "-" + NewCode + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Year.ToString().Remove(0, 2) + "-" + req;
            return View(Model);
        }


        public ActionResult CreateRequest(CreateReqModels Data, string base64Str)
        {
            int forPresident = 0;
            string Author = Data.Author;
            var db = new ORtoMySql();
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null)
                return RedirectToAction("Logout", "Userlogin");


            if (mySession.s_isDivisionApprover == 1 && mySession.s_job_title != "GMO-GENMAN" && mySession.s_usr_id != "LHUI1011873")
                forPresident = 1;


            if (mySession.isDivRequest == 1 && Check_OnlineRequest_Department(mySession.s_costcenter) == false)
            {
                return Json(new
                {
                    resCode = "001",
                    status = false,
                    msg = "Your Department Approver has not been added yet.. <br/> Please contact <b>MMD - " + mySession.s_zonecode + "</b> to add your Department Approver."
                }, JsonRequestBehavior.AllowGet);
            }
            if (mySession.isDivRequest == 1 && Check_OnlineRequest_Division(mySession.s_costcenter) == false)
            {
                return Json(new
                {
                    resCode = "001",
                    status = false,
                    msg = "Your Division Approver has not been added yet.. <br/> Please contact <b>MMD - " + mySession.s_zonecode + "</b> to add your Division Approver."
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                using (MySqlConnection conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.CommandTimeout = 0;

                    foreach (var item in Data.ReqItems)
                    {
                        cmd.CommandText = " INSERT INTO OnlineRequest.requestItems (reqNumber, TypeID, itemDescription, qty, syscreated, syscreator, unit) " +
                                          " VALUES (@reqNumber, @TypeID, @itemDescription, @qty, @syscreated, @syscreator, @unit) ";
                        cmd.Parameters.AddWithValue("@reqNumber", Data.RequestNo.ToString().Trim());
                        cmd.Parameters.AddWithValue("@TypeID", Data.RequestType.ToString().Trim());
                        cmd.Parameters.AddWithValue("@itemDescription", item.ItemDescription.ToString().Trim());
                        cmd.Parameters.AddWithValue("@qty", item.ItemQty.ToString().Trim());
                        cmd.Parameters.AddWithValue("@syscreated", Data.ReqDate);
                        cmd.Parameters.AddWithValue("@syscreator", mySession.s_usr_id);
                        cmd.Parameters.AddWithValue("@unit", item.ItemUnit.ToString().Trim());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    cmd.CommandText = " INSERT INTO OnlineRequest.onlineRequest_Open " +
                                      " (reqNumber, reqDate, reqCreator, TypeID, reqDescription, reqTotal, BranchCode, Region, Area, AreaCode, DivCode, Zonecode, reqStatus, isApproved, isDivRequest, TotalQty, DeptCode, syscreated, syscreator, sysmodified, sysmodifier, forPresident) " +
                                      " VALUES " +
                                      " (@reqNumber, @reqDate, @reqCreator, @TypeID, @reqDescription, @reqTotal, @BranchCode, @Region, @Area, @AreaCode, @DivCode, @ZoneCode, @reqStatus, @isApproved, @isDivRequest, @TotalQty, @deptCode, @syscreated, @syscreator, @sysmodified, @sysmodifier, @forPresident)";

                    cmd.Parameters.AddWithValue("@reqNumber", Data.RequestNo);
                    cmd.Parameters.AddWithValue("@reqDate", Data.ReqDate);
                    cmd.Parameters.AddWithValue("@reqCreator", mySession.s_fullname);
                    cmd.Parameters.AddWithValue("@TypeID", Data.RequestType);
                    cmd.Parameters.AddWithValue("@reqDescription", Data.Description);
                    cmd.Parameters.AddWithValue("@reqTotal", Data.OverallTotalCost);
                    cmd.Parameters.AddWithValue("@BranchCode", Data.BranchCode);
                    cmd.Parameters.AddWithValue("@Region", Data.Region);
                    if (Data.DivisionCode != null) { cmd.Parameters.AddWithValue("@DivCode", Data.DivisionCode); } else { cmd.Parameters.AddWithValue("@DivCode", Data.DivisionCode); }
                    cmd.Parameters.AddWithValue("@Area", mySession.s_area);
                    cmd.Parameters.AddWithValue("@AreaCode", mySession.s_areaCode);
                    cmd.Parameters.AddWithValue("@ZoneCode", Data.ZoneCode);
                    cmd.Parameters.AddWithValue("@reqStatus", "OPEN");
                    cmd.Parameters.AddWithValue("@isApproved", 0);
                    cmd.Parameters.AddWithValue("@isDivRequest", mySession.isDivRequest);
                    cmd.Parameters.AddWithValue("@forPresident", forPresident);
                    cmd.Parameters.AddWithValue("@deptCode", mySession.s_costcenter);
                    cmd.Parameters.AddWithValue("@TotalQty", Data.TotalQty);
                    cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@syscreator", mySession.s_usr_id);
                    cmd.Parameters.AddWithValue("@sysmodified", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@sysmodifier", mySession.s_usr_id);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    if (mySession.isDivRequest == 1)
                    {
                        if (que.UpdateRecommendedApproval(conn, cmd, db, tran, Data.RequestType) == "error")
                        {
                            return Json(new { status = false, msg = "Unable to process Request." }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    var List = getRecommendedApproval(Data.RequestType);
                    try
                    {
                        foreach (var item in List)
                        {
                            if (mySession.s_job_title == "AREA MANAGER")
                            {
                                A_approve.AreaManager(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else if (mySession.s_job_title == "REGIONAL MAN")
                            {
                                A_approve.RegionalManager(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else if (mySession.s_isDepartmentApprover == 1)
                            {
                                A_approve.DepartmentManager(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else if (mySession.s_isDivisionApprover == 1 && forPresident == 1 && mySession.s_usr_id != "LHUI1011873")
                            {
                                A_approve.DivisionManager(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else if (mySession.s_job_title == "GMO-GENMAN")
                            {
                                A_approve.GMOGenman(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else if (mySession.s_usr_id == "LHUI1011873")
                            {
                                A_approve.President(conn, cmd, db, tran, Data, List, mySession);
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO OnlineRequest.onlineRequest_Escalation (reqNumber) VALUES (@reqNumber)";
                                cmd.Parameters.AddWithValue("@reqNumber", Data.RequestNo);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                InsertApproverStatus(conn, cmd, db, tran, Data);
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        tran.Rollback();
                        log.Fatal(x.Message, x);
                        return Json(new
                        {
                            status = false,
                            msg = "Error in auto approving of request."
                        }, JsonRequestBehavior.AllowGet);
                    }


                    if (!string.IsNullOrEmpty(base64Str))
                    {
                        if (file.FileUpload(conn, cmd, db, tran, Data, base64Str, mySession) == "success")
                        {
                            log.Info("Sucessfully inserted Diagnostic file || request no:" + Data.RequestNo + " || requested by: " + mySession.s_usr_id + " || with request type ID: " + Data.RequestType);
                        }
                    }

                    insertNewORNum(conn, cmd, db, tran);

                    tran.Commit();
                    log.Info("Request has been created || request no:" + Data.RequestNo + " || requested by: " + mySession.s_usr_id + " || with request type ID: " + Data.RequestType);
                    return Json(new { status = true, msg = "Request has been created." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = "Unable to process Request."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool Check_OnlineRequest_Division(string CostCenter)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT costcenter FROM division WHERE CostCenter = @CostCenter";
                cmd.Parameters.AddWithValue("@CostCenter", CostCenter);
                cmd.CommandTimeout = 0;
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
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

        private bool Check_OnlineRequest_Department(string CostCenter)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT costcenter FROM department WHERE CostCenter = @CostCenter";
                cmd.Parameters.AddWithValue("@CostCenter", CostCenter);
                cmd.CommandTimeout = 0;
                conn.Open();
                using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
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

        private List<RequestTypeMenu> getRecommendedApproval(string type)
        {
            try
            {
                var db = new ORtoMySql();
                var menu = new List<RequestTypeMenu>();

                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = " SELECT isAMApproval, isRMApproval, isGMApproval, " +
                                      " isDivManApproval, DivCode1, isDivManApproval2, DivCode2, isDivManApproval3, DivCode3, isPresidentApproval " +
                                      " FROM OnlineRequest.requestType WHERE TypeID = @TypeID";
                    cmd.Parameters.AddWithValue("@TypeID", type);
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var m = new RequestTypeMenu();
                            m.isAMApproval = Convert.ToInt32(rdr["isAMApproval"].ToString().Trim());
                            m.isRMApproval = Convert.ToInt32(rdr["isRMApproval"].ToString().Trim());
                            m.isGMApproval = Convert.ToInt32(rdr["isGMApproval"].ToString().Trim());
                            m.isDivManApproval = Convert.ToInt32(rdr["isDivManApproval"].ToString().Trim());
                            m.Approval_DivCode1 = rdr["DivCode1"].ToString().Trim();
                            m.isDivManApproval2 = Convert.ToInt32(rdr["isDivManApproval2"].ToString().Trim());
                            m.Approval_DivCode2 = rdr["DivCode2"].ToString().Trim();
                            m.isDivManApproval3 = Convert.ToInt32(rdr["isDivManApproval3"].ToString().Trim());
                            m.Approval_DivCode3 = rdr["DivCode3"].ToString().Trim();
                            m.isPresidentApproval = Convert.ToInt32(rdr["isPresidentApproval"].ToString().Trim());

                            menu.Add(m);
                        }
                    }
                    return menu;
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }

        private ActionResult InsertApproverStatus(MySqlConnection con, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran, CreateReqModels Data)
        {
            try
            {
                con = db.getConnection();
                cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO OnlineRequest.requestApproverStatus (reqNumber) VALUES (@reqNumber)";
                cmd.Parameters.AddWithValue("@reqNumber", Data.RequestNo);
                cmd.CommandTimeout = 0;
                cmd.ExecuteNonQuery();
            }
            catch (Exception x)
            {
                tran.Rollback();
                log.Fatal(x.Message, x);
                return Json(new { status = false, msg = "Unable to process Request." }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public bool EscalationChecking(string ReqNo)
        {
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Escalation WHERE reqNumber = @ReqNo";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    using (var read = cmd.ExecuteReader(CommandBehavior.CloseConnection))
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

        private bool GetORNum()
        {
            var db = new ORtoMySql();
            var ss = (ORSession)Session["UserSession"];

            bool ret = false;

            string year, month, setName, orNum;

            year = DateTime.Now.Year.ToString().Remove(0, 2);
            month = DateTime.Now.Month.ToString().PadLeft(2, '0');
            var regcode = ss.s_regionCode;
            setName = "";

            if (regcode.Length > 3)
            {
                setName = regcode.Substring(0, 3);
            }
            else
            {
                setName = regcode;
            }

            string sql = "select ornum from settings where settingyear=@year and settingname=@setName and settingmonth=@month order by ornum desc limit 1;";
            using (var conn = db.getConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@setName", setName);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.CommandTimeout = 0;
                    conn.Open();
                    using (var read = cmd.ExecuteReader())
                    {
                        while (read.Read())
                        {
                            orNum = read["ornum"].ToString().Trim();
                            var orn = Convert.ToInt32(orNum) + 1;
                            string req = orn.ToString();
                            Session["reqNo"] = req.PadLeft(5, '0');
                            ret = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    log.Fatal(e.Message, e);
                    throw;
                }
                return ret;
            }
        }

        public bool insertNewORNum(MySqlConnection conn, MySqlCommand cmd, ORtoMySql db, MySqlTransaction tran)
        {
            try
            {
                var mySession = (ORSession)Session["UserSession"];
                string year, month, setName;
                year = DateTime.Now.Year.ToString().Remove(0, 2);
                month = DateTime.Now.Month.ToString().PadLeft(2, '0');
                var regcode = mySession.s_regionCode;
                setName = "";

                if (regcode.Length > 3)
                {
                    setName = regcode.Substring(0, 3);
                }
                else
                {
                    setName = regcode;
                }

                string ornumber = Session["reqNo"].ToString();

                conn = db.getConnection();
                cmd = conn.CreateCommand();
                cmd.CommandTimeout = 0;
                cmd.CommandText = "INSERT INTO settings (settingyear,settingname,settingmonth,ornum) VALUES (@year,@setName,@month,@ornumber)";
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@setName", setName);
                cmd.Parameters.AddWithValue("@ornumber", ornumber);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                tran.Rollback();
                throw;
            }
        }

        [Route("cash-request")]
        public ActionResult CashRequest()
        {

            var encrypt = new AESEncryption();
            ORSession mySession = (ORSession)Session["UserSession"];

            try
            {
                log.Info("mySession:" + mySession);

                if (mySession == null)
                {
                    return RedirectToAction("Logout", "Userlogin");
                }
                else
                {

                    string serviceUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                    var source = new System.Uri(serviceUrl + "/GetCashRequestApprovers/?zonecode=" + mySession.s_zonecode + "&class_04=" + mySession.s_area + "&region=" + mySession.s_region + "&areacode=" + mySession.s_areaCode + "&task=" + mySession.s_task);
                    var requestHandler = new RequestHandler(source, "GET", "application/json");

                    string x = requestHandler.HttpGetRequest();

                    dynamic response = JObject.Parse(x);

                    if (response.resCode == 0)
                    {
                        string[] userCred = new string[] {
                            mySession.s_fullname, mySession.s_usr_id , mySession.s_res_id , mySession.s_job_title, mySession.s_task, mySession.s_zonecode, mySession.s_bedrnm,mySession.s_area, mySession.s_region
                        };

                        var encryptedUser = encrypt.AESEncrypt(userCred.ToString(), "kWuYDGElyQDpGKM9");


                        return CashRequestRedirect(response.ListOfCashRequestApprovers[0], encryptedUser, mySession);

                    }
                    else
                    {
                        ViewBag.Error = response.resMsg.ToString();
                        return View("Error");
                    }

                    //return Json(response.ListOfCashRequestApprovers[0].AmName.ToString(), JsonRequestBehavior.AllowGet);

                }
            }

            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
        }

        private ActionResult CashRequestRedirect(dynamic approvers, string encryptedUser, ORSession mySession)
        {
            return Redirect("http://127.0.0.1:8080/#/home?fullname=" +
               mySession.s_fullname.ToString() + "&" + "userId=" + mySession.s_usr_id.ToString() + "&" + "resId=" + mySession.s_res_id.ToString() +
                "&" + "jobTitle=" + mySession.s_job_title.ToString() +
                "&" + "task=" + mySession.s_task.ToString() + "&" + "zonecode=" + mySession.s_zonecode.ToString() +
               "&" + "bedrnm=" + mySession.s_bedrnm.ToString() + "&" + "area=" + mySession.s_area.ToString() +
                "&" + "region=" + mySession.s_region.ToString() + "&" + "areaCode=" + mySession.s_areaCode + "&" + "amName=" + approvers.AmName.ToString() +
                "&" + "rmName=" + approvers.RmName.ToString() + "&" + "ramName=" + approvers.RamName.ToString() + "&" + "asstName=" + approvers.GmoGenAsstName.ToString() +
                "&" + "vpoName=" + approvers.GmoGenName.ToString() + "&" + "token=" + encryptedUser

           );

            //return Redirect("http://10.4.8.168:3000/#/home?fullname=" +
            //    mySession.s_fullname.ToString() + "&" + "userId=" + mySession.s_usr_id.ToString() + "&" + "resId=" + mySession.s_res_id.ToString() +
            //     "&" + "jobTitle=" + mySession.s_job_title.ToString() +
            //     "&" + "task=" + mySession.s_task.ToString() + "&" + "zonecode=" + mySession.s_zonecode.ToString() +
            //    "&" + "bedrnm=" + mySession.s_bedrnm.ToString() + "&" + "area=" + mySession.s_area.ToString() +
            //     "&" + "region=" + mySession.s_region.ToString() + "&" + "areaCode=" + mySession.s_areaCode + "&" + "amName=" + approvers.AmName.ToString() +
            //     "&" + "rmName=" + approvers.RmName.ToString() + "&" + "ramName=" + approvers.RamName.ToString() + "&" + "asstName=" + approvers.GmoGenAsstName.ToString() +
            //     "&" + "vpoName=" + approvers.GmoGenName.ToString() + "&" + "token=" + encryptedUser

            //);
        }


    }
}