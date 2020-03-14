using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class B_UserController : BaseController
    {
        //
        // GET: /B_User/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<B_User> roles = B_User.GetAllB_User();
            if (roles != null && roles.Count > 0 && CurrentUser.UserName != WeConfig.robot)
            {
                roles.RemoveAll(m => m.UserName == WeConfig.robot);
            }
            return View(roles);
        }

        [B_MenuRightsTag("添加", "Index")]
        public ActionResult B_UserAdd()
        {
            List<Supplier> supplierList = Supplier.GetEntitysAll();
            ViewBag.SupplierList = supplierList;
            return View();
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult B_UserEdit(int id)
        {
            B_User user = B_User.GetEntityByID(id);
            List<B_Role> roles = B_Role.GetUserRoles(user.UserName);
            B_Role role = (roles != null && roles.Count > 0) ? roles[0] : null;
            user.RoleID = role == null ? 0 : role.ID;
            return View(user);
        }

        [B_MenuRightsTag("添加", "Index")]
        public ContentResult B_UserToAdd(B_User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return Content("用户名不能为空");
            }
            string reg = "^[\u4E00-\u9FA5A-Za-z0-9_]+$";
            Regex regex = new Regex(reg, RegexOptions.None);
            if (regex.IsMatch(user.UserName) == false)
            {
                return Content("用户名只能为中文、字母或数字");
            }

            if (B_User.GetUserCountByUserName(user.UserName) > 0)
            {
                return Content("该用户名已存在，请换一个吧");
            }

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return Content("姓名不能为空");
            }

            bool IsValid = false;
            if (Request["IsValid"] != null)
            {
                if (Request["IsValid"].ToString() == "on")
                {
                    IsValid = true;
                }
            }

            user.IsValid = IsValid;
            user.DatAdd = DateTime.Now;
            int rtn = user.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                if (user.RoleID > 0)
                {
                    if (B_Role.EditUserRole(user.UserName, user.RoleID))
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("角色保存出错");
                    }

                }
                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult B_UserToEdit(B_User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return Content("姓名不能为空");
            }
            if (user.UserName == user.PassWord)
            {
                return Content("账号密码不能一致");
            }
            if (user.PassWord.Length < 6)
            {
                return Content("密码必须在6位以上");
            }
            bool IsValid = false;
            if (Request["IsValid"] != null)
            {
                string IsStr = Request["IsValid"].ToString();
                if (IsStr == "on")
                {
                    IsValid = true;
                }
            }
            user.IsValid = IsValid;
            B_User oldUser = B_User.GetEntityByID(user.ID);
            int rtn = user.UserEditByID();
            if (rtn > 0)
            {
                if (user.RoleID > 0)
                {
                    if (B_Role.EditUserRole(oldUser.UserName, user.RoleID))
                    {
                        return Content("ok");
                    }
                    else
                    {
                        return Content("角色保存出错");
                    }

                }
                return Content("ok");
            }
            return Content("保存出错");
        }

        /// <summary>
        /// 后台用户详情
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [B_MenuRightsTag("客户详情", "Index")]
        public ActionResult B_UserDetail(string username)
        {
            B_User user = B_User.GetB_UserByUserName(username);
            if (user == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "用户不存在" });
            }
            return View(user);
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult B_UserToDel(int id)
        {
            B_User user = B_User.GetEntityByID(id);
            if (CurrentUser.UserName == user.UserName)
            {
                return Content("不能删除当前登录用户");
            }
            int rtn = B_User.DeleteByID(id);
            if (rtn > 0)
            {
                B_Role.DelUserRoleByUserName(user.UserName);
                return Content("ok");
            }
            return Content("删除出错");
        }


    }
}
