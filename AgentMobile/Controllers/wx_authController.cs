using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class wx_authController : BaseController
    {
        //
        // GET: /wx_auth/

        public ActionResult getconfig(string url)
        {
            wxjsconfig config = new wxjsconfig();
            try
            {
                if (IsWx)
                {
                    WXVariousApi VariousApi = new WXVariousApi();
                    VariousApi.LoadWxConfigIncidentalAccess_token();
                    string nonceStr = WXVariousApi.GenerateNonceStr();
                    string timestamp = WXVariousApi.GenerateTimeStamp();
                    config.signature = VariousApi.GetSignature(url, nonceStr, timestamp);
                    config.nonceStr = nonceStr;
                    config.timestamp = timestamp;
                    config.AppID = VariousApi.WxConfig.APPID;
                }
            }
            catch (Exception ex) 
            {
                DAL.Log.Instance.Write(ex.ToString(), "wx_auth_getconfig_error");
            }

            return Json(config, JsonRequestBehavior.AllowGet);
        }

        public class wxjsconfig 
        {
            public string signature { get; set; }
            public string nonceStr { get; set; }
            public string timestamp { get; set; }
            public string AppID { get; set; }
        }

    }
}
