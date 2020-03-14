using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WeBusiness
{
    public static class WebApiConfig
    {
        //管理工具包执行安装命令：Install-Package Microsoft.AspNet.WebApi.WebHost
        public static void Register(HttpConfiguration config)
        {
            // 启用Web API特性路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "ApiPDA/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "MobileApi",
                routeTemplate: "ApiMobile/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "CommApi",
                routeTemplate: "Api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
