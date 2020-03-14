using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class problemController : Controller
    {
        //
        // GET: /problem/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult add()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult Index1()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult add1()
        {
            return View();
        }
        public ActionResult toAdd(B_problem para)
        {   
            if (string.IsNullOrWhiteSpace(para.problem))
            {
                return Content("故障问题不能为空");
            }
            para.lag = "c";
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        public ActionResult toAdd1(B_problem para)
        {
            if (string.IsNullOrWhiteSpace(para.problem))
            {
                return Content("故障问题不能为空");
            }
            para.lag = "e";
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        private string StrWhere(problem condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (problem like '%{0}%' )", condition.keyword);
            }
            return where;
        }
        public ActionResult GetPage(problem condition)
        {
            string where = StrWhere(condition);
            where += " and lag='c'";
            return GetPages(condition, where);
        }
        public ActionResult GetPage1(problem condition)
        {
            string where = StrWhere(condition);
            where += " and lag='e'";
            return GetPages(condition, where);
        }
        private ActionResult GetPages(problem condition, string where)
        {
            PageJsonModel<B_problem> page = new PageJsonModel<B_problem>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @" B_problem";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        [B_MenuRightsTag("彻底删除", "Index")]
        public ActionResult Delete(int ID)
        {
            int rtn = B_problem.DeleteByID(ID);
            return Content(rtn > 0 ? "ok" : "删除出错了！！");
        }
    }
}
