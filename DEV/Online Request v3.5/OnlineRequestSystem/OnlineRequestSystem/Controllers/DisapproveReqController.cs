using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class DisapproveReqController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DisapproveReqController));
        private string format = "yyyy/MM/dd HH:mm:ss";
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;

        public ActionResult DisapproveRequest(CreateReqModels Aprv)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return RedirectToAction("Logout", "Userlogin");
            try
            {
                string Remarks = "";
                var mod = new CloseRequest();
                mod.Approver = Aprv.Approver;
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT * FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo";
                    cmd.Parameters.AddWithValue("@reqNo", Aprv.RequestNo);
                    conn.Open();
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
                        mod.ZoneCode = rdr["Zonecode"].ToString().Trim();
                        mod.reqStatus = rdr["reqStatus"].ToString().Trim();
                        mod.isApproved = rdr["isApproved"].ToString().Trim();
                        mod.isDivRequest = Convert.ToInt32(rdr["isDivRequest"]);
                        mod.forPresident = Convert.ToInt32(rdr["forPresident"]);
                        mod.DeptCode = rdr["DeptCode"].ToString().Trim();
                    }

                    #region [ Switching Approver ]

                    switch (Aprv.Approver)
                    {
                        case "DM":
                            Remarks = Aprv.E_DMRemarks;
                            break;

                        case "LocalDiv":
                            Remarks = Aprv.E_LocalDivRemarks;
                            break;

                        case "AM":
                            Remarks = Aprv.E_AMRemarks;
                            break;

                        case "RM":
                            Remarks = Aprv.E_RMRemarks;
                            break;

                        case "VPAssistant":
                            Remarks = Aprv.E_VPAssistantRemarks;
                            break;

                        case "GM":
                            Remarks = Aprv.E_GMRemarks;
                            break;

                        case "Pres":
                            Remarks = Aprv.E_PresRemarks;
                            break;

                        case "Div1":
                            Remarks = Aprv.E_DivRemarks;
                            break;

                        case "Div2":
                            Remarks = Aprv.E_Div2Remarks;
                            break;

                        case "Div3":
                            Remarks = Aprv.E_Div3Remarks;
                            break;

                        default:
                            break;
                    };

                    #endregion [ Switching Approver ]

                    if (InsertDisapproved(mod, Remarks) == "success")
                    {
                        cmd.CommandText = "DELETE FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo ";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@reqNo", Aprv.RequestNo);
                        conn.Open();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            log.Info("Request successfully disapproved and inserted into close table || with request no: " + mod.reqNumber + " || disapprover : " + mySession.s_fullname);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request disapproved.",
                                retURL = Aprv.returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        log.Fatal("Database Connection Error");
                        return Json(new
                        {
                            status = false,
                            rescode = "2000",
                            msg = "Database Connection Error"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                return View("Error");
            }
            return View();
        }

        public string InsertDisapproved(CloseRequest mod, string Remarks)
        {
            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) return "error";
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO OnlineRequest.onlineRequest_Close  " +
                            " (reqNumber, reqDate, reqCreator, TypeID, reqDescription, BranchCode, Region, Area, AreaCode ,DivCode, ZoneCode, " +
                            " reqStatus, isApproved, isDivRequest, ClosedDate, ClosedBy, Remarks, syscreated, syscreator, forPresident, deptCode ) " +
                            " VALUES (@reqNumber, @reqDate, @reqCreator, @TypeID, @reqDescription, @BranchCode, @Region, @Area, @AreaCode, @DivCode, @ZoneCode,  " +
                            " @reqStatus, @isApproved, @isDivRequest, @ClosedDate, @ClosedBy, @Remarks, @syscreated, @syscreator, @forPresident, @deptCode)";
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
                        cmd.Parameters.AddWithValue("@reqStatus", "DISAPPROVED");
                        cmd.Parameters.AddWithValue("@isApproved", mod.isApproved);
                        cmd.Parameters.AddWithValue("@isDivRequest", mod.isDivRequest);
                        cmd.Parameters.AddWithValue("@ClosedDate", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@ClosedBy", mySession.s_fullname);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@syscreated", syscreated.ToString(format));
                        cmd.Parameters.AddWithValue("@syscreator", mySession.s_usr_id);
                        cmd.Parameters.AddWithValue("@forPresident", mod.forPresident);
                        cmd.Parameters.AddWithValue("@deptCode", mod.DeptCode);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        UpdateEscalationRemarks(mod, Remarks, mySession);
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

        public bool UpdateEscalationRemarks(CloseRequest mod, string Remarks, ORSession session)
        {
            try
            {
                var db = new ORtoMySql();
                using (var conn = db.getConnection())
                {
                    var cmd = conn.CreateCommand();
                    switch (mod.Approver)
                    {
                        case "DM":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDM_Name = @E_DMName, EscalationDM_Date = @E_DMDate, EscalationDM_Remarks = @Remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@E_DMName", mod.E_DMName);
                            cmd.Parameters.AddWithValue("@E_DMDate", Convert.ToDateTime(mod.E_DMDate));
                            break;

                        case "LocalDiv":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationLocalDiv_Name = @LocalDiv_Name, EscalationLocalDiv_Date = @LocalDiv_Date, EscalationLocalDiv_Remarks = @Remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@LocalDiv_Name", mod.E_LocalDivName);
                            cmd.Parameters.AddWithValue("@LocalDiv_Date", Convert.ToDateTime(mod.E_LocalDivDate));
                            break;

                        case "AM":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationAM_Name = @E_AMname, EscalationAM_Date = @E_AMDate, EscalationAM_Remarks = @Remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@E_AMName", mod.E_AMName);
                            cmd.Parameters.AddWithValue("@E_AMDate", Convert.ToDateTime(mod.E_AMDate));
                            break;

                        case "RM":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationRM_Name = @E_RMname, EscalationRM_Date = @E_RMDate , EscalationRM_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_RMname", mod.E_RMName);
                            cmd.Parameters.AddWithValue("@E_RMDate", Convert.ToDateTime(mod.E_RMDate));
                            break;

                        case "VPAssistant":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationVPAssistant = @approver, EscalationVPAssistant_Date = @datenow, EscalationVPAssistant_Remarks = @Remarks WHERE reqNumber = @ReqNo ";
                            cmd.Parameters.AddWithValue("@approver", mod.E_VPAssistantName);
                            cmd.Parameters.AddWithValue("@datenow", Convert.ToDateTime(mod.E_VPAssistantDate));
                            break;

                        case "GM":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationGM_Name = @E_GMName, EscalationGM_Date = @E_GMDate , EscalationGM_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_GMName", mod.E_GMName);
                            cmd.Parameters.AddWithValue("@E_GMDate", Convert.ToDateTime(mod.E_GMDate));
                            break;

                        case "Pres":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_Name = @E_PresName, EscalationPres_Date = @E_PresDate , EscalationPres_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_PresName", mod.E_PresName);
                            cmd.Parameters.AddWithValue("@E_PresDate", Convert.ToDateTime(mod.E_PresDate));
                            break;

                        case "Div1":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @E_DivName , EscalationDiv_Date = @E_DivDate , EscalationDiv_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_DivName", mod.E_DivName);
                            cmd.Parameters.AddWithValue("@E_DivDate", Convert.ToDateTime(mod.E_DivDate));
                            break;

                        case "Div2":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @E_Div2Name , EscalationDiv2_Date = @E_Div2Date , EscalationDiv2_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_Div2Name", mod.E_Div2Name);
                            cmd.Parameters.AddWithValue("@E_Div2Date", Convert.ToDateTime(mod.E_Div2Date));
                            break;

                        case "Div3":
                            cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @E_Div3Name , EscalationDiv3_Date = @E_Div3Date , EscalationDiv3_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@E_Div3Name", mod.E_Div3Name);
                            cmd.Parameters.AddWithValue("@E_Div3Date", Convert.ToDateTime(mod.E_Div3Date));
                            break;
                    }

                    cmd.Parameters.AddWithValue("@ReqNo", mod.reqNumber);
                    cmd.Parameters.AddWithValue("@Remarks", Remarks);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    log.Info("Sucessfully Inserted to Close Table " + mod.reqNumber);
                    return true;
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                throw;
            }
        }
    }
}