using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class j_C_ConsumerController : BaseController
    {
        //
        // GET: /j_C_Consumer/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(C_ConsumerSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (UserName like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(C_ConsumerSearch condition)
        {
            string where = StrWhere(condition);
            where += " and Type='促销员' and Stat='未审核' ";
            PageJsonModel<C_Consumer> page = new PageJsonModel<C_Consumer>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_Consumer ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }



        [B_MenuRightsTag("审核", "Index")]
        public ContentResult ToAudit(int id)
        {
            int[] ids = { id};
            if (C_Consumer.ToAudits(ids))
            {
                return Content("ok");
            }
            return Content("审核出错");
        }
        [B_MenuRightsTag("批量审核", "Index")]
        public ContentResult ToAudits(int[] ids)
        {
            bool rtn = C_Consumer.ToAudits(ids);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }

    }
}
