using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class RequestItemsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RequestItemsController));

        public ActionResult insertDivItemStatus(string ReqNo, string description, int quantity, int isCheckedDiv)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            string desc = description.Trim();
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");
            
            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestItems SET actualQtyDiv = @quantity , isCheckedDiv = @isCheckedDiv WHERE reqNumber = @ReqNo AND itemDescription = @description;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@description", desc);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@isCheckedDiv", isCheckedDiv);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        public ActionResult insertBranchItemStatus(string ReqNo, string description, int quantity, int isCheckedBranch)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            string desc = description.Trim();
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestItems SET actualQtyBranch = @quantity, isCheckedBranch = @isCheckedBranch WHERE reqNumber = @ReqNo AND itemDescription = @description;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@description", desc);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@isCheckedBranch", isCheckedBranch);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        public ActionResult insertMMDItemStatus(string ReqNo, string description, int quantity, int isCheckedMMD)
        {
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            string desc = description.Trim();
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");
            
            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestItems SET actualQtyMMD = @quantity , isCheckedMMD = @isCheckedMMD WHERE reqNumber = @ReqNo AND itemDescription = @description;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@description", desc);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@isCheckedMMD", isCheckedMMD);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        public ActionResult insertSDCItemStatus(string ReqNo, string description, int quantity, int isCheckedSDC)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                string desc = description.Trim();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE OnlineRequest.requestItems SET actualQtySDC = @quantity , isCheckedSDC = @isCheckedSDC WHERE reqNumber = @ReqNo AND itemDescription = @description;";
                    cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                    cmd.Parameters.AddWithValue("@description", desc);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@isCheckedSDC", isCheckedSDC);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        // INVIDIDUAL STATUS
        public ActionResult MMDItemStatus(string ReqNo, string status, string description, string isDivrequest)
        {
            string faIcon = "";
            var ss = (ORSession)Session["UserSession"];
            var db = new ORtoMySql();
            string desc = description.Trim();
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");
            
            try
            {
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    if (isDivrequest == "1")
                    {
                        if (status == "Cancelled")
                        {
                            faIcon = "Cancelled";
                            cmd.CommandText = " UPDATE requestItems SET MMDStatus = @status , DivStatus = @status, itemStatus = @status " +
                                              " WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                        }
                        else
                        {
                            faIcon = "Open";
                            cmd.CommandText = " UPDATE requestItems SET MMDStatus = @status, DivStatus = 'Open', itemStatus = 'Open' " +
                                              " WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                        }
                    }
                    else
                    {
                        if (status == "Cancelled")
                        {
                            faIcon = "Cancelled";
                            cmd.CommandText = " UPDATE requestItems SET MMDStatus = @status , SDCStatus = @status, BranchStatus = @status,  itemStatus = @status " +
                                              " WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                        }
                        else
                        {
                            faIcon = "Open";
                            cmd.CommandText = " UPDATE requestItems SET MMDStatus = @status , SDCStatus = 'Open', BranchStatus = 'Open', itemStatus = 'Open' " +
                                              " WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                        }
                    }
                    cmd.Parameters.AddWithValue("@reqno", ReqNo);
                    cmd.Parameters.AddWithValue("@itemDescrption", desc);
                    cmd.Parameters.AddWithValue("@status", status.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request item: " + desc + "| status to: " + status + " has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    classIcon = faIcon,
                    msg = "Successfully updated."
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

        public ActionResult SDCSItemtatus(string ReqNo, string status, string description)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                string desc = description.Trim();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE requestItems SET SDCStatus = @status WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                    cmd.Parameters.AddWithValue("@reqno", ReqNo);
                    cmd.Parameters.AddWithValue("@itemDescrption", desc);
                    cmd.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request item: " + desc + "| status to: " + status + " has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        public ActionResult BranchItemStatus(string ReqNo, string status, string description)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                string desc = description.Trim();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE requestItems SET BranchStatus = @status, itemStatus = @status WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                    cmd.Parameters.AddWithValue("@reqno", ReqNo);
                    cmd.Parameters.AddWithValue("@itemDescrption", desc);
                    cmd.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request item: " + desc + "| status to: " + status + " has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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

        public ActionResult DivItemStatus(string ReqNo, string status, string description)
        {
            var ss = (ORSession)Session["UserSession"];
            if (ss == null) 
                return RedirectToAction("Logout", "Userlogin");

            try
            {
                var db = new ORtoMySql();
                string desc = description.Trim();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE requestItems SET DivStatus = @status, itemStatus = @status WHERE reqNumber = @reqno AND itemDescription = @itemDescrption";
                    cmd.Parameters.AddWithValue("@reqno", ReqNo);
                    cmd.Parameters.AddWithValue("@itemDescrption", desc);
                    cmd.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                log.Info("Request item: " + desc + "| status to: " + status + " has been updated; || request no: " + ReqNo + " || Processor: " + ss.s_usr_id);
                return Json(new
                {
                    status = true,
                    msg = "Successfully updated."
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
    }
}