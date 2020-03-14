using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class WeConfig
    {
        /// <summary>
        /// 微信调试
        /// </summary>
        public static string WxDebug = ConfigurationManager.AppSettings["WxDebug"] ?? "";
        /// <summary>
        /// 超级管理员
        /// </summary>
        public static string robot = (ConfigurationManager.AppSettings["robot"] == null || ConfigurationManager.AppSettings["robot"].ToString().Trim() == "") ? "way" : ConfigurationManager.AppSettings["robot"].ToString();
        /// <summary>
        /// 后台域名
        /// </summary>
        public static string b_domain = ConfigurationManager.AppSettings["b_domain"] ?? "";
        /// <summary>
        /// 微信端域名
        /// </summary>
        public static string wx_domain = ConfigurationManager.AppSettings["wx_domain"] ?? "";

        public static string js_version= ConfigurationManager.AppSettings["Script:Version"] ?? "";
        public static string css_version = ConfigurationManager.AppSettings["Style:Version"] ?? "";
        public static string img_version = ConfigurationManager.AppSettings["Image:Version"] ?? "";
    }
}
