using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSIntegralCodeAreaController : BaseController
    {
        //
        // GET: /SYSIntegralCodeArea/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        private string StrWhere(SYSIntegralCodeAreaSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (AreaName like '%{0}%')", condition.keyword);
            }
            return where;
        }

        public ActionResult GetPage(int ActivityID)
        {
            PageJsonModel<SYSIntegralCodeArea> page = new PageJsonModel<SYSIntegralCodeArea>();
            page.pageIndex =1;
            page.pageSize = 10000;
            page.strForm = " SYSIntegralCodeArea  ";
            page.strSelect = " * ";
            page.strWhere = " and ActivityID=" + ActivityID.ToString();
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult ToAdd(SYSIntegralCodeArea para)
        {
            if (string.IsNullOrWhiteSpace(para.AreaName))
            {
                return Content("区域名称不能为空");
            }

            if (RepeatHelper.NoRepeatTwoAnd("SYSIntegralCodeArea", "AreaName", para.AreaName, "ActivityID", para.ActivityID.ToString(), para.ID) > 0)
            {
                return Content("区域名称已存在");
            }
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                para.ID = rtn;
                return Json(para, JsonRequestBehavior.AllowGet);
            }
            return Json("error");
        }
        public ActionResult Edit(int id)
        {
            SYSIntegralCodeArea para = SYSIntegralCodeArea.GetEntityByID(id);
            return View(para);
        }

        public ContentResult ToEdit(SYSIntegralCodeArea para)
        {
            if (string.IsNullOrWhiteSpace(para.AreaName))
            {
                return Content("区域名称不能为空");
            }

            if (RepeatHelper.NoRepeatTwoAnd("SYSIntegralCodeArea", "AreaName", para.AreaName, "ActivityID", para.ActivityID.ToString(), para.ID) > 0)
            {
                return Content("区域名称已存在");
            }
            int rtn = para.EditByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }
        
        public ContentResult ToDel(int id)
        {
            if (SYSIntegralCodeArea.DeleteByID(id) > 0)
            {

                SYSIntegralCode.ClearAreaID(id);
                return Content("ok");
            }
            return Content("删除出错");
        }

        public ContentResult ToDels(int[] ids)
        {
            bool rtn = SYSIntegralCodeArea.ToDels(ids);
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
