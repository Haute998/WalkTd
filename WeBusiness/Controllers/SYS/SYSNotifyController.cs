using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSNotifyController : BaseController
    {
        //
        // GET: /SYSNotify/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(SYSNotifySearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (MsgType like '%{0}%' or MsgName like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSNotifySearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSNotify> page = new PageJsonModel<SYSNotify>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSNotify ";
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
        public ContentResult ToAdd(SYSNotify para)
        {
            if (string.IsNullOrWhiteSpace(para.MsgType))
            {
                return Content("消息类别不能为空");
            }
            if (string.IsNullOrWhiteSpace(para.MsgCode))
            {
                return Content("消息代号不能为空");
            }
            if (RepeatHelper.NoRepeat("SYSNotify", "MsgCode", para.MsgCode, para.ID) > 0)
            {
                return Content("消息代号已存在");
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
            SYSNotify para = SYSNotify.GetEntityByID(id);
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(SYSNotify para)
        {
            if (string.IsNullOrWhiteSpace(para.MsgType))
            {
                return Content("消息类别不能为空");
            }
            if (string.IsNullOrWhiteSpace(para.MsgCode))
            {
                return Content("消息代号不能为空");
            }
            if (RepeatHelper.NoRepeat("SYSNotify", "MsgCode", para.MsgCode, para.ID) > 0)
            {
                return Content("消息代号已存在");
            }
            int rtn = para.EditByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (SYSNotify.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("批量删除", "Index")]
        public ContentResult ToDels(int[] ids)
        {
            bool rtn = SYSNotify.ToDels(ids);
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
