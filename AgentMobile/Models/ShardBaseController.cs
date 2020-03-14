using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Models
{
    public class ShardBaseController : BaseController
    {
        /// <summary>
        /// 是否是微信中打开
        /// </summary>
        protected bool IsWx { get; private set; }
        private ActionResult GetErrorResult(string message, string title = null, string type = "")
        {

            //跳转到异常页面
            if (title != null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Title = title, Message = message });
            }
            return View(ErrorPage.ViewName, new ErrorPage { Message = message });
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
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

            IsWx = false;
            if (Request.UserAgent.ToLower().Contains("micromessenger"))
            {
                IsWx = true;

                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }

            ViewData["IsWx"] = IsWx;
        }

    }
}
