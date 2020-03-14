using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SuggestionsController : BaseController
    {
        //
        // GET: /Advices/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetNoAdvicePage(C_UserAdvice condition)
        {
            string where = string.Empty;
            where += "and State='未审核'";
            if (!string.IsNullOrWhiteSpace(Common.Filter(condition.keyword)))
            {
                where += string.Format(@" and (Name like '%{0}%' or Phone like '%{0}%') ", Common.Filter(condition.keyword));
            }
            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"C_UserAdvice";
            page.strSelect = " * ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {   
                page.strOrder = "DatCreate asc";
            }

            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("查看")]
        public ActionResult VerifyIndex()
        {
            return View();
        }
        public ActionResult GetAdvicePage(C_UserAdvice condition)
        {
            string where = string.Empty;
            where += "and State='已审核'";
            if (!string.IsNullOrWhiteSpace(Common.Filter(condition.keyword)))
            {
                where += string.Format(@" and (Name like '%{0}%' or Phone like '%{0}%') ", Common.Filter(condition.keyword));
            }

            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"C_UserAdvice";
            page.strSelect = " * ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "DatCreate asc";
            }

            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("审核", "Index")]
        public ContentResult GetVerify(int ID)
        {
            C_UserAdvice advice = C_UserAdvice.GetEntityByID(ID);
            advice.UserName = CurrentUser.UserName;
            advice.B_Name = CurrentUser.Name;
            advice.DatVerify = DateTime.Now;
            advice.State = "已审核";
            int iRet = advice.UpdateByID();

            if (iRet > 0)
            {
                SYSLog.add("审核了客户名为[" + advice.Name + "]的投诉建议", "后台用户" + CurrentUser.UserName + "(" + CurrentUser.Name + ")", CurrentURL, "审核投诉建议", "电脑端后台");
                return Content("ok");
            }
            else
            {
                return Content("审核失败！");
            }
        }
    }
}
