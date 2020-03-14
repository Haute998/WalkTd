using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSLogController : BaseController
    {
        //
        // GET: /SYSLog/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(SYSLogSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Logs like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSLogSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSLog> page = new PageJsonModel<SYSLog>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSLog ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
