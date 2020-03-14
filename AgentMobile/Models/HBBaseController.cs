using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    public class HBBaseController : Controller
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        protected string UserName { get; private set; }
        /// <summary>
        /// 当前用户
        /// </summary>
        protected C_WxUser CurrentUser { get; private set; }
        protected string C_UserTypeID { get; private set; }
        /// <summary>
        /// 是否是微信中打开
        /// </summary>
        protected bool IsWx { get; private set; }

        private string _ActionName;
        /// <summary>
        /// 动作方法名
        /// </summary>
        protected string ActionName
        {
            get
            {
                return _ActionName;
            }
        }


        private string _ControllerName;
        /// <summary>
        /// 控制器名称
        /// </summary>
        protected string ControllerName
        {
            get
            {
                return _ControllerName;
            }
        }

        /// <summary>
        /// 当前URL
        /// </summary>
        protected string CurrentURL
        {
            get
            {
                return string.Concat("/", _ControllerName, "/", _ActionName);
            }
        }
        /// <summary>
        /// 当前完整url
        /// </summary>
        protected string FullUrl
        {
            get
            {
                return HttpUtility.UrlEncode(Request.Url.ToString());
            }
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                return;
            }

            var ex = filterContext.Exception;
            if (ex == null)
            {
                return;
            }
            string title = ex.GetType().ToString();
            string message = ex.Message;
            string detail = ex.StackTrace;
            try
            {
                new SYSExceptionCode
                {
                    Dat = DateTime.Now,
                    ExType = title,
                    Message = message,
                    ExContent = detail,
                    ExURL = string.Concat("/", ControllerName, "/", ActionName),
                    Oper = User.Identity.Name
                }.InsertAndReturnIdentity();

                string url = string.Concat(ControllerName, "_", ActionName);
                DAL.Log.Instance.Write(string.Concat(title, ":", message, detail), "err_" + url);
            }
            catch (Exception tex)
            {
                message = tex.ToString();
            }


            filterContext.ExceptionHandled = true;
            //异常消息处理
            filterContext.Result = GetErrorResult(message, title);
        }
        /// <summary>
        /// 错误跳转
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type">login：登陆/defriend：拉黑</param>
        /// <returns></returns>
        private ActionResult GetErrorResult(string message, string title = null, string type = "")
        {
            if (type == "login")
            {
                string userAgent = Request.UserAgent;

                //if (!string.IsNullOrWhiteSpace(VConfig.WxDebug))
                //{
                //    //调试模式
                //    Common.SetCookie("WxUserName", VConfig.WxDebug);
                //}
                //else 
                    if (userAgent.ToLower().Contains("micromessenger"))
                {
                    return RedirectToAction("WXLogin", "Home", new { url = FullUrl });
                }
                else
                {
                    //非微信打开的处理
                    DAL.Log.Instance.Write(userAgent.ToLower(), "logininfo");
                    return RedirectToAction("Index", "Home");
                }
            }

            if (type == "defriend")
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "您已被拉黑，无法访问该页" });
            }

            //跳转到异常页面
            if (title != null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Title = title, Message = message });
            }
            return View(ErrorPage.ViewName, new ErrorPage { Message = message });
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            _ActionName = filterContext.ActionDescriptor.ActionName;
            _ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            string _ControllerType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.ToString();
            if (_ControllerType != "AgentMobile.Controllers." + _ControllerName + "Controller")
            {
                filterContext.Result = GetErrorResult("若继承HBBaseController则该控制器只能使用默认后缀Controller和命名空间只能使用AgentMobile.Controllers", null);
                return;
            }
            string UserNameCookie = Common.GetCookieValue("WxUserName");
            UserName = UserNameCookie;
            if (string.IsNullOrWhiteSpace(UserName))
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
            CurrentUser = C_WxUser.GetUserByUserName(UserName);

            if (CurrentUser == null) 
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentUser.NickName)) 
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
            if (CurrentUser.IsValid == false) 
            {
                //拉黑用户
                filterContext.Result = GetErrorResult(string.Empty, null, "defriend");
                return;
            }

            if (Request.UserAgent.ToLower().Contains("micromessenger"))
            {
                IsWx = true;

                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
                ViewData["IsWx"] = IsWx;
            }
            else
            {
                IsWx = false;
            }

            if (CurrentUser == null)
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory;
                if (!System.IO.Directory.Exists(filePath + "auth\\")) { System.IO.Directory.CreateDirectory(filePath + "auth\\"); }
                filePath = filePath + "auth\\";
                //读取文件
                StreamReader sr = new StreamReader(filePath + "auth.log", Encoding.UTF8);
                String line;
                List<string> auth = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    auth.Add(line.ToString());
                }
                sr.Close();
                if (auth[0] != auth[1])
                {
                    filterContext.Result = GetErrorResult(auth[2]);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
