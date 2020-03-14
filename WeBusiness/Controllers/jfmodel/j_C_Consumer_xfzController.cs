using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class j_C_Consumer_xfzController : BaseController
    {
        //
        // GET: /j_C_Consumer_xfz/


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
                where += string.Format(" and (C_Consumer.UserName like '%{0}%' or C_UserWxInfo.nickname like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(C_ConsumerSearch condition)
        {
            string where = StrWhere(condition);   
            where += " and C_Consumer.Type='消费者' ";
            PageJsonModel<C_Consumer> page = new PageJsonModel<C_Consumer>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_Consumer left join C_UserWxInfo on C_Consumer.UserName=C_UserWxInfo.C_ConsumerUserName ";
            page.strSelect = " C_Consumer.*,C_UserWxInfo.nickname wx_name,C_UserWxInfo.country wx_address,C_UserWxInfo.sex wx_sex,C_UserWxInfo.headimgurl wx_headurl ";
            page.strWhere = where;
            page.strOrder = "C_Consumer.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
