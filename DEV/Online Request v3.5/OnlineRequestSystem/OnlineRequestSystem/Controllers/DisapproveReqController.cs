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

                    mod.Approver = Aprv.Approver;
                    mod.ForPO = Aprv.ForPO;

                    mod.E_DMName = Aprv.E_DMName;
                    mod.E_DMDate = Aprv.E_DMDate;
                    mod.E_DMRemarks = Aprv.E_DMRemarks;

                    mod.E_LocalDivName = Aprv.E_LocalDivName;
                    mod.E_LocalDivDate = Aprv.E_LocalDivDate;
                    mod.E_LocalDivRemarks = Aprv.E_LocalDivRemarks;

                    mod.E_AMName = Aprv.E_AMName;
                    mod.E_AMDate = Aprv.E_AMDate;
                    mod.E_AMRemarks = Aprv.E_AMRemarks;

                    mod.E_RMName = Aprv.E_RMName;
                    mod.E_RMDate = Aprv.E_RMDate;
                    mod.E_RMRemarks = Aprv.E_RMRemarks;

                    mod.E_VPAssistantName = Aprv.E_VPAssistantName;
                    mod.E_VPAssistantDate = Aprv.E_VPAssistantDate;
                    mod.E_VPAssistantRemarks = Aprv.E_VPAssistantRemarks;

                    mod.E_GMName = Aprv.E_GMName;
                    mod.E_GMDate = Aprv.E_GMDate;
                    mod.E_GMRemarks = Aprv.E_GMRemarks;

                    mod.E_PresName = Aprv.E_PresName;
                    mod.E_PresDate = Aprv.E_PresDate;
                    mod.E_PresRemarks = Aprv.E_PresRemarks;

                    mod.E_DivName = Aprv.E_DivName;
                    mod.E_DivDate = Aprv.E_DivDate;
                    mod.E_DivRemarks = Aprv.E_DivRemarks;

                    mod.E_Div2Name = Aprv.E_Div2Name;
                    mod.E_Div2Date = Aprv.E_Div2Date;
                    mod.E_Div2Remarks = Aprv.E_Div2Remarks;

                    mod.E_Div3Name = Aprv.E_Div3Name;
                    mod.E_Div3Date = Aprv.E_Div3Date;
                    mod.E_Div3Remarks = Aprv.E_Div3Remarks;

                    mod.E_VPO_POName = Aprv.E_VPO_POName;
                    mod.E_VPO_PODate = Aprv.E_VPO_PODate;
                    mod.E_VPO_PORemarks = Aprv.E_VPO_PORemarks;

                    mod.E_Pres_POName = Aprv.E_Pres_POName;
                    mod.E_Pres_PODate = Aprv.E_Pres_PODate;
                    mod.E_Pres_PORemarks = Aprv.E_Pres_PORemarks;

                    #region [ Switching Approver ]                   

                    switch (mod.Approver)
                    {
                        case "DM":
                            Remarks = mod.E_DMRemarks;
                            break;

                        case "LocalDiv":
                            Remarks = mod.E_LocalDivRemarks;
                            break;

                        case "AM":
                            Remarks = mod.E_AMRemarks;
                            break;

                        case "RM":
                            Remarks = mod.E_RMRemarks;
                            break;

                        case "VPAssistant":
                            Remarks = mod.E_VPAssistantRemarks;
                            break;

                        case "GM":
                            if (mod.ForPO == 1)
                            {
                                Remarks = mod.E_VPO_PORemarks;
                            }
                            else
                            {
                                Remarks = mod.E_GMRemarks;
                            }

                            break;

                        case "Pres":
                            if (mod.ForPO == 1)
                            {
                                Remarks = mod.E_Pres_PORemarks;
                            }
                            else
                            {
                                Remarks = mod.E_PresRemarks;
                            }

                            break;

                        case "Div1":
                            Remarks = mod.E_DivRemarks;
                            break;

                        case "Div2":
                            Remarks = mod.E_Div2Remarks;
                            break;

                        case "Div3":
                            Remarks = mod.E_Div3Remarks;
                            break;

                        default:
                            break;
                    };

                    #endregion [ Switching Approver ]

                    if (InsertDisapproved(mod, Remarks) == "success")
                    {
                        cmd.CommandText = "DELETE FROM OnlineRequest.onlineRequest_Open WHERE reqNumber = @reqNo ";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@reqNo", mod.reqNumber);
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
                            if (mod.ForPO == 1)
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationVPO_PO_Name = @E_VPO_PO_Name, EscalationVPO_PO_Date = @E_VPO_PO_Date , EscalationVPO_PO_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_VPO_PO_Name", mod.E_VPO_POName);
                                cmd.Parameters.AddWithValue("@E_VPO_PO_Date", Convert.ToDateTime(mod.E_VPO_PODate));
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationGM_Name = @E_GMName, EscalationGM_Date = @E_GMDate , EscalationGM_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_GMName", mod.E_GMName);
                                cmd.Parameters.AddWithValue("@E_GMDate", Convert.ToDateTime(mod.E_GMDate));
                            }

                            break;

                        case "Pres":
                            if (mod.ForPO == 1)
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_PO_Name = @E_Pres_PO_Name, EscalationPres_PO_Date = @E_Pres_PO_Date , EscalationPres_PO_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_Pres_PO_Name", mod.E_Pres_POName);
                                cmd.Parameters.AddWithValue("@E_Pres_PO_Date", Convert.ToDateTime(mod.E_Pres_PODate));
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_Name = @E_PresName, EscalationPres_Date = @E_PresDate , EscalationPres_Remarks = @Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_PresName", mod.E_PresName);
                                cmd.Parameters.AddWithValue("@E_PresDate", Convert.ToDateTime(mod.E_PresDate));
                            }

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