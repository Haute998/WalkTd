using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class MyCodeController : Controller
    {
        //
        // GET: /MyCode/

        public ActionResult Index(string ID)
        {

            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            string nonceStr = WXVariousApi.GenerateNonceStr();
            string timestamp = WXVariousApi.GenerateTimeStamp();
            ViewData["signature"] = VariousApi.GetSignature(Request.Url.Host.ToString(), nonceStr, timestamp);
            ViewData["nonceStr"] = nonceStr;
            ViewData["timestamp"] = timestamp;
            ViewData["AppID"] = VariousApi.WxConfig.APPID;

            ViewData["Url"] = DESEncrypt.DesEncrypt(ID);
            ViewData["Share"] = ID;
            return View();

        }

    }
}
