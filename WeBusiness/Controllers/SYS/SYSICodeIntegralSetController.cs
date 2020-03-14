using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSICodeIntegralSetController : BaseController
    {
        //
        // GET: /SYSICodeIntegralSet/

        //
        // GET: /SYSICodeIntegralSet/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSICodeIntegralSetSearch condition)
        {
            string where = SYSICodeIntegralSetSearch.StrWhere(condition);
            PageJsonModel<SYSICodeIntegralSet> page = new PageJsonModel<SYSICodeIntegralSet>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSICodeIntegralSet ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(SYSICodeIntegralSet para)
        {
            if (string.IsNullOrWhiteSpace(para.CodePrefix))
            {
                return Content("前缀不能为空");
            }
            if (RepeatHelper.NoRepeat("SYSICodeIntegralSet", "CodePrefix", para.CodePrefix, para.ID) > 0)
            {
                return Content("该前缀已存在");
            }
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id)
        {
            SYSICodeIntegralSet para = SYSICodeIntegralSet.GetEntityByID(id);
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(SYSICodeIntegralSet para)
        {
            if (string.IsNullOrWhiteSpace(para.CodePrefix))
            {
                return Content("前缀不能为空");
            }
            if (RepeatHelper.NoRepeat("SYSICodeIntegralSet", "CodePrefix", para.CodePrefix, para.ID) > 0)
            {
                return Content("该前缀已存在");
            }
            int rtn = para.UpdateByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (SYSICodeIntegralSet.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("批量删除", "Index")]
        public ContentResult ToDels(int[] ids)
        {
            bool rtn = SYSICodeIntegralSet.ToDels(ids);
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
