using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSNotifySubscribeController : BaseController
    {
        //
        // GET: /SYSNotifySubscribe/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(SYSNotifySearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (MsgType like '%{0}%' or MsgCode like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSNotifySearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSNotify> page = new PageJsonModel<SYSNotify>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSNotify ";
            page.strSelect = string.Format(" *,(case when REPLACE(Subscriber,',{0},',',')=',' then '' else REPLACE(Subscriber,',{0},',',') end) as elseSubscriber ", CurrentUser.UserName);
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("订阅", "Index")]
        public ActionResult SubscriberByID(int id, int type)
        {
            int rtn = SYSNotify.SubscriberByID(id, CurrentUser.UserName, type);
            return Content(rtn > 0 ? "ok" : "订阅失败");
        }

    }
}
