using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/ServerCheck")]
    public class ServerCheckController : ApiBaseController
    {
        [HttpPost]
        public RequestResult Post(string chkcode)
        {
            RequestResult result = new RequestResult();
            result.data = chkcode;
            result.message = "正常";
            result.success = true;
            return result;
        }

        [HttpGet]
        public RequestResult Get()
        {
            RequestResult result = new RequestResult();
            result.message = "正常";
            result.success = true;
            return result;
        }
    }
}
