using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineRequestSystem.Areas.Administrator.Controllers
{
    public class ApproverStatusController : Controller
    {
        public string Index()
        {
            if (Convert.ToInt32(Session["Utility"]) == 1)
            {
                return "Approver status";
            }
            else
            {
                return "Not authorized to access.";
            }      
        }
    }
}