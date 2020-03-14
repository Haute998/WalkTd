using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class SYSMNotifyController : BaseController
    {
        //
        // GET: /SYSMNotify/
        /// <summary>
        /// 获取公告显示
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTop5()
        {
            List<SYSMNotify> notifys = SYSMNotify.GetTop5BySort();
            if (notifys == null || notifys.Count <= 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(notifys, JsonRequestBehavior.AllowGet);
        }
    }
}
