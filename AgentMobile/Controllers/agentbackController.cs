using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class agentbackController : BaseController
    {
        //
        // GET: /agentback/

        public ActionResult Index()
        {  
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            ViewData["C_UserNoVerity"] = C_User.GetC_UserCircles(CurrentUser.ID);
            //ViewData["OrderNoVerity"] = Order.GetC_UserCircles(CurrentUser.UserName);
            ViewData["Upgrade"] = C_UserUpGrade.GetCountUpGrade(CurrentUser.UserName);
            return View();
        }
        public ActionResult agentNoAudit()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }
        public ActionResult ScaleIndex()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }
        public ActionResult GetLeverList(string SelVal, int Lever)
        {
            List<C_UserType> cType = C_UserType.GetEntitysAll().ToList();
            List<C_UserType> nType = new List<C_UserType>();
            if (SelVal == "授权链接")
            {
                nType = cType.Where(m => m.Lever > Lever).ToList();
            }
            else
            {
                nType = cType.Where(m => m.Lever <= Lever).ToList();
            }
            return Json(nType, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 代理审核&&意向代理
        /// </summary>
        /// <returns></returns>
        public ActionResult agentyixiang(string username="") 
        {
            ViewData["username"] = username;
            return View();
        }

    }
}
