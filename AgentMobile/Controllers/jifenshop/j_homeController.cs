using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class j_homeController : Controller
    {
        //
        // GET: /j_home/

        public ActionResult login(string username="")
        {
            ViewData["username"] = username;
            return View();
        }

        public ActionResult tologin(C_Consumer c) 
        {
            c.UserName = c.UserName.Trim();
            c.Pwd = c.Pwd.Trim();
            if (string.IsNullOrWhiteSpace(c.UserName))
            {
                return Content("账号不能为空");
            }
            if (string.IsNullOrWhiteSpace(c.Pwd))
            {
                return Content("密码不能为空");
            }
            C_Consumer user = C_Consumer.GetEntityByUserNamepwd(c.UserName,c.Pwd);
            if (user == null) 
            {
                return Content("账号或密码不正确");
            }
            if (user.Type != "促销员") 
            {
                return Content("只限促销员登录");
            }
            Session["C_Consumer_UserName"] = user.UserName;

            return Content("ok");
        }



        public ActionResult reg(string type="") 
        {
            ViewData["type"] = type;
            return View();
        }


        public ActionResult toreg(C_Consumer c) 
        {

            c.UserName = c.UserName.Trim();
            c.Pwd = c.Pwd.Trim();
            c.Mobile = c.UserName;
            c.Type = "促销员";
            c.Stat = "未审核";


            string addressStr = Request["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    c.Province = addre[i];
                }
                else if (i == 1)
                {
                    c.City = addre[i];
                }
                else if (i == 2)
                {
                    c.Area = addre[i];
                }
            }




            if (string.IsNullOrWhiteSpace(c.UserName)) 
            {
                return Content("手机号不能为空");
            }
            

            if (string.IsNullOrWhiteSpace(c.Pwd)) 
            {
                return Content("密码不能为空");
            }
            if (c.Pwd != c.pwdconfirm) 
            {
                return Content("两次输入密码不一致");
            }
            C_Consumer oldc = C_Consumer.GetEntityByUserName(c.UserName);

            if (oldc != null) 
            {
                return Content("该手机号已注册");
            }

            c.DatReg = DateTime.Now;
            c.ID=c.InsertAndReturnIdentity();
            if (c.ID>0) 
            {
                return Content("ok");
            }
            return Content("注册失败");




        }

    }
}
