using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineRequestSystem.Areas.Administrator.Controllers;

namespace OnlineRequestSystem.Areas.Administrator.Controllers
{

    public class HomeController : Controller
    {
        OpenRequestController x = new OpenRequestController();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Authenticate(string key)
        {

            try
            {
                if (key == "Mlinc1234")
                {
                    Session["Utility"] = "1";
                    return RedirectToAction("OpenRequest", "OpenRequest", new { Area = "Administrator" });
                }
                else
                {                    
                    return View("_Unauthorized");
                }
            }
            catch (Exception)
            {
                return View("_Unauthorized");
            }
        }
    }
}