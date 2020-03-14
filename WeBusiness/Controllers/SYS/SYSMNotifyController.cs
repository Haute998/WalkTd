using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSMNotifyController : BaseController
    {
        //
        // GET: /SYSMNotify/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<SYSMNotify> notify = SYSMNotify.GetBySort();
            return View(notify);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(SYSMNotify notify)
        {
            if (string.IsNullOrWhiteSpace(notify.Title))
            {
                return Content("标题不能为空");
            }
            if (RepeatHelper.NoRepeat("SYSMNotify", "Title", notify.Title, notify.ID) > 0)
            {
                return Content("标题已存在");
            }
            notify.OperCreate = CurrentUser.UserName;
            notify.DatCreate = DateTime.Now;
            notify.DatEdit = DateTime.Now;
            notify.OperEdit = CurrentUser.UserName;
            int rtn = notify.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id)
        {
            SYSMNotify notify = SYSMNotify.GetEntityByID(id);
            return View(notify);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(SYSMNotify notify)
        {
            notify.OperEdit = CurrentUser.UserName;
            int rtn = notify.EditByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (SYSMNotify.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }

        [B_MenuRightsTag("选择文章", "Index")]
        public ActionResult SetMArticle()
        {
            return View();
        }
        //private string StrWhere(SYSMArticleSearch condition)
        //{
        //    string where = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(condition.Title))
        //    {
        //        where += string.Format(" and Title ='{0}'", condition.Title);
        //    }
        //    return where;
        //}

        //[B_MenuRightsTag("选择文章", "Index")]
        //public ActionResult GetPage(SYSMArticleSearch condition)
        //{
        //    string where = StrWhere(condition);
        //    PageJsonModel<SYSMArticle> page = new PageJsonModel<SYSMArticle>();
        //    page.pageIndex = condition.pageIndex;
        //    page.pageSize = condition.pageSize;
        //    page.strForm = " SYSMArticle ";
        //    page.strSelect = " * ";
        //    page.strWhere = where;
        //    page.strOrder = "ID desc";
        //    page.LoadList();

        //    if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
        //    {
        //        return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

    }
}
