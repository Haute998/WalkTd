using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class AgentCensusController : BaseController
    {
        //
        // GET: /AgentCensus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult YJIndex()
        {
            return View();
        }
        public ActionResult YJCustomerPieCensus(string key)
        {
            string value = C_User.YJGetPieC_UserCountCensus(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(value, JsonRequestBehavior.AllowGet);
        }


   

    }
}
