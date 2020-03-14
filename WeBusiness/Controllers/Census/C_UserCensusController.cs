using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;

namespace WeBusiness.Controllers
{
    public class C_UserCensusController : BaseController
    {
        //
        // GET: /C_UserCensus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

    }
}
