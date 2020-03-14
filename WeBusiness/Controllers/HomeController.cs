using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WeBusiness.Models;
using WeModels;
using WeModels.Models;
using WeModels.ModelService.Common;

namespace WeBusiness.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string url)
        {
            try
            {
                BaseAuthorizeModel auth = BaseAuthorizeHelper.GetAuthorizeModel(this.HttpContext);
                if (string.IsNullOrWhiteSpace(auth.TempDataMsg) == false)
                {
                    TempData["Msg"] = auth.TempDataMsg;
                }
                if (auth.IsAuthorize == false)
                {
                    string userAgent = Request.UserAgent;
                    return RedirectToAction("Login", "Home", new { url = Request.Url.ToString() });
                }
                B_User user = auth.CurrentSYSUser;
                List<B_Menu> menus = B_Menu.GetShowMenus();

                if (user.UserName != WeConfig.robot)
                {
                    menus.RemoveAll(m => m.IsRobot);
                }

                List<B_Role> roles = B_Role.GetUserRoles(user.UserName);
                List<B_MenuRights> rolerignt = B_MenuRights.GetRoleUser(roles[0].ID);
                ViewData["role"] = (roles != null && roles.Count > 0) ? roles[0] : null;
                ViewData["menus"] = menus;
                ViewData["url"] = url;
                ViewData["UserRole"] = rolerignt;

                return View(user);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString());
                return View(ErrorPage.ViewName, new ErrorPage { Message = ex.ToString() });
            }
        }
        
        public ActionResult Login(string url)
        {
            ViewData["url"] = url;
            return View();
        }
        [HttpPost]
        public ContentResult Login(B_User user)
        {
            try
            {
                string rtn = string.Empty;
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    rtn = "账号不能为空！";
                    return Content(rtn);
                }
                if (user.UserName == WeConfig.robot)
                {
                    rtn = "没有此账号！";
                    return Content(rtn);
                }
                if (string.IsNullOrWhiteSpace(user.PassWord))
                {
                    rtn = "密码不能为空！";
                    return Content(rtn);
                }
                if (string.IsNullOrWhiteSpace(user.valiCode))
                {
                    return Content("验证码不能为空");
                }
                if (Session["ValidateCode"] == null)
                {
                    rtn = "验证码超时！";
                    return Content(rtn);
                }
                else
                {
                    if (!user.valiCode.Equals(Session["ValidateCode"]))
                    {
                        rtn = "验证码错误！";
                        return Content(rtn);
                    }
                }
                string error = string.Empty;
                string userName = user.UserName;
                string guidCode = DAL.MD5Helper.GetMD5UTF8(Request.UserHostAddress + "," + Guid.NewGuid().ToString());
                user.LoginLastDat = DateTime.Now;
                user.CurrentTime = user.LoginLastDat.AddSeconds(System.Web.Security.FormsAuthentication.Timeout.TotalSeconds);
                user.LoginLastIp = Request.UserHostAddress;
                user.GuidCode = guidCode;

                //业务逻辑(判断和设置)
                Func<B_User, bool, bool> func = (dbUser, usrExists) =>
                {
                    if (usrExists)
                    {
                        TempData["Msg"] = dbUser.UserName + "已下线，请重新登录！";
                    }

                    if (dbUser.IsValid == false)
                    {
                        error = "您的帐号已被禁用，请及时联系管理员！";
                        return false;
                    }
                    //快捷生成ticket
                    FormsAuthentication.SetAuthCookie(dbUser.UserName, false);
                    HttpCookie cookie = Request.Cookies.Get(BaseAuthorizeHelper.GuidCodeCookieKey);
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(BaseAuthorizeHelper.GuidCodeCookieKey);
                    }
                    cookie.Value = guidCode;
                    Response.Cookies.Add(cookie);
                    return true;
                };

                if (B_UserManager.Login(user, func))
                {
                    string url = string.Concat("/", this.ControllerContext.RouteData.Values["controller"].ToString(),
                        "/", this.ControllerContext.RouteData.Values["action"].ToString());

                    SYSLog.add("电脑端后台用户登录", "后台用户" + user.Name + "(" + user.UserName + ")登录，ip为" + Request.UserHostAddress, "/Home/Login", "登录", "电脑端后台");


                    return Content("ok");
                }
                rtn = "账号或密码错误";
                if (!string.IsNullOrWhiteSpace(error))
                {
                    rtn = error;
                }
                return Content(rtn);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Login_error");
                return Content("连接数据库出错");
            }

        }
        public ActionResult VIP(string url)
        {
            ViewData["url"] = url;
            return View();
        }
        [HttpPost]
        public ContentResult VIPLogin(B_User user)
        {
            try
            {
                string rtn = string.Empty;
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    rtn = "账号不能为空！";
                    return Content(rtn);
                }
                if (string.IsNullOrWhiteSpace(user.PassWord))
                {
                    rtn = "密码不能为空！";
                    return Content(rtn);
                }
                if (string.IsNullOrWhiteSpace(user.valiCode))
                {
                    return Content("验证码不能为空");
                }
                if (Session["ValidateCode"] == null)
                {
                    rtn = "验证码超时！";
                    return Content(rtn);
                }
                else
                {
                    if (!user.valiCode.Equals(Session["ValidateCode"]))
                    {
                        rtn = "验证码错误！";
                        return Content(rtn);
                    }
                }
                string error = string.Empty;
                string userName = user.UserName;
                string guidCode = DAL.MD5Helper.GetMD5UTF8(Request.UserHostAddress + "," + Guid.NewGuid().ToString());
                user.LoginLastDat = DateTime.Now;
                user.CurrentTime = user.LoginLastDat.AddSeconds(System.Web.Security.FormsAuthentication.Timeout.TotalSeconds);
                user.LoginLastIp = Request.UserHostAddress;
                user.GuidCode = guidCode;

                //业务逻辑(判断和设置)
                Func<B_User, bool, bool> func = (dbUser, usrExists) =>
                {
                    if (usrExists)
                    {
                        TempData["Msg"] = dbUser.UserName + "已下线，请重新登录！";
                    }

                    if (dbUser.IsValid == false)
                    {
                        error = "您的帐号已被禁用，请及时联系管理员！";
                        return false;
                    }
                    //快捷生成ticket
                    FormsAuthentication.SetAuthCookie(dbUser.UserName, false);
                    HttpCookie cookie = Request.Cookies.Get(BaseAuthorizeHelper.GuidCodeCookieKey);
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(BaseAuthorizeHelper.GuidCodeCookieKey);
                    }
                    cookie.Value = guidCode;
                    Response.Cookies.Add(cookie);
                    return true;
                };

                if (B_UserManager.Login(user, func))
                {
                    string url = string.Concat("/", this.ControllerContext.RouteData.Values["controller"].ToString(),
                        "/", this.ControllerContext.RouteData.Values["action"].ToString());
                    return Content("ok");
                }
                rtn = "账号或密码错误";
                if (!string.IsNullOrWhiteSpace(error))
                {
                    rtn = error;
                }
                return Content(rtn);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Login_error");
                return Content("连接数据库出错");
            }
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult ValidCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        public ActionResult LogOff()
        {

            HttpCookie cookie = Request.Cookies.Get(BaseAuthorizeHelper.GuidCodeCookieKey);
            //退出写入日志
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                B_User usr = B_UserManager.GetB_User(m => m.UserName == userName);
                if (usr != null)
                {
                    //如果是当前用户
                    if (cookie != null && cookie.Value == usr.GuidCode)
                    {
                        B_UserManager.LogOff(userName);
                    }
                }

                B_MenuRights.RemoveHashMenuRights(userName);
                FormsAuthentication.SignOut();
            }

            if (cookie != null)
            {
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Login", "Home");
        }

    }
}
