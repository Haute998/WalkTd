using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class j_mainController : Controller
    {
        //
        // GET: /j_main/

        public ActionResult Index(string code="")
        {
            ViewData["code"] = code;
            return View();
        }



    }
}
