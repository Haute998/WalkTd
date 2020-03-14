using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class j_jfloguseController : BaseController
    {
        //
        // GET: /j_jfloguse/


        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(j_jflogSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (logContents like '%{0}%' or UserName like '%{0}%' or Code like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(j_jflogSearch condition)
        {
            string where = StrWhere(condition);
            where += " and Type='兑换' and UserType='消费者' ";
            PageJsonModel<j_jflog> page = new PageJsonModel<j_jflog>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " j_jflog ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
