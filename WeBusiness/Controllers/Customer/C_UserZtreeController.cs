using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class C_UserZtreeController : BaseController
    {
        //
        // GET: /C_UserZtree/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            ViewData["ParametersVal"] = BaseParameters.GetEntityByID(1).ParametersVal;
            ViewData["ChiefCount"] = C_User.GetOptionzTreeMenu(0).Count;
            return View();
        }
        public ActionResult GetOptionTree(int ID)
        {
            if (!string.IsNullOrWhiteSpace(C_User.GetOptionTreeMenu(ID)))
            {
                return Json(C_User.GetOptionTreeMenu(ID), JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet); ;
        }
        public ActionResult GetUserMsg(int ID)
        {
            SearchZtree data = C_User.GetOptionTreeUserMsg(ID);
            if (data != null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet); ;
        }



    }
}
