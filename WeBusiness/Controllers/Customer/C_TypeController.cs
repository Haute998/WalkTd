using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class C_TypeController : BaseController
    {
        //
        // GET: /C_Type/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<C_UserType> Types = C_UserType.GetEntityList("");
            return View(Types);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult TypeAddIndex()
        {
            return View();
        }

        [B_MenuRightsTag("添加", "TypeAddIndex")]
        public ActionResult GetTypeAdd(C_UserType Types)
        {
            if (string.IsNullOrWhiteSpace(Types.Name))
            {
                return Content("类型名称不能为空");
            }
            if (Types.Lever <= 0)
            {
                return Content("类型级别必须大于0");
            }
            if (C_UserType.GetEntittyInt(Types.Lever))
            {
                return Content("已经有这个级别请重新输入！！");
            }
            int rtn = Types.InsertAndReturnIdentity();
            return Content(rtn > 0 ? "ok" : "添加出错了！！");
        }

         [B_MenuRightsTag("修改", "Index")]
        public ActionResult TypeEditIndex(int ID)
        {
            C_UserType Types = C_UserType.GetEntityByID(ID);
            return View(Types);
        }

        [B_MenuRightsTag("修改", "TypeEditIndex")]
        public ActionResult GetTypeEdit(C_UserType Types)
        {
            if (string.IsNullOrWhiteSpace(Types.Name))
            {
                return Content("类型名称不能为空");
            }
            C_UserType oldType = C_UserType.GetEntityByID(Types.ID);
            oldType.Name = Types.Name;
            oldType.TypeCode = Types.TypeCode;
            int rtn = oldType.UpdateByID();

            if (rtn > 0) 
            {
                SYSLog.add("将代理类型[" + oldType.Name + "]从[" + oldType.Name + "]修改为[" + Types.Name + "]", "电脑端后台用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")", CurrentURL, "修改代理类型", "电脑后台");
            }

            return Content(rtn > 0 ? "ok" : "修改出错了！！");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ActionResult GetTypeDel(int ID)
        {

            int rtn = C_UserType.DeleteByID(ID);
            return Content(rtn > 0 ? "ok" : "删除出错了！！");
        }
    }
}
