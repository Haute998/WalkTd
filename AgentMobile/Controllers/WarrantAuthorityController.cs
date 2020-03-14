using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class WarrantAuthorityController : Controller
    {
        //
        // GET: /WarrantAuthority/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AuthorityAgent(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Content("不能为空！！");
            }
            C_User cuser = C_User.GetC_UserBy(" and Phone='" + id + "'");
            if (cuser == null)
            {
                return Content("此代理没有授权");
            }
            if (cuser.state != "已审核")
            {
                return Content("此代理没有授权");
            }
            return Content("ok");
        }
        public ActionResult AgentIndex(string id)
        {
            C_User cuser = C_User.GetC_UserBy(" and Phone='" + id + "'");
            return View(cuser);
        }
    }
}
