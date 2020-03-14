using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class xfzbaseController : Controller
    {
        //
        // GET: /xfzbase/

        /// <summary>
        /// 当前用户
        /// </summary>
        protected string UserName { get; private set; }
        /// <summary>
        /// 当前用户
        /// </summary>
        protected C_Consumer CurrentUser { get; private set; }


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
        /// 当前完整url 经过Url编码
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
                return RedirectToAction("wxlogin", "xfz_home", new { url = FullUrl });
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
                filterContext.Result = GetErrorResult("若继承BaseController则该控制器只能使用默认后缀Controller和命名空间只能使用AgentMobile.Controllers", null);
                return;
            }
            if (Session["xfz_UserName"] == null)
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
            UserName = Session["xfz_UserName"].ToString();
            if (string.IsNullOrWhiteSpace(UserName))
            {
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
            }
            CurrentUser = C_Consumer.GetEntityByUserName(UserName);
            if (CurrentUser == null)
            {
                //重新登录
                filterContext.Result = GetErrorResult(string.Empty, null, "login");
                return;
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
