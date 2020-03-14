using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeBusiness.Models;
using System.IO;
using System.Text;

namespace WeBusiness
{
    /// <summary> 
    /// 错误页面
    /// </summary>
    public class ErrorPage
    {
        /// <summary>
        /// 视图名
        /// </summary>
        public const string ViewName = "Error";


        /// <summary>
        /// 错误标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

    }

    /// <summary>
    /// 控制器基类
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 登录后唯一标识(存客户端Cookie里)
        /// </summary>
        protected string GuidCode { get; private set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        protected B_User CurrentUser { get; private set; }



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

        private B_MenuRightsTagAttribute _MenuRightsTag;
        /// <summary>
        /// 菜单权限标签
        /// </summary>
        protected B_MenuRightsTagAttribute MenuRightsTag
        {
            get { return _MenuRightsTag; }
        }


        //异常处理
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
            WeException eV = ex as WeException;
            if (eV != null)
            {
                title = eV.Ex_TypeName;
                message = eV.Ex_Message;
                detail = eV.Ex_Content;
            }

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


        private ActionResult GetErrorResult(string message, string title = null, bool isRedirectToLogin = false)
        {
            //if (filterContext.HttpContext.Request.IsAjaxRequest())
            if (actionResult == ReturnResult.Content)
            {
                return Content(message);
            }
            else if (actionResult == ReturnResult.Json)
            {
                return Json(new { success = false, error = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (isRedirectToLogin)
                {
                    string userAgent = Request.UserAgent;

                    return RedirectToAction("Login", "Home", new { url = FullUrl });
                }
                //跳转到异常页面
                if (title != null)
                {
                    return View(ErrorPage.ViewName, new ErrorPage { Title = title, Message = message });
                }
                return View(ErrorPage.ViewName, new ErrorPage { Message = message });
            }
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (CurrentUser != null)
            {
                var tmpControllerName = ControllerName + "Controller";
                object[] arrObjects = filterContext.ActionDescriptor.GetCustomAttributes(typeof(B_MenuRightsTagAttribute), false);
                if (arrObjects.Length > 0 && CurrentUser.UserName != WeConfig.robot)
                {
                    _MenuRightsTag = arrObjects[0] as B_MenuRightsTagAttribute;
                    if (_MenuRightsTag != null)
                    {
                        //如果不是主菜单则取主菜单方法名
                        string tmpActionName = _MenuRightsTag.IsMainMenu ? ActionName : _MenuRightsTag.MainMethod;
                        B_Menu menu = B_MenuRights.GetMenu(tmpControllerName, tmpActionName);
                        string errMessage = string.Empty;
                        if (menu != null && string.IsNullOrWhiteSpace(menu.MenuName) == false)
                        {
                            List<B_MenuRights> mrList = B_MenuRights.GetHashMenuRights(CurrentUser.UserName).FindAll(m => m.CodeOn == tmpControllerName);
                            bool isExists = false;
                            if (_MenuRightsTag.IsMainMenu)
                            {
                                isExists = mrList.Exists(m => m.MethodName == _MenuRightsTag.Name && m.MethodCode.ToLower() == ActionName.ToLower());
                            }
                            else
                            {
                                //继承主方法
                                isExists = mrList.Exists(m => m.MethodName == _MenuRightsTag.Name &&
                                    (m.MethodCode == _MenuRightsTag.MainMethod || m.MethodCode.ToLower() == ActionName.ToLower() || m.HasInheritMethodCode(ActionName)));
                            }
                            if (isExists == false)
                            {
                                errMessage = "您没有" + menu.MenuName + _MenuRightsTag.Name + "权限的操作！";
                            }
                            if (menu.IsValid == false)
                            {
                                errMessage = "该页已停用，暂时不能访问";
                            }
                        }
                        else
                        {
                            //代码中已加权限标签，但菜单中没有添加该菜单或没获取到权限
                            errMessage = "您没有权限操作该页";
                        }

                        if (errMessage.Equals(string.Empty) == false)
                        {
                            filterContext.Result = GetErrorResult(errMessage);
                        }
                    }
                }
            }

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

        private ReturnResult actionResult = ReturnResult.View;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            ReflectedActionDescriptor descriptor = filterContext.ActionDescriptor as ReflectedActionDescriptor;
            if (descriptor != null)
            {
                Type actionType = descriptor.MethodInfo.ReturnType;
                if (actionType.Equals(typeof(ActionResult)))
                {
                    if (actionType.IsSubclassOf(typeof(JsonResult)))
                    {
                        actionResult = ReturnResult.Json;
                    }
                    else if (actionType.IsSubclassOf(typeof(ContentResult)))
                    {
                        actionResult = ReturnResult.Content;
                    }
                }
                else
                {
                    if (actionType.Equals(typeof(JsonResult)))
                    {
                        actionResult = ReturnResult.Json;
                    }
                    else if (actionType.Equals(typeof(ContentResult)))
                    {
                        actionResult = ReturnResult.Content;
                    }
                }
            }

            _ActionName = filterContext.ActionDescriptor.ActionName;
            _ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            string _ControllerType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.ToString();
            if (_ControllerType != "WeBusiness.Controllers." + _ControllerName + "Controller")
            {
                filterContext.Result = GetErrorResult("若继承BaseController则该控制器只能使用默认后缀Controller和命名空间只能使用WeBusiness.Controllers", null, false);
                return;
            }

            BaseAuthorizeModel auth = BaseAuthorizeHelper.GetAuthorizeModel(filterContext.HttpContext, CurrentURL);
            if (string.IsNullOrWhiteSpace(auth.GuidCode) == false)
            {
                GuidCode = auth.GuidCode;
            }

            string _loginfo = "异常退出";
            if (string.IsNullOrWhiteSpace(auth.TempDataMsg) == false)
            {
                TempData["Msg"] = auth.TempDataMsg;
                _loginfo = auth.TempDataMsg;
            }

            if (auth.IsAuthorize == false)
            {
                //异常退出
                logType = SYSLogType.Other;
                logInfo = _loginfo;
                logUser = auth.UserName;
                //WriteSYSLog();

                filterContext.Result = GetErrorResult(auth.TempDataMsg, null, true);
            }
            else
            {
                CurrentUser = auth.CurrentSYSUser;
                logUser = CurrentUser.UserName;
            }
        }

        /// <summary>
        /// 写入菜单日志
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logInfo"></param>
        protected void WriteSYSLog(string logType, string logInfo)
        {

        }

        private string logType = SYSLogType.Visit;
        private string logInfo = "访问页面";
        private string logUser = string.Empty;


        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //WriteSYSLog();
        }



    }
}
