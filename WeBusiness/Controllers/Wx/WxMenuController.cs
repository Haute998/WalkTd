using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.WxModel;

namespace WeBusiness.Controllers
{
    public class WxMenuController : BaseController
    {
        //
        // GET: /WxMenu/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            //List<WXMenu> menus = WXMenu.GetEntitysAll();
            //if (menus == null || menus.Count <= 0)
            //{
            //    WxMenuConvert.GetPublishMenuToDB();
            //}

            //拉取微信菜单保存到本地数据库
            WxMenuConvert.GetPublishMenuToDB();
            List<WXMenu> menus = WXMenu.GetEntitysAll();
            return View(menus);
        }

        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetMenuByMenuID(int id)
        {
            WXMenu menu = WXMenu.GetEntityByID(id);

            if (menu == null)
            {
            }

            return Json(menu, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult AddMenu(WXMenu menu)
        {
            if (menu.ParentID == 0)
            {
                if (WXMenu.GetMenuCount(menu.ParentID) >= 3)
                {
                    return Content("fail|一级菜单最多只能有三个，不能再加了");
                }
            }
            if (menu.ParentID > 0)
            {
                if (WXMenu.GetMenuCount(menu.ParentID) >= 5)
                {
                    return Content("fail|二级菜单最多只能有五个，不能再加了");
                }
            }

            menu.Menukey = DAL.Generate_Second.NextBillNumber();
            do
            {
                WXMenu oldmenu = WXMenu.GetMenuByMenukey(menu.Menukey);
                if (oldmenu == null)
                {
                    break;
                }
            }
            while (true);


            int menuid = menu.InsertAndReturnIdentity();
            return Content("ok|" + menuid.ToString());
        }

        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id, string name, string value)
        {
            int rtn = WXMenu.Edit(id, name, value);
            return rtn > 0 ? Content("ok") : Content("修改失败");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult EditMedia(int menuid, string Media_id, string MediaType)
        {
            int rtn = WXMenu.EditMedia(menuid, Media_id, MediaType);
            return rtn > 0 ? Content("ok") : Content("修改失败");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ActionResult DelMenuByID(int id)
        {
            int rtn = WXMenu.DeleteAllByID(id);
            return Content(rtn > 0 ? "ok" : "删除失败");
        }
        [B_MenuRightsTag("发布", "Index")]
        public ActionResult ToPublish()
        {
            string rtn = WXMenu.ToPublish();
            if (rtn == "ok")
            {
                return Content("ok");
            }
            else
            {
                return Content("发布失败，请检查是否填写完整。详情:" + rtn);
            }
        }
    }
}
