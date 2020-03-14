using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class MinOrderPriceController : BaseController
    {
        //
        // GET: /MinOrderPrice/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<C_UserType> usertype = C_UserType.GetEntitysAll().OrderByDescending(m=>m.Lever).ToList();
            return View(usertype);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult edit(int id,decimal price) 
        {
            C_UserType type = C_UserType.GetEntityByID(id);

            if (type == null) 
            {
                return Content("该客户级别不存在");
            }

            int rtn= C_UserType.EditMinOrderPriceByID(id,price);

            if (rtn > 0) 
            {
                SYSLog.add("修改[" + type.Name + "]订单最小金额从[" + type.MinOrderPrice + "]修改为[" + price + "]", "后台用户" + CurrentUser.UserName + "(" + CurrentUser.Name + ")", CurrentURL,"订单最小金额", "电脑端后台");
            }

            return Content(rtn>0?"ok":"修改出错啦");
        }

    }
}
