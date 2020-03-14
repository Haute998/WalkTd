using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class BlackCustomerController : Controller
    {
        //
        // GET: /BlackCustomer/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetBlackPage(C_UserSearch condition)
        {
            string where = string.Empty;
            where += " and state='黑名单' ";
            return GetPages(condition, where);
        }
        private ActionResult GetPages(C_UserSearch condition, string where)
        {
            PageJsonModel<C_UserShow> page = new PageJsonModel<C_UserShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"( select c.ID ID,c.wxNo wxNo ,c.Name Name,c.Phone Phone,c.Card Card,c.C_UserTypeID C_UserTypeID,c.DatCreate DatCreate,c.DatVerify DatVerify,c.state state,c.Identifier Identifier,c.Chief Chief,t.Name LevelName from C_User as  c left join C_UserType as t on t.Lever=c.C_UserTypeID ) as C_UserShow";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
    }
}
