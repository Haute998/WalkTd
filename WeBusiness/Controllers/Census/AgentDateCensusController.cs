using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class AgentDateCensusController : BaseController
    {
        //
        // GET: /AgentDateCensus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAgentYearCensus(int Year)
        {
            string SelVal = AgentCensus.GetAgentCount(Year);
            if (string.IsNullOrWhiteSpace(SelVal))
            {
                return Content("");
            }
            return Content(SelVal);
        }
    }
}
