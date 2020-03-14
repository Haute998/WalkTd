using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels.Models.C_UserModel;
using WeModels;

namespace AgentMobile.Controllers
{
    public class WarrantController : BaseController
    {
        //
        // GET: /Warrant/

        public ActionResult MyIndex(string ID)
        {
            if (string.IsNullOrWhiteSpace(ID))
            {
                ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            }
            else
            {
                ViewData["user"] = C_UserVM.GetVMByID(Convert.ToInt32(ID));
            }
            return View(CurrentUser);
        }

        public ActionResult AgentIndex(string ID)
        {
            ViewData["user"] = C_UserVM.GetVMByUserName(ID);
            return View(CurrentUser);
        }


        public ActionResult EditCustomer(string UserName)
        {
            var obj = C_UserVM.GetVMByUserName(UserName);
                
            return View(obj);
        }


        public ActionResult EditTo(FormCollection c)
        {
            C_User user = C_User.GetC_UserByUserName(c["UserName"].ToString());
            if (user==null)
            {
                return Content("没有找到用户信息");
            }
            string addressStr = c["PCAs"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    user.Province = addre[i];
                }
                else if (i == 1)
                {
                    user.City = addre[i];
                }
                else if (i == 2)
                {
                    user.Area = addre[i];
                }
            }

            user.Phone = c["Phone"];
            if (user.Phone.Length != 11)
            {
                return Content("手机号有误");
            }
            if (C_User.GetPhoneCnt(user.Phone,user.UserName) > 0)
            {
                return Content("该手机号已存在");
            }
            user.DatPwdChange = DateTime.Now;
            user.UpdateTime = CommonFunc.GetNowTimestamp();
            user.Name = c["Name"];
            user.WxQRCode = c["WxQRCode"];
            user.PassWord = user.Phone.Substring(user.Phone.Length - 6, 6);
            user.wxNo = c["wxNo"];

            if (user.UpdateByID() > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("保存出错");
            }
        }
    }
}
