using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class AgentIntentionController : BaseController
    {
        //
        // GET: /AgentIntention/


        //
        // GET: /AgentIntention/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(AgentIntentionSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (a.Name like '%{0}%' or a.Phone like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(AgentIntentionSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<AgentIntention> page = new PageJsonModel<AgentIntention>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " AgentIntention a left join C_User us on a.ChiefUser=us.UserName ";
            page.strSelect = " a.*,us.Name ChiefUser_Name ";
            page.strWhere = where;
            page.strOrder = " a.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (AgentIntention.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("批量删除", "Index")]
        public ContentResult ToDels(int[] ids)
        {
            bool rtn = AgentIntention.ToDels(ids);
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
