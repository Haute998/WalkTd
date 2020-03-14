using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using WeModels;
using WeModels.Models;

namespace WeBusiness.Models
{
    public class BaseAuthorizeHelper
    {
        /// <summary>
        /// 唯一标识码
        /// </summary>
        public const string GuidCodeCookieKey = "WeBusiness_JF_Test_GuidCode";


        /// <summary>
        /// 根据Controller类型获取菜单权限列表
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="actionName"></param>
        /// <param name="hasMainMenuTag"></param>
        /// <returns></returns>
        public static List<B_MenuRights> GetBOMenuRightsByControllerType(Type controllerType, string actionName, out bool hasMainMenuTag)
        {
            hasMainMenuTag = false;
            int index = controllerType.ToString().LastIndexOf('.');
            string controllerName = controllerType.ToString();
            if (index > 0)
            {
                controllerName = controllerName.Substring(index + 1);
            }
            List<B_MenuRights> listBOMenuRights = new List<B_MenuRights>();
            List<MethodInfo> methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase).ToList();
            if (methods.Count > 0)
            {
                //所有继承的菜单
                List<B_MenuRights> allBOMenuRights = new List<B_MenuRights>();
                foreach (MethodInfo method in methods)
                {
                    B_MenuRightsTagAttribute tag = method.GetCustomAttributes<B_MenuRightsTagAttribute>().FirstOrDefault();
                    if (tag != null && tag.IsMainMenu == false)
                    {
                        if (listBOMenuRights.Exists(m => m.MethodCode == method.Name && m.MethodName == tag.Name) == false)
                        {
                            allBOMenuRights.Add(new B_MenuRights
                            {
                                CodeOn = controllerName,
                                MethodCode = method.Name,
                                MethodName = tag.Name
                            });
                        }
                    }
                }
                //主菜单
                List<MethodInfo> selMethods = methods.FindAll(m => m.Name == actionName);
                foreach (MethodInfo method in selMethods)
                {
                    B_MenuRightsTagAttribute tag = method.GetCustomAttributes<B_MenuRightsTagAttribute>().FirstOrDefault();
                    if (tag != null)
                    {
                        if (tag.IsMainMenu)
                        {
                            hasMainMenuTag = true;
                        }
                        if (listBOMenuRights.Exists(m => m.MethodName == tag.Name) == false)
                        {
                            StringBuilder sbCodeInherits = new StringBuilder();
                            List<B_MenuRights> curBOMenuRights = allBOMenuRights.FindAll(m => m.MethodName == tag.Name);
                            foreach (B_MenuRights curRights in curBOMenuRights)
                            {
                                if (method.Name != curRights.MethodCode)
                                {
                                    sbCodeInherits.Append(curRights.MethodCode + ",");
                                }
                            }
                            B_MenuRights rights = new B_MenuRights
                            {
                                CodeOn = controllerName,
                                MethodCode = method.Name,
                                MethodName = tag.Name,
                                MethodCodeMain = sbCodeInherits.ToString().TrimEnd(',')
                            };
                            listBOMenuRights.Add(rights);
                        }
                    }
                }
                //继承菜单
                selMethods = methods.FindAll(m => m.Name != actionName);
                foreach (MethodInfo method in selMethods)
                {
                    B_MenuRightsTagAttribute tag = method.GetCustomAttributes<B_MenuRightsTagAttribute>().FirstOrDefault();
                    if (tag != null)
                    {
                        //不是主菜单且主方法相同和权限名称不同
                        if (tag.IsMainMenu == false && tag.MainMethod == actionName && listBOMenuRights.Exists(m => m.MethodName == tag.Name) == false)
                        {
                            StringBuilder sbCodeInherits = new StringBuilder();
                            List<B_MenuRights> curBOMenuRights = allBOMenuRights.FindAll(m => m.MethodName == tag.Name);
                            foreach (B_MenuRights curRights in curBOMenuRights)
                            {
                                if (method.Name != curRights.MethodCode)
                                {
                                    sbCodeInherits.Append(curRights.MethodCode + ",");
                                }
                            }
                            B_MenuRights rights = new B_MenuRights
                            {
                                CodeOn = controllerName,
                                MethodCode = method.Name,
                                MethodName = tag.Name,
                                MethodCodeMain = sbCodeInherits.ToString().TrimEnd(',')
                            };
                            listBOMenuRights.Add(rights);
                        }
                    }
                }
            }
            return listBOMenuRights;
        }
        /// <summary>
        /// 根据context获取基础授权
        /// </summary>
        /// <param name="context"></param>
        /// <param name="curUrl"></param>
        /// <returns></returns>
        public static BaseAuthorizeModel GetAuthorizeModel(HttpContextBase context, string curUrl = null)
        {   
            string error = "您的帐号已下线，请重新登录后再操作！";
            BaseAuthorizeModel auth = new BaseAuthorizeModel
            {
                IsAuthorize = false
            };

            bool isAuthenticated = context.User.Identity.IsAuthenticated;
            if (isAuthenticated == false)
            {
                auth.TempDataMsg = error;
                return auth;
            }

            //取唯一标识和判断是否第一次登录
            HttpCookie cookie = context.Request.Cookies.Get(GuidCodeCookieKey);
            if (cookie == null)
            {
                FormsAuthentication.SignOut();
                auth.TempDataMsg = error;
                return auth;
            }

            auth.GuidCode = cookie.Value;
            auth.UserName = context.User.Identity.Name;
            //处理应用程序重启还在线的用户
            B_User usr = B_UserManager.GetB_UserAndRefresh(u => u.UserName == auth.UserName, curUrl);
            if (usr == null)
            {
                if (B_UserManager.IsInitTimeOut == false)
                {
                    usr = B_UserManager.GetB_UserAndLogin(auth.UserName, u =>
                    {
                        if (u.IsValid == false)
                        {
                            error = "您的帐号被禁用，请及时联系管理员！";
                            return false;
                        }
                        if (u.GuidCode != auth.GuidCode)
                        {
                            //error = "您的帐号在其他地方登录，您已下线！";
                            return false;
                        }
                        return true;
                    });
                }
                if (usr == null)
                {
                    auth.TempDataMsg = error;
                    FormsAuthentication.SignOut();
                    return auth;
                }
            }

            //同步系统超时(精确到秒所以比实际延迟一点)
            if (usr.IsLoginedTimeOut)
            {
                auth.TempDataMsg = "您的帐号已超时，请重新登录后再操作！";
                //不用移除登录用户(登录时会自动移除超时用户)
                FormsAuthentication.SignOut();
                return auth;
            }
            if (usr.GuidCode != auth.GuidCode)
            {
                //auth.TempDataMsg = "您的帐号已在其他地方登录，您已下线！";
                //不用移除登录用户(不能移除在别处登录的用户)
                FormsAuthentication.SignOut();
                return auth;
            }
            //如果处理中有设置错误消息
            if (usr.IsNoError == false)
            {
                auth.TempDataMsg = usr.ErrorData;
                B_UserManager.RemoveUser(usr.UserName);
                FormsAuthentication.SignOut();
                return auth;
            }

            auth.IsAuthorize = true;
            auth.CurrentSYSUser = usr;
            return auth;
        }
    }
}