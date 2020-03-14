using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class B_RoleController : BaseController
    {
        //
        // GET: /B_Role/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<B_Role> roles = B_Role.GetEntitysAll();
            return View(roles);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult B_RoleAdd()
        {
            return View();
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult B_RoleEdit(int id)
        {
            B_Role role = B_Role.GetEntityByID(id);
            return View(role);
        }
        [B_MenuRightsTag("分配权限", "Index")]
        public ActionResult B_RoleRight(int id)
        {
            B_Role dbBORoles = B_Role.GetEntityByID(id);
            if (dbBORoles == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "分配角色权限失败，角色不存在或已删除！" });
            }

            List<B_Menu> Menus = B_Menu.GetValidMenus(true);

            if (CurrentUser.UserName != WeConfig.robot)
            {
                Menus.RemoveAll(m => m.IsRobot);

                foreach (var item in Menus) 
                {
                    if (item.SubMenuList != null)
                    {
                        item.SubMenuList.RemoveAll(m => m.IsRobot);
                    }
                }
            }
            ViewBag.Menus = Menus;
            ViewBag.MenuRights = B_MenuRights.GetEntitysDictionary();

            List<B_RoleRights> roRightsList = B_RoleRights.GetEntitysByRoleID(id);
            HashSet<int> setRights = new HashSet<int>();
            foreach (B_RoleRights roRights in roRightsList)
            {
                setRights.Add(roRights.RightID);
            }
            ViewBag.HashRights = setRights;
            ViewBag.dbBORoles = dbBORoles;
            return View();
        }

        [B_MenuRightsTag("分配桌面权限", "Index")]
        public ActionResult B_RoleMin(int id)
        {
            B_Role dbBORoles = B_Role.GetEntityByID(id);
            if (dbBORoles == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "分配角色权限失败，角色不存在或已删除！" });
            }

            List<B_UserDesktopAuth> roRightsList = B_UserDesktopAuth.GetEntitysMainRoleID(id, CurrentUser.UserName);
            HashSet<int> setRights = new HashSet<int>();
            foreach (B_UserDesktopAuth roRights in roRightsList)
            {
                setRights.Add(roRights.MainID);
            }

            ViewBag.MenuRights = B_UserDesktopAuth.GetRolesDesktopAuth(id);

            ViewBag.HashRights = setRights;
            ViewBag.dbBORoles = dbBORoles;
            ViewData["UserName"] = CurrentUser.UserName;
            ViewData["C_UserName"] = WeConfig.robot;
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult B_RoleToAdd(B_Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                return Content("角色名不能为空");
            }
            if (RepeatHelper.NoRepeat("B_Role", "RoleName", role.RoleName, role.ID) > 0)
            {
                return Content("角色名已存在");
            }
            int rtn = role.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult B_RoleToEdit(B_Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                return Content("角色名不能为空");
            }
            if (RepeatHelper.NoRepeat("B_Role", "RoleName", role.RoleName, role.ID) > 0)
            {
                return Content("角色名已存在");
            }
            int rtn = role.UpdateByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }
        [B_MenuRightsTag("删除", "Index")]
        public ContentResult B_RoleToDel(int id)
        {
            int rtn = B_Role.DeleteByID(id);
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("分配权限", "Index")]
        [HttpPost]
        public ContentResult RightsToConfig(int id, string[] idlist)
        {

            if (idlist == null || idlist.Length <= 0)
            {
                return Content("分配角色权限失败，请选择要分配的权限！");
            }
            List<int> rList = new List<int>();
            foreach (string sid in idlist)
            {
                int tid = 0;
                if (int.TryParse(sid, out tid))
                {
                    rList.Add(tid);
                }
            }
            if (B_Role.RelevanceRightsList(id, rList))
            {
                B_MenuRights.ClearHashMenuRights();
            }
            else
            {
                return Content("分配角色权限失败！");
            }
            return Content("ok");

        }

        public ContentResult ToSetRolesDesktop(int RolesId, string DesktopIdSet)
        {
            B_Role.SetDesktopAuth(DesktopIdSet, RolesId);
            return Content("ok");
        }
    }
}
