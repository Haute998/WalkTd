using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models.C_UserModel;

namespace WeBusiness.Controllers
{
    public class C_UserPromoteController : BaseController
    {
        //
        // GET: /C_UserPromote/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUserPage(C_UserSearch condition)
        {
            string where = string.Empty;
            if (string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and UserName like '%" + condition.keyword + "%'";
            }
            return GetPages(condition, where);
        }
        private ActionResult GetPages(C_UserSearch condition, string where)
        {
            PageJsonModel<C_UserUpGrade> page = new PageJsonModel<C_UserUpGrade>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"(SELECT [C_UserUpGrade].*,t1.Name OldUserTypeName,t2.Name NewUserTypeName,C_User.Name C_Name FROM [C_UserUpGrade] 
                                      left join C_UserType t1 on [C_UserUpGrade].OldUserTypeID=t1.Lever
                                      left join C_UserType t2 on [C_UserUpGrade].NewUserTypeID=t2.Lever 
                                      left join C_User on [C_UserUpGrade].UserName=C_User.UserName
                                      where ParentUser='' and AuditStat='未审核') as Show";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        [B_MenuRightsTag("审核", "Index")]
        public ActionResult GetPromote(int ID)
        {

            int[] IDS = { ID };
            string msg = C_UserUpGradeVM.AuditByIDs(IDS);
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "ok";
            }
            return Content(msg);
        }
        [B_MenuRightsTag("取消", "Index")]
        public ActionResult GetNoPromote(int ID)
        {
            int[] IDS = { ID };
            string msg = "出错了！！";
            if (C_UserUpGradeVM.GetNoPromote(IDS))
            {
                msg = "ok";
            }
            return Content(msg);
        }

    }
}
