using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class B_MenuController : BaseController
    {
        //
        // GET: /B_Menu/
        [B_MenuRightsTag("查看")]
        public ActionResult MainMenus()
        {
            List<B_Menu> menus = B_Menu.GetEntitysAll().OrderBy(s => s.Sort).ToList();
            menus = menus.FindAll(m => m.ParentID == 0);
            return View(menus);
        }
        [B_MenuRightsTag("添加", "MainMenus")]
        public ActionResult MainMenusAdd()
        {
            return View();
        }
        [B_MenuRightsTag("修改", "MainMenus")]
        public ActionResult MainMenusEdit(int id)
        {
            B_Menu menu = B_Menu.GetEntityByID(id);
            return View(menu);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult Menus()
        {
            List<B_Menu> menus = B_Menu.GetEntitysAll();
            return View(menus);
        }
        [B_MenuRightsTag("添加", "Menus")]
        public ActionResult MenusAdd()
        {
            return View();
        }
        [B_MenuRightsTag("修改", "Menus")]
        public ActionResult MenusEdit(int id)
        {
            B_Menu menu = B_Menu.GetEntityByID(id);
            return View(menu);
        }
        [B_MenuRightsTag("添加", "Menus")]
        [ValidateInput(false)]
        public ContentResult MenusToAdd(B_Menu menu)
        {   
            menu.IsShow = Request["IsShow"] != null && Request["IsShow"].ToString() == "on" ? true : false;
            menu.IsValid = Request["IsValid"] != null && Request["IsValid"].ToString() == "on" ? true : false;
            menu.IsRobot = Request["IsRobot"] != null && Request["IsRobot"].ToString() == "on" ? false : true;

            if (string.IsNullOrWhiteSpace(menu.MenuName))
            {
                return Content("名称不能为空");
            }
            if (RepeatHelper.NoRepeat("B_Menu", "MenuName", menu.MenuName, menu.ID) > 0)
            {
                return Content("菜单名已存在");
            }
            if (!string.IsNullOrWhiteSpace(menu.MenuUrl) && RepeatHelper.NoRepeat("B_Menu", "MenuUrl", menu.MenuUrl, menu.ID) > 0)
            {
                return Content("菜单链接已存在");
            }

            int rtn = menu.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                bool hasMainMenuTag = false;
                bool hasError = false;
                List<B_MenuRights> muRights = null;
                if (string.IsNullOrWhiteSpace(menu.MenuUrl) == false)
                {
                    menu.MenuUrl = menu.MenuUrl.Trim().TrimEnd('/');
                    try
                    {
                        string[] arrUrl = menu.MenuUrl.TrimStart('/').Split('/');
                        Type type = Type.GetType("WeBusiness.Controllers." + arrUrl[0] + "Controller");
                        muRights = BaseAuthorizeHelper.GetBOMenuRightsByControllerType(type, arrUrl[1], out hasMainMenuTag);
                    }
                    catch (Exception ex)
                    {
                        DAL.Log.Instance.Write(ex.ToString(), "BOMenu_Add");
                        hasError = true;
                    }
                }

                if (hasMainMenuTag && muRights != null)
                {
                    B_MenuRights.RelevanceList(rtn, muRights);
                }
                if (hasError)
                {
                    return Content("添加菜单成功，但菜单没有添加权限！");
                }


                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Menus")]
        [ValidateInput(false)]
        public ContentResult MenusToEdit(B_Menu menu)
        {
            menu.IsShow = Request["IsShow"] != null && Request["IsShow"].ToString() == "on" ? true : false;
            menu.IsValid = Request["IsValid"] != null && Request["IsValid"].ToString() == "on" ? true : false;
            menu.IsRobot = Request["IsRobot"] != null && Request["IsRobot"].ToString() == "on" ? false : true;

            if (string.IsNullOrWhiteSpace(menu.MenuName))
            {
                return Content("名称不能为空");
            }
            if (RepeatHelper.NoRepeat("B_Menu", "MenuName", menu.MenuName, menu.ID) > 0)
            {
                return Content("菜单名已存在");
            }
            if (!string.IsNullOrWhiteSpace(menu.MenuUrl) && RepeatHelper.NoRepeat("B_Menu", "MenuUrl", menu.MenuUrl, menu.ID) > 0)
            {
                return Content("菜单链接已存在");
            }
            int rtn = menu.UpdateByID();
            if (rtn > 0)
            {
                bool hasMainMenuTag = false;
                bool hasError = false;
                List<B_MenuRights> muRights = null;
                if (string.IsNullOrWhiteSpace(menu.MenuUrl) == false)
                {
                    menu.MenuUrl = menu.MenuUrl.Trim().TrimEnd('/');
                    try
                    {
                        string tmpUrl = menu.MenuUrl.TrimStart('/').Split('?')[0];
                        string[] arrUrl = tmpUrl.Split('/');
                        Type type = Type.GetType("WeBusiness.Controllers." + arrUrl[0] + "Controller");
                        muRights = BaseAuthorizeHelper.GetBOMenuRightsByControllerType(type, arrUrl[1], out hasMainMenuTag);
                    }
                    catch (Exception ex)
                    {
                        DAL.Log.Instance.Write(ex.ToString(), "BOMenu_Update");
                        hasError = true;
                    }
                }
                if (hasMainMenuTag && muRights != null)
                {
                    B_MenuRights.RelevanceList(menu.ID, muRights);
                }
                if (hasError)
                {
                    return Content("修改菜单成功，但菜单没有添加权限！");
                }
                B_MenuRights.ClearHashMenuRights();
                return Content("ok");
            }
            return Content("保存出错");
        }

        public ContentResult MenusToUnValid(int id)
        {
            B_Menu menu = B_Menu.GetEntityByID(id);
            menu.IsValid = false;
            menu.IsShow = false;
            menu.IsRobot = true;
            if (menu.UpdateByID() > 0)
            {
                return Content("ok");
            }
            return Content("停用出错");
        }

        public ContentResult IsModuleShow(int id)
        {
            B_Menu menu = B_Menu.GetEntityByID(id);
            menu.IsValid = true;
            menu.IsShow=true;
            menu.IsRobot = false;
            if (menu.UpdateByID() > 0)
            {
                return Content("ok");
            }
            return Content("启用出错");
        }

        [B_MenuRightsTag("删除", "Menus")]
        public ContentResult MenusToDel(int id)
        {
            if (B_Menu.DeleteAll(id))
            {
                B_MenuRights.ClearHashMenuRights();

                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("一键更新权限", "Menus")]
        public ContentResult UpdateAllRights()
        {
            try
            {
                List<B_Menu> menus = B_Menu.GetEntitysAll();
                string ErrorStr = "";
                foreach (B_Menu menu in menus)
                {
                    if (menu.ParentID == 0)
                    {
                        continue;
                    }
                    bool hasMainMenuTag = false;
                    List<B_MenuRights> muRights = null;
                    if (string.IsNullOrWhiteSpace(menu.MenuUrl) == false)
                    {
                        menu.MenuUrl = menu.MenuUrl.Trim().TrimEnd('/');
                        try
                        {
                            string tmpUrl = menu.MenuUrl.TrimStart('/').Split('?')[0];
                            string[] arrUrl = tmpUrl.Split('/');
                            Type type = Type.GetType("WeBusiness.Controllers." + arrUrl[0] + "Controller");
                            muRights = BaseAuthorizeHelper.GetBOMenuRightsByControllerType(type, arrUrl[1], out hasMainMenuTag);
                        }
                        catch (Exception ex)
                        {
                            DAL.Log.Instance.Write(ex.ToString(), "BOMenu_Update");
                            ErrorStr += "【" + menu.MenuName + "】";
                        }
                    }
                    if (hasMainMenuTag && muRights != null)
                    {
                        B_MenuRights.RelevanceList(menu.ID, muRights);
                    }
                    B_MenuRights.ClearHashMenuRights();

                }
                if (!string.IsNullOrWhiteSpace(ErrorStr))
                {
                    return Content("更新失败的菜单：" + ErrorStr);
                }
                return Content("ok");
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "B_Menu_UpdateAllRights_error");
                return Content("更新失败");
            }
        }

    }
}
