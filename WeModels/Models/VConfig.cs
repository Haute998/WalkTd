using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class VConfig
    {
        /// <summary>
        /// 微信调试
        /// </summary>
        public static string WxDebug = ConfigurationManager.AppSettings["WxDebug"] ?? "";
        /// <summary>
        /// 超级管理员
        /// </summary>
        public static string robot = (ConfigurationManager.AppSettings["robot"] == null || ConfigurationManager.AppSettings["robot"].ToString().Trim() == "") ? "tc" : ConfigurationManager.AppSettings["robot"].ToString();
        /// <summary>
        /// 后台域名
        /// </summary>
        public static string b_domain = ConfigurationManager.AppSettings["b_domain"] ?? "";
        /// <summary>
        /// 微信端域名
        /// </summary>
        public static string wx_domain = ConfigurationManager.AppSettings["wx_domain"] ?? "";

        //=======【微信支付证书路径设置】===================================== 
        /* 证书路径,注意应该填写绝对路径（仅退款、撤销订单或其他资金流出的项目时需要）
        */
        public const string SSLCERT_PATH = "D:\\微商管理系统\\电脑端\\cert\\apiclient_cert.p12";
        public const string SSLCERT_PASSWORD = "1340126701";
    }
}
