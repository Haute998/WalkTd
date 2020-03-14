using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class PublicShowController : Controller
    {
        //
        // GET: /PublicShow/

        public ActionResult CompanyProfile()
        {
            SYSCompanyProfile Profile = SYSCompanyProfile.GetProfile();
            if (Profile == null)
            {
                Profile = new SYSCompanyProfile();
            }
            return View(Profile);
        }

        public ActionResult support() 
        {
            return View();
        }

        /// <summary>
        /// 代理审核&&意向代理
        /// </summary>
        /// <returns></returns>
        public ActionResult agentyixiang(string username = "")
        {
            ViewData["username"] = username;
            return View();
        }
    }
}
