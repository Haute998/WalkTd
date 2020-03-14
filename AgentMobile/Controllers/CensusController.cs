using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class CensusController : BaseController
    {
        //
        // GET: /Census/

        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }

    }
}
