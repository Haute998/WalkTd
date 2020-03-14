using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class ActivityMsgController : Controller
    {
        //
        // GET: /ActivityMsg/

        public ActionResult Index()
        {
            ViewBag.ActiveList = LotteryActivitys.GetEntitysAll();

            return View();
        }

    }
}
