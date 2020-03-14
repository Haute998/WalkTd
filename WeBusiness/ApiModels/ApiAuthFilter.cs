using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace WeBusiness
{
    public class ApiAuthFilter : ActionFilterAttribute
    {
        public string AuthCode{get;set;}
        public ApiAuthFilter(string code)
        {
            AuthCode = code;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
        }
    }
}