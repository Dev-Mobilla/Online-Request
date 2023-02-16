using OnlineRequestSystem.Models;
using OnlineRequestSystem.Service;
using System;
using System.Web.Mvc;

namespace OnlineRequestSystem.Controllers
{
    public class MultipleApprovingController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MultipleApprovingController));
        private MultiApproving que = new MultiApproving();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult AM_approve(string ReqNO)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {               
                if (que.ApproveAM(ReqNO, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
                return Json(new
                {
                    status = false,
                    msg = ex
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RM_Approve(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApproveRM(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DM_Approve(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApproveDM(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DivApprover(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApproveDivApprover(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult VPAssistantApprover(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApproveVPAssistant(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult VPApprover(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApproveVP(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PresidentApprove(string ReqNo)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.ApprovedPresident(ReqNo, ss) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully approved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MultipleDisapprove(string ReqNo, string ForPO, string approver)
        {
            var ss = (ORSession)Session["UserSession"];
            try
            {
                if (que.MultipleDisapprove(ReqNo, ForPO, ss, approver) == "success")
                {
                    return Json(new
                    {
                        status = true,
                        msg = "Successfully Disapproved!"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = false,
                        msg = "Unable to process request!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                log.Fatal(x.Message, x);
                return Json(new
                {
                    status = false,
                    msg = x
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}