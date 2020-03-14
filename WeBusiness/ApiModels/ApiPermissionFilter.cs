using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeModels;
using System.Threading;

namespace WeBusiness
{
    public class ApiPermissionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            RequestResult result = new RequestResult();
            //Thread.Sleep(2000);
            // 请求限制间隔
            string IPAddress = ((System.Web.HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            if (IPMsg.IsExist(IPAddress))
            {   
                result.code = 100;
                result.message = "请求太过频繁，请稍后再试！";
                actionContext.Response = GetHttpResponseMessage(result);
            }
            else
            {
                IPMsg.AddIPAddress(IPAddress);
            }
        }

        protected HttpResponseMessage GetHttpResponseMessage(object result)
        {
            string json = JsonConvert.SerializeObject(result);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}