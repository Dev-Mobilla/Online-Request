using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineRequestSystem.Areas.Administrator.Controllers
{
    public class RequestItemsController : Controller
    { 
        public ActionResult RequestItems()
        {
            return View();
        }
    }
}