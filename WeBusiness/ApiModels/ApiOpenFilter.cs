using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeModels;

namespace WeBusiness
{
    public class ApiOpenFilter : ActionFilterAttribute
    {
        #region 属性

        private bool _IsOpenApi;
        /// <summary>
        /// 菜单权限标签
        /// </summary>
        protected bool IsOpenApi
        {
            get { return _IsOpenApi; }
        }
        
        #endregion
        public ApiOpenFilter(bool IsOpen)
        {
            _IsOpenApi = IsOpen;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            RequestResult result = new RequestResult();

            // 请求限制间隔

            HttpContextBase context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;

            if (!IsOpenApi)
            {
                if (request["UserToken"]==null)
                {
                    result.code = 101;
                    result.message = "无法验证用户密钥.";
                    actionContext.Response = GetHttpResponseMessage(result);
                }
                else
                {
                    string UserToken = request["UserToken"].ToString();
                    if (PDAUserMsg.TokenGetUser(UserToken) == null)
                    {
                        result.code = 101;
                        result.message = "用户密钥已过期或不存在.";
                        actionContext.Response = GetHttpResponseMessage(result);
                    }
                }
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