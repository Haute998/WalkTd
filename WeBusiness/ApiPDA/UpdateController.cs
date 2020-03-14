using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;

namespace WeBusiness.ApiPDA
{
    [RoutePrefix("ApiPDA/Update")]
    public class UpdateController : ApiController
    {
        [Route("GetVersion")]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetVersion()
        {
            RequestResult result = new RequestResult();
            try
            {
                PDAApp app = PDAApp.GetDefultScanApp();
                app.AppPath = WeConfig.b_domain + app.AppPath;
                result.data = app;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("PDA更新出错：" + ex.Message, "PDA更新出错");
            }

            return result;
        }

        [Route("GetAgentVersion")]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetAgentVersion()
        {
            RequestResult result = new RequestResult();
            try
            {
                PDAAgentApp app = PDAAgentApp.GetDefultScanApp();
                app.AppPath = WeConfig.b_domain + app.AppPath;
                result.data = app;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("代理APP更新出错：" + ex.Message, "代理APP更新出错");
            }

            return result;
        }
    }
}
