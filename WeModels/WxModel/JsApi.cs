using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Net;
using LitJson;
using Newtonsoft.Json;
using DAL;

namespace WeModels
{
    /// <summary>
    /// 该类支持通过APPID，APPSECRET
    /// </summary>
    public class JsApi
    {
        /// <summary>
        /// 保存页面对象，因为要在类的方法中使用Page的Request对象
        /// </summary>
        private Controller controller { get; set; }
        /// <summary>
        /// openid用于调用统一下单接口
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// access_token(用户授权)
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 有效值：APPID，APPSECRET
        /// </summary>
        public BaseWxConfig WxConfig { get { return BaseWxConfig.GetWxConfig(); } set { } }
        public JsApi(Controller controller)
        {
            this.controller = controller;
        }

        /**
        * 
        * 网页授权获取用户基本信息的全部过程
        * 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        * 第一步：利用url跳转获取code
        * 第二步：利用code去获取openid和access_token
        * snsapi_base
        */
        public string GetOpenidAndAccessToken()
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(controller.Request.QueryString["code"]))
            {
                //获取code码，以获取openid和access_token
                string code = controller.Request.QueryString["code"];
                if (GetOpenidAndAccessTokenFromCode(code) == "no") 
                {
                    string reloadUrl = controller.Request.Url.AbsoluteUri.Replace("code", "");
                    return HttpUtility.HtmlEncode(reloadUrl);
                }
                DAL.Log.Instance.Write(code, "获取code");
            }
            else
            {
                //构造网页授权获取code的URL
                string host = controller.Request.Url.Host;
                string path = controller.Request.RawUrl;
                string redirect_uri = HttpUtility.UrlEncode("http://" + host + path);
                WxData data = new WxData();
                data.SetValue("appid", WxConfig.APPID);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_base");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
            }
            return url;
        }


        /// <summary>
        /// 获取Openid、AccessToken    snsapi_userinfo方式
        /// </summary>
        public string GetOpenidAndAccessToken_snsapi_userinfo()
        {
            string url = string.Empty;
           
            if (!string.IsNullOrEmpty(controller.Request.QueryString["code"]))
            {
                //获取code码，以获取openid和access_token
                string code = controller.Request.QueryString["code"];
                if (GetOpenidAndAccessTokenFromCode(code) == "no") 
                {
                    return HttpUtility.HtmlEncode(controller.Request.Url.AbsoluteUri);
                }
                DAL.Log.Instance.Write(code, "获取code");
            }
            else
            {
                // 构造网页授权获取code的URL
                string host = controller.Request.Url.Host;
                string path = controller.Request.RawUrl;
                DAL.Log.Instance.Write("host:" + host + ",path:" + path, "构造网页授权获取code的URL");
                string redirect_uri = HttpUtility.UrlEncode("http://" + host + path);
                WxData data = new WxData();
                data.SetValue("appid", WxConfig.APPID);
                data.SetValue("redirect_uri", redirect_uri);
                data.SetValue("response_type", "code");
                data.SetValue("scope", "snsapi_userinfo");
                data.SetValue("state", "STATE" + "#wechat_redirect");
                url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
            }
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetWxCode(string code)
        {
            if (GetOpenidAndAccessTokenFromCode(code) == "no")
            {
                return HttpUtility.HtmlEncode(controller.Request.Url.AbsoluteUri);
            }
            else
            {
                return "ok";
            }
        }

        public string GetCodeUrl()
        {
            string host = controller.Request.Url.Host;
            string path = controller.Request.RawUrl;
            DAL.Log.Instance.Write("host:" + host + ",path:" + path, "构造网页授权获取code的URL");
            string redirect_uri = HttpUtility.UrlEncode("http://" + host + path);
            WxData data = new WxData();
            data.SetValue("appid", WxConfig.APPID);
            data.SetValue("redirect_uri", redirect_uri);
            data.SetValue("response_type", "code");
            data.SetValue("scope", "snsapi_userinfo");
            data.SetValue("state", "STATE" + "#wechat_redirect");
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?" + data.ToUrl();
			
            return url;
        }

      
	    
        //* 通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
        //* {
        //*  "access_token":"ACCESS_TOKEN",
        //*  "expires_in":7200,
        //*  "refresh_token":"REFRESH_TOKEN",
        //*  "openid":"OPENID",
        //*  "scope":"SCOPE",
        //*  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
        //* }
        //* 其中access_token可用于获取共享收货地址
        //* openid是微信支付jsapi支付接口统一下单时必须的参数
        //* 更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
        //* @失败时抛异常WxPayException   
        
        /// <summary>
        /// 通过 code 获取 access_token
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOpenidAndAccessTokenFromCode(string code)
        {
            try
            {
                Log.Instance.Write("appid:" + WxConfig.APPID + ",secret:" + WxConfig.APPSECRET + ",code" + code, "Error_Param");
                //构造获取openid及access_token的url
                WxData data = new WxData();
                data.SetValue("appid", WxConfig.APPID);
                data.SetValue("secret", WxConfig.APPSECRET);
                data.SetValue("code", code);
                data.SetValue("grant_type", "authorization_code");
                string url = "https://api.weixin.qq.com/sns/oauth2/access_token?" + data.ToUrl();

                //请求url以获取数据
                string result = WebRequestHelper.Get(url);
                if (result.Contains("errcode"))
                {
                    return "no";
                }

                DAL.Log.Instance.Write(result + "，APPSECRET：" + WxConfig.APPSECRET, "获取openid和code");
                //保存access_token，用于收货地址获取
                JsonData jd = JsonMapper.ToObject(result);
                access_token = (string)jd["access_token"];

                //获取用户openid
                openid = (string)jd["openid"];

                return string.Empty;

            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "Error_GetOpenidAndAccessTokenFromCode");
                return string.Empty;
            }
        }




    }
}