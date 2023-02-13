using MySql.Data.MySqlClient;
using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Data;
using System.Globalization;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class ApprovStatController : Controller
    {
        #region In
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ApprovStatController));
        private string format = "yyyy/MM/dd HH:mm:ss";
        private Helper que = new Helper();
        private DateTime syscreated = DateTime.Now;
        private DateTime sysmodified = DateTime.Now;
        #endregion In

        #region Approve Requests
        public ActionResult ApprovedRequest(int Status, CreateReqModels Aprv, string forPresident, string returnUrl)
        {
            Aprv.returnUrl = returnUrl;
            if (Status == 0)
                return RedirectToAction("DisapproveRequest", "DisapproveReq", Aprv);

            var mySession = (ORSession)Session["UserSession"];
            if (mySession == null) { return RedirectToAction("Logout", "Userlogin"); }
            TempData["Status"] = Status;
            var ReqStatus = (ReqApproverStatus)(Aprv);
            var Recommended = (RecommendedApproval)(Aprv);
            var db = new ORtoMySql();
            try
            {
                if (Aprv.Approver == "DM" && Aprv.ForPO == 0)
                {
                    if (InsertApproveRequest(Aprv))
                    {
                        log.Info("A request has been approved by DEPARTMENT MANAGER || " + Aprv.Sts_DM_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = true,
                            rescode = "2000",
                            msg = "Request successfully approved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        log.Info("A request has been disapproved by DEPARTMENT MANAGER || " + Aprv.Sts_DM_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = false,
                            rescode = "2001",
                            msg = "Request disapproved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (Aprv.Approver == "LocalDiv" && Aprv.ForPO == 0)
                {
                    if (InsertApproveRequest(Aprv))
                    {
                        log.Info("A request has been approved by DIVISION MANAGER || " + Aprv.Sts_LocalDiv_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = true,
                            rescode = "2000",
                            msg = "Request successfully approved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        log.Info("A request has been disapproved by DIVISION MANAGER || " + Aprv.Sts_LocalDiv_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = false,
                            rescode = "2001",
                            msg = "Request disapproved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (Recommended.isAMApproval == 1 && Aprv.ForPO == 0)
                {
                    if (mySession.s_job_title == "AREA MANAGER")
                    {
                        if (InsertApproveRequest(Aprv))
                        {
                            log.Info("A request has been approved by AREA MANAGER || " +
                            Aprv.Sts_AM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request successfully approved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            log.Info("A request has been disapproved by AREA MANAGER || " +
                            Aprv.Sts_AM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = false,
                                rescode = "2001",
                                msg = "Request disapproved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                if (Recommended.isRMApproval == 1 && Aprv.ForPO == 0)
                {
                    if (mySession.s_job_title == "REGIONAL MAN")
                    {
                        if (InsertApproveRequest(Aprv))
                        {
                            log.Info("A request has been approved by  REGIONAL MANAGER || " +
                            Aprv.Sts_RM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request successfully approved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            log.Info("A request has been disapproved by REGIONAL MANAGER || " +
                            Aprv.Sts_RM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = false,
                                rescode = "2001",
                                msg = "Request disapproved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                if (Aprv.ForPO == 0 && mySession.s_job_title != "GMO-GENMAN" && (Recommended.isDivManApproval == 1 || Recommended.isDivManApproval2 == 1 || Recommended.isDivManApproval3 == 1))
                {
                    if (mySession.s_isDivisionApprover == 1)
                    {
                        if (InsertApproveRequest(Aprv))
                        {
                            log.Info("A request has been approved by DIVISION APPROVER ||Division 1: "
                            + Aprv.Sts_Div1_Approver + " |Division 2: " + Aprv.Sts_Div2_Approver
                            + " |Division 3: " + Aprv.Sts_Div3_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request successfully approved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            log.Info("A request has been disapproved by DIVISION APPROVER ||Division 1: "
                            + Aprv.Sts_Div1_Approver + " |Division 2: " + Aprv.Sts_Div2_Approver + " |Division 3: "
                            + Aprv.Sts_Div3_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = false,
                                rescode = "2001",
                                msg = "Request disapproved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (Recommended.isGMApproval == 1 || Aprv.ForPO == 1)
                {
                    if (mySession.s_job_title == "GMO-GENMAN")
                    {
                        if (InsertApproveRequest(Aprv))
                        {
                            log.Info("A request has been approved by GENERAL MANAGER || "
                            + Aprv.Sts_GM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request successfully Approved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            log.Info("A request has been disapproved by GENERAL MANAGER || "
                            + Aprv.Sts_GM_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = false,
                                rescode = "2001",
                                msg = "Request Disapproved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (Aprv.Approver == "VPAssistant" && Aprv.ForPO == 0)
                {
                    if (InsertApproveRequest(Aprv))
                    {
                        log.Info("A request has been approved by VP Assistant || "
                        + Aprv.Sts_VPAssistant_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = true,
                            rescode = "2000",
                            msg = "Request successfully Approved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        log.Info("A request has been disapproved by VP Assistant || "
                        + Aprv.Sts_VPAssistant_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = false,
                            rescode = "2001",
                            msg = "Request Disapproved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (Recommended.isPresidentApproval == 1 || Aprv.ForPO == 1)
                {
                    if (mySession.s_usr_id == "LHUI1011873")
                    {
                        if (InsertApproveRequest(Aprv))
                        {
                            log.Info("A request has been approved by PRESIDENT || "
                            + Aprv.Sts_Pres_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = true,
                                rescode = "2000",
                                msg = "Request successfully Approved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            log.Info("A request has been disapproved by PRESIDENT || "
                            + Aprv.Sts_Pres_Approver + " || request no:" + Aprv.RequestNo);
                            return Json(new
                            {
                                status = false,
                                rescode = "2001",
                                msg = "Request Disapproved.",
                                retURL = returnUrl
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (forPresident == "1" || Aprv.ForPO == 1)
                {
                    if (InsertApproveRequest(Aprv))
                    {
                        log.Info("A request has been approved by PRESIDENT || "
                        + Aprv.Sts_Pres_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = true,
                            rescode = "2000",
                            msg = "Request successfully Approved.",
                            retURL = returnUrl
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        log.Info("A request has been disapproved by PRESIDENT || "
                        + Aprv.Sts_Pres_Approver + " || request no:" + Aprv.RequestNo);
                        return Json(new
                        {
                            status = false,
                            rescode = "2001",
                            msg = "Request Disapproved.",
                            retURL = returnUrl
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
            return View(Aprv);
        }
        #endregion Approve Requests

        #region Escalation Checking
        public bool EscalationChecking(string ReqNo)
        {
            var db = new ORtoMySql();
            using (var conn = db.getConnection())
            {
                var cmd = new MySqlCommand("SELECT * FROM OnlineRequest.onlineRequest_Escalation WHERE reqNumber = @ReqNo", conn);
                cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
                conn.Open();
                var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (rdr.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion Escalation Checking

        #region Insert Approve Request
        public bool InsertApproveRequest(CreateReqModels Aprv)
        {
            ORtoMySql db = new ORtoMySql();
            var mySession = (ORSession)Session["UserSession"];
            try
            {
                var Status = TempData["Status"];
                var ReqStatus = (ReqApproverStatus)(Aprv);
                var Recommended = (RecommendedApproval)(Aprv);
                using (MySqlConnection conn = db.getConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();

                    if (Convert.ToInt32(Status) == 1)
                    {
                        switch (Aprv.Approver)
                        {
                            case "DM":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDM_Name = @E_DMName, EscalationDM_Date = @E_DMDate, EscalationDM_Remarks = @E_DMRemarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@E_DMName", Aprv.E_DMName);
                                cmd.Parameters.AddWithValue("@E_DMDate", Convert.ToDateTime(Aprv.E_DMDate));
                                cmd.Parameters.AddWithValue("@E_DMRemarks", Aprv.E_DMRemarks);
                                break;

                            case "LocalDiv":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationLocalDiv_Name = @E_LocalDivName, EscalationLocalDiv_Date = @E_LocalDivDate, EscalationLocalDiv_Remarks = @E_LocalDivRemarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@E_LocalDivName", Aprv.E_LocalDivName);
                                cmd.Parameters.AddWithValue("@E_LocalDivDate", Convert.ToDateTime(Aprv.E_LocalDivDate));
                                if (Aprv.E_LocalDivRemarks == "" || string.IsNullOrEmpty(Aprv.E_LocalDivRemarks))
                                {
                                    Aprv.E_LocalDivRemarks = "Approved by Division Manager";
                                }
                                cmd.Parameters.AddWithValue("@E_LocalDivRemarks", Aprv.E_LocalDivRemarks);
                                break;

                            case "AM":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationAM_Name = @E_AMname, EscalationAM_Date = @E_AMDate, EscalationAM_Remarks = @E_AMRemarks WHERE reqNumber = @ReqNo ";
                                cmd.Parameters.AddWithValue("@E_AMName", Aprv.E_AMName);
                                cmd.Parameters.AddWithValue("@E_AMDate", Convert.ToDateTime(Aprv.E_AMDate));
                                cmd.Parameters.AddWithValue("@E_AMRemarks", Aprv.E_AMRemarks);
                                break;

                            case "RM":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationRM_Name = @E_RMname, EscalationRM_Date = @E_RMDate , EscalationRM_Remarks = @E_RMRemarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_RMname", Aprv.E_RMName);
                                cmd.Parameters.AddWithValue("@E_RMDate", Convert.ToDateTime(Aprv.E_RMDate));
                                cmd.Parameters.AddWithValue("@E_RMRemarks", Aprv.E_RMRemarks);
                                break;

                            case "VPAssistant":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationVPAssistant = @E_VPAssistant, EscalationVPAssistant_Date = @E_VPAssDate , EscalationVPAssistant_Remarks = @E_VPAssRemarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_VPAssistant", Aprv.E_VPAssistantName);
                                cmd.Parameters.AddWithValue("@E_VPAssDate", Convert.ToDateTime(Aprv.E_VPAssistantDate));
                                if (Aprv.E_VPAssistantRemarks == "" || string.IsNullOrEmpty(Aprv.E_VPAssistantRemarks))
                                {
                                    Aprv.E_VPAssistantRemarks = "Approved by VP Assistant";
                                }
                                cmd.Parameters.AddWithValue("@E_VPAssRemarks", Aprv.E_VPAssistantRemarks);
                                break;

                            case "GM":
                                if (Aprv.ForPO == 1)
                                {
                                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationVPO_PO_Name = @E_VPO_POName, EscalationVPO_PO_Date = @E_VPO_PODate , EscalationVPO_PO_Remarks = @E_VPO_PORemarks WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@E_VPO_POName", Aprv.E_VPO_POName);
                                    cmd.Parameters.AddWithValue("@E_VPO_PODate", Convert.ToDateTime(Aprv.E_VPO_PODate));
                                    if (Aprv.E_VPO_PORemarks == "" || string.IsNullOrEmpty(Aprv.E_VPO_PORemarks))
                                    {
                                        Aprv.E_VPO_PORemarks = "PO Approved by VPO";
                                    }
                                    cmd.Parameters.AddWithValue("@E_VPO_PORemarks", Aprv.E_VPO_PORemarks);
                                    break;
                                }
                                else
                                {
                                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationGM_Name = @E_GMName, EscalationGM_Date = @E_GMDate , EscalationGM_Remarks = @E_GMRemarks WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@E_GMName", Aprv.E_GMName);
                                    cmd.Parameters.AddWithValue("@E_GMDate", Convert.ToDateTime(Aprv.E_GMDate));
                                    if (Aprv.E_GMRemarks == "" || string.IsNullOrEmpty(Aprv.E_GMRemarks))
                                    {
                                        Aprv.E_GMRemarks = "Approved by Division Manager";
                                    }
                                    cmd.Parameters.AddWithValue("@E_GMRemarks", Aprv.E_GMRemarks);
                                    break;
                                }

                            case "Pres":
                                if (Aprv.ForPO == 1)
                                {
                                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_PO_Name = @E_Pres_POName, EscalationPres_PO_Date = @E_Pres_PODate , EscalationPres_PO_Remarks = @E_Pres_PORemarks WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@E_Pres_POName", Aprv.E_Pres_POName);
                                    cmd.Parameters.AddWithValue("@E_Pres_PODate", Convert.ToDateTime(Aprv.E_Pres_PODate));
                                    cmd.Parameters.AddWithValue("@E_Pres_PORemarks", Aprv.E_Pres_PORemarks);
                                    break;
                                }
                                else
                                {
                                    cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationPres_Name = @E_PresName, EscalationPres_Date = @E_PresDate , EscalationPres_Remarks = @E_PresRemarks WHERE reqNumber = @ReqNo";
                                    cmd.Parameters.AddWithValue("@E_PresName", Aprv.E_PresName);
                                    cmd.Parameters.AddWithValue("@E_PresDate", Convert.ToDateTime(Aprv.E_PresDate));
                                    cmd.Parameters.AddWithValue("@E_PresRemarks", Aprv.E_PresRemarks);
                                    break;
                                }

                            case "Div1":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv_Name = @E_DivName , EscalationDiv_Date = @E_DivDate , EscalationDiv_Remarks = @E_DivRemarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_DivName", Aprv.E_DivName);
                                cmd.Parameters.AddWithValue("@E_DivDate", Convert.ToDateTime(Aprv.E_DivDate));
                                cmd.Parameters.AddWithValue("@E_DivRemarks", Aprv.E_DivRemarks);
                                break;

                            case "Div2":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv2_Name = @E_Div2Name , EscalationDiv2_Date = @E_Div2Date , EscalationDiv2_Remarks = @E_Div2Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_Div2Name", Aprv.E_Div2Name);
                                cmd.Parameters.AddWithValue("@E_Div2Date", Convert.ToDateTime(Aprv.E_Div2Date));
                                cmd.Parameters.AddWithValue("@E_Div2Remarks", Aprv.E_Div2Remarks);
                                break;

                            case "Div3":
                                cmd.CommandText = "UPDATE OnlineRequest.onlineRequest_Escalation SET EscalationDiv3_Name = @E_Div3Name , EscalationDiv3_Date = @E_Div3Date , EscalationDiv3_Remarks = @E_Div3Remarks WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@E_Div3Name", Aprv.E_Div3Name);
                                cmd.Parameters.AddWithValue("@E_Div3Date", Convert.ToDateTime(Aprv.E_Div3Date));
                                cmd.Parameters.AddWithValue("@E_Div3Remarks", Aprv.E_Div3Remarks);
                                break;
                        }
                        cmd.Parameters.AddWithValue("@ReqNo", Aprv.RequestNo);
                        cmd.ExecuteNonQuery();

                        UpdateApproverStatus(Convert.ToInt32(Status), Aprv);
                        que.SetDateModified(Aprv.RequestNo, mySession.s_usr_id);
                        return true;
                    }
                    return true;
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return false;
            }
        }
        #endregion Insert Approve Request

        #region Update approver status
        public void UpdateApproverStatus(int Status, CreateReqModels Aprv)
        {
            var mySession = (ORSession)Session["UserSession"];
            ORtoMySql db = new ORtoMySql();
            var ReqStatus = (ReqApproverStatus)(Aprv);
            var Recommended = (RecommendedApproval)(Aprv);
            try
            {
                using (MySqlConnection conn = db.getConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    switch (Aprv.Approver)
                    {
                        case "DM":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET DM_Approver = @Approver , DM_Approved_Date = @Date, isApprovedDM = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@isApproved", Aprv.Sts_DM_isApproved);
                            break;

                        case "LocalDiv":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET LocalDiv_Approver = @Approver, LocalDiv_Approved_Date = @Date, isApprovedLocalDiv = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@isApproved", Aprv.Sts_LocalDiv_isApproved);
                            break;

                        case "AM":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET AM_Approver = @Approver , AM_Approved_Date = @Date, isApprovedAM = @isApproved WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@isApproved", Aprv.Sts_AM_isApproved);
                            break;

                        case "RM":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET RM_Approver = @Approver , RM_Approved_Date = @Date, isApprovedRM = @isApprovedRM WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@isApprovedRM", Aprv.Sts_RM_isApproved);
                            break;

                        case "VPAssistant":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET VPAssistant_Approver = @Approver, VPAssistant_Approved_Date = @Date , isApprovedVPAssistant = @isApprovedVPAss WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@isApprovedVPAss", Aprv.Sts_VPAssistant_isApproved);
                            break;

                        case "GM":
                            if (Aprv.ForPO == 1)
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET VPO_PO_Approver = @Approver, VPO_PO_Approved_Date = @Date , isVPO_PO_Approved = @isVPO_PO_Approved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@isVPO_PO_Approved", Aprv.Sts_VPO_PO_isApproved);
                                break;
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET GM_Approver = @Approver, GM_Approved_Date = @Date , isApprovedGM = @isApprovedGM WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@isApprovedGM", Aprv.Sts_GM_isApproved);
                                break;
                            }

                        case "Div1":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver1 = @Approver , DivCode1 = @DivCode , Div_Approved_Date1 = @Date , isApprovedDiv1 = @isApprovedDiv1 WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@DivCode", mySession.s_DivCode);
                            cmd.Parameters.AddWithValue("@isApprovedDiv1", Aprv.Sts_Div1_isApproved);
                            break;

                        case "Div2":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver2 = @Approver , DivCode2 = @DivCode , Div_Approved_Date2 = @Date , isApprovedDiv2 = @isApprovedDiv2 WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@DivCode", mySession.s_DivCode);
                            cmd.Parameters.AddWithValue("@isApprovedDiv2", Aprv.Sts_Div2_isApproved);
                            break;

                        case "Div3":
                            cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET Div_Approver3 = @Approver , DivCode3 = @DivCode , Div_Approved_Date3 = @Date , isApprovedDiv3 = @isApprovedDiv3 WHERE reqNumber = @ReqNo";
                            cmd.Parameters.AddWithValue("@DivCode", mySession.s_DivCode);
                            cmd.Parameters.AddWithValue("@isApprovedDiv3", Aprv.Sts_Div3_isApproved);
                            break;

                        case "Pres":
                            if (Aprv.ForPO == 1)
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET Pres_PO_Approver = @Approver , Pres_PO_Approved_Date = @Date , isPres_PO_Approved = @isPres_PO_Approved , isPO_Approved = @isPOApproved WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@isPres_PO_Approved", Aprv.Sts_Pres_PO_isApproved);
                                cmd.Parameters.AddWithValue("@isPOApproved", 1);
                                break;
                            }
                            else
                            {
                                cmd.CommandText = "UPDATE OnlineRequest.requestApproverStatus SET Pres_Approver = @Approver , Pres_Approved_Date = @Date , isApprovedPres = @isApprovedPres WHERE reqNumber = @ReqNo";
                                cmd.Parameters.AddWithValue("@isApprovedPres", Aprv.Sts_Pres_isApproved);
                                break;
                            }
                    }
                    cmd.Parameters.AddWithValue("@Date", syscreated.ToString(format, CultureInfo.InvariantCulture));
                    cmd.Parameters.AddWithValue("@Approver", mySession.s_usr_id);
                    cmd.Parameters.AddWithValue("@ReqNo", Aprv.RequestNo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                ViewBag.Error = x.ToString();
                throw;
            }
        }
        #endregion Update approver status
    }
}