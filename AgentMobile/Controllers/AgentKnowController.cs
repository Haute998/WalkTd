using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels.Models.C_UserModel;
using WeModels;

namespace AgentMobile.Controllers
{
    public class AgentKnowController : BaseController
    {
        //
        // GET: /AgentKnow/

        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            ViewData["SYSAgentKnow"] = SYSAgentKnow.GetNavAdvsMobile();
            return View();
        }
        public ActionResult ProfityIndex()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            ViewData["SYSPolicy"] = SYSPolicy.GetNavAdvsMobile();
            return View();
        }

    }
}
