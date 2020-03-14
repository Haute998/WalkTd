using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DAL;
using WeModels.WxModel;

namespace WeModels
{
    /// <summary>
    /// 调用之前，请先LoadWxConfigIncidentalAccess_token
    /// </summary>
    public class WXVariousApi
    {
        public BaseWxConfig WxConfig { get; set; }
        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxData unifiedOrderResult { get; set; }

        private static object locker = new object();
        /// <summary>
        /// 读取微信配置
        /// </summary>
        public void LoadWxConfigIncidentalAccess_token()
        {
            double FirstExpires_inDouble = 200;//提前两百秒刷新
            lock (locker)
            {
                WxConfig = BaseWxConfig.GetWxConfig();
                if (string.IsNullOrEmpty(WxConfig.ACCESS_TOKEN) || WxConfig.token_dat_now.AddSeconds(WxConfig.token_expires_in - FirstExpires_inDouble) < DateTime.Now || string.IsNullOrWhiteSpace(WxConfig.JSApiTicket))
                {
                    UpdateAccess_token();
                }
            }

        }
        /// <summary>
        /// 重新获取Access_token同时更新JSApiTicket
        /// </summary>
        private void UpdateAccess_token()
        {
            try
            {
                WxConfig.token_dat_now = DateTime.Now;//记录当前更新Access_token的时间
                WxData datatoken = new WxData();
                datatoken.SetValue("grant_type", "client_credential");
                datatoken.SetValue("appid", WxConfig.APPID);
                datatoken.SetValue("secret", WxConfig.APPSECRET);
                string tokenurl = "https://api.weixin.qq.com/cgi-bin/token?" + datatoken.ToUrl();
                //请求url以获取数据
                string tokenresult = WebRequestHelper.Get(tokenurl);
                JsonData jd = JsonMapper.ToObject(tokenresult);
                WxConfig.ACCESS_TOKEN = jd["access_token"].ToString();//记录获取到的Access_token
                WxConfig.token_expires_in = Convert.ToDouble(jd["expires_in"].ToString());//记录有效时间(秒)
                WxConfig.token_dat_now = DateTime.Now;
                DAL.Log.Instance.Write(tokenresult, "获取access_token");

                string result = WebRequestHelper.Get("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + WxConfig.ACCESS_TOKEN + "&type=jsapi");
                Log.Instance.Write("Getjsapi_ticket :" + result, "Getjsapi_ticket");
                JsapiTicketRtn rtn = JsonConvert.DeserializeObject<JsapiTicketRtn>(result);
                DAL.Log.Instance.Write(result, "获取jsapi_ticket");
                if (rtn.errcode == "0")
                {
                    WxConfig.JSApiTicket = rtn.ticket;
                }
                WxConfig.UpdateByID();

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_UpdateAccess_token");
                //throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 通过OpenID获取微信用户信息
        /// </summary>
        /// <returns></returns>
        public WXUserInfo GetUserInfo(string openid)
        {
            try
            {
                WxData data = new WxData();
                data.SetValue("access_token", WxConfig.ACCESS_TOKEN);
                data.SetValue("openid", openid);
                data.SetValue("lang", "zh_CN");
                string url = "https://api.weixin.qq.com/cgi-bin/user/info?" + data.ToUrl();

                //请求url以获取数据
                string result = WebRequestHelper.Get(url);
                //保存用户信息
                WXUserInfo wxuserinfo = JsonConvert.DeserializeObject<WXUserInfo>(result);
                DAL.Log.Instance.Write("请求：" + data.ToUrl() + "，返回：" + result, "获取微信用户信息");
                return wxuserinfo;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_GetUserInfo");
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// 获取微信用户信息  授权方式
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public WXUserInfo GetUserInfo0(string openid, string access_token)
        {
            try
            {
                string url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", access_token, openid);
                //请求url以获取数据
                string result = WebRequestHelper.Get(url);
                //保存用户信息
                WXUserInfo wxuserinfo = JsonConvert.DeserializeObject<WXUserInfo>(result);
                DAL.Log.Instance.Write("请求：" + url + "，返回：" + result, "获取微信用户信息授权方式");
                return wxuserinfo;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_GetUserInfo0");
                return null;
            }
        }

        /**
        * 生成随机串，随机串包含字母或数字
        * @return 随机串
        */
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        #region 重新获取Code的跳转链接(没有用户授权的，只能获取基本信息）
        /// <summary>重新获取Code,以后面实现带着Code重新跳回目标页面(没有用户授权的，只能获取基本信息（openid））</summary>
        /// <param name="url">目标页面</param>
        /// <returns></returns>
        public string GetCodeUrl(string url, string APPID)
        {
            //对url进行编码
            url = System.Web.HttpUtility.UrlEncode(url);
            string CodeUrl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + APPID + "&redirect_uri=" + url + "?action=viewtest&response_type=code&scope=snsapi_base&state=1#wechat_redirect";
            return CodeUrl;

        }
        #endregion

        #region 以Code换取用户的openid、access_token
        /// <summary>根据Code获取用户的openid、access_token</summary>
        public string GetOauthAccessOpenId(string code, string APPID, string APPSECRET)
        {
            string Openid = "";
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + APPID + "&secret=" + APPSECRET + "&code=" + code + "&grant_type=authorization_code";
            string gethtml = WebRequestHelper.Get(url);
            //Log.Instance.Write("拿到的url是：" + url);
            //Log.Instance.Write("获取到的gethtml是" + gethtml);
            OAuth_Token ac = new OAuth_Token();
            ac = JsonConvert.DeserializeObject<OAuth_Token>(gethtml);
            //Log.Instance.Write("能否从html里拿到openid=" + ac.openid);
            Openid = ac.openid;
            return Openid;
        }

        public class OAuth_Token
        {
            /// <summary>
            /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
            /// </summary>
            public string access_token { get; set; }
            /// <summary>
            /// access_token接口调用凭证超时时间，单位（秒）
            /// </summary>
            public string expires_in { get; set; }
            /// <summary>
            /// 用户刷新access_token
            /// </summary>
            public string refresh_token { get; set; }
            /// <summary>
            /// 用户唯一标识,请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户授权作用域
            /// </summary>
            public string scope { get; set; }
        }
        #endregion
        /**
          * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
           * @return 时间戳
          */
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


        /// <summary>
        /// 获取signature（通过AccessToken和jsapi_ticket获取）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="nonceStr"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public string GetSignature(string url, string nonceStr, string timestamp)
        {
            string signatureStr = "jsapi_ticket=" + WxConfig.JSApiTicket + "&noncestr=" + nonceStr + "&timestamp=" + timestamp + "&url=" + url;
            Log.Instance.Write("signatureStr :" + signatureStr, "GetSignature");
            //SHA1加密
            string signature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(signatureStr, "SHA1");
            Log.Instance.Write("signature :" + signature, "GetSignature");
            return signature;
        }

        /**
        *  
        * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
        * 微信浏览器调起JSAPI时的输入参数格式如下：
        * {
        *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
        *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
        *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
        *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
        *   "signType" : "MD5",         //微信签名方式:    
        *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
        * }
        * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
        * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
        * 
        */
        public string GetJsApiParameters()
        {
            Log.Instance.Write("JsApiPay::GetJsApiParam is processing...", "GetJsApiParameters");

            WxData jsApiParam = new WxData();
            jsApiParam.SetValue("appId", WxConfig.APPID);
            jsApiParam.SetValue("timeStamp", GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(WxConfig.PayKey));

            string parameters = jsApiParam.ToJson();

            Log.Instance.Write("Get jsApiParam :" + parameters, "GetJsApiParameters");
            return parameters;
        }



        /**
        * 
        * 统一下单
        * @param WxPaydata inputObj 提交给统一下单API的参数
        * @param int timeOut 超时时间
        * @throws WxPayException
        * @return 成功时返回，其他抛异常
        */
        public WxData UnifiedOrder(WxData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                Log.Instance.Write("缺少统一支付接口必填参数out_trade_no！", "Error_UnifiedOrder");
            }
            else if (!inputObj.IsSet("body"))
            {
                Log.Instance.Write("缺少统一支付接口必填参数body！", "Error_UnifiedOrder");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                Log.Instance.Write("缺少统一支付接口必填参数total_fee！", "Error_UnifiedOrder");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                Log.Instance.Write("缺少统一支付接口必填参数trade_type！", "Error_UnifiedOrder");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                Log.Instance.Write("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！", "Error_UnifiedOrder");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                Log.Instance.Write("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！", "Error_UnifiedOrder");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxConfig.NotifyUrl);//异步通知url
            }

            inputObj.SetValue("appid", WxConfig.APPID);//公众账号ID
            inputObj.SetValue("mch_id", WxConfig.MchID);//商户号
            inputObj.SetValue("spbill_create_ip", HttpContext.Current.Request.UserHostAddress);//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(WxConfig.PayKey));
            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            Log.Instance.Write("Request:" + xml, "Log_UnfiedOrder");
            string response = WebRequestHelper.PostUTF8(url, xml);
            Log.Instance.Write("Response:" + response, "Log_UnfiedOrder");

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxData result = new WxData();
            result.FromXml(response, WxConfig.PayKey,true);


            return result;
        }

        /**
    * 调用统一下单，获得下单结果
    * @return 统一下单结果
    * @失败时抛异常WxPayException
    */
        /// <summary>
        /// 统一下单 二维码模式一
        /// </summary>
        /// <param name="order"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
   
        /// <summary>
        /// 获取微信菜单
        /// </summary>
        /// <returns></returns>
        public JsonData GetMenus() 
        {
            string url = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token="+WxConfig.ACCESS_TOKEN;
            string result = WebRequestHelper.Get(url);
            DAL.Log.Instance.Write(result, "MenusString");
            JsonData jdrtns = JsonMapper.ToObject(result);
            return jdrtns;
        }

        /// <summary>
        /// 通过OpenID列表群发消息(文本)
        /// </summary>
        /// <returns></returns>
        public JsonData SendMsgText(List<string> openidlst, string content)
        {
            try
            {

                Dictionary<string, string> haha = new Dictionary<string, string>();
                haha.Add("content",HttpUtility.UrlEncode(content));

                WxData data = new WxData();
                data.SetValue("touser", openidlst);
                data.SetValue("msgtype", "text");
                data.SetValue("text", haha);
                string url = "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token=" + WxConfig.ACCESS_TOKEN;

                string sendreq=HttpUtility.UrlDecode(data.ToJson());
                //请求url以获取数据
                string result = WebRequestHelper.PostUTF8(url, sendreq);
                //结果
                //JsonData jd = JsonMapper.ToObject(result);
                DAL.Log.Instance.Write("请求：" + sendreq + "，返回：" + result, "根据openid群发文本");
                JsonData jdrtns = JsonMapper.ToObject(result);
                return jdrtns;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_SendMsgText");
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// 通过OpenID列表群发消息(图文消息)
        /// </summary>
        /// <returns></returns>
        public JsonData SendMsgNews(List<string> openidlst, string media_id)
        {
            try
            {

                Dictionary<string, string> haha = new Dictionary<string, string>();
                haha.Add("media_id", media_id);

                WxData data = new WxData();
                data.SetValue("touser", openidlst);
                data.SetValue("msgtype", "mpnews");
                data.SetValue("mpnews", haha);
                string url = "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token=" + WxConfig.ACCESS_TOKEN;

                //请求url以获取数据
                string result = WebRequestHelper.PostUTF8(url, data.ToJson());
                //结果
                //JsonData jd = JsonMapper.ToObject(result);
                DAL.Log.Instance.Write("请求：" + data.ToJson() + "，返回：" + result, "根据openid群发图文");
                JsonData jdrtns = JsonMapper.ToObject(result);
                return jdrtns;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_SendMsgNews");
                throw new Exception(ex.ToString());
            }
        }


        /// <summary>
        /// 获取素材列表
        /// </summary>
        /// <param name="type">素材的类型，图片（image）、视频（video）、语音 （voice）、图文（news）</param>
        /// <param name="offset">从全部素材的该偏移位置开始返回，0表示从第一个素材 返回</param>
        /// <param name="count">返回素材的数量，取值在1到20之间</param>
        /// <returns></returns>
        public JsonData GetMaterials(string type, int offset, int count) 
        {
            try
            {
                WxData data = new WxData();
                data.SetValue("type", type);
                data.SetValue("offset", offset);
                data.SetValue("count", count);
                string url = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=" + WxConfig.ACCESS_TOKEN;
                string result = WebRequestHelper.PostUTF8(url, data.ToJson());
                DAL.Log.Instance.Write("请求：" + data.ToJson() + "，返回：" + result, "获取素材列表");
                JsonData jdrtns = JsonMapper.ToObject(result);
                return jdrtns;
            }
            catch (Exception ex) 
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_GetMaterials");
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// 获取素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public JsonData GetMaterial(string media_id)
        {
            try
            {
                WxData data = new WxData();
                data.SetValue("media_id", media_id);
                string url = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=" + WxConfig.ACCESS_TOKEN;
                string result = WebRequestHelper.PostUTF8(url, data.ToJson());
                DAL.Log.Instance.Write("请求：" + data.ToJson() + "，返回：" + result, "根据media_id获取素材");
                JsonData jdrtns = JsonMapper.ToObject(result);
                return jdrtns;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "Error_GetMaterial");
                throw new Exception(ex.ToString());
            }
        }
        /// <summary>
        /// 获取永久参数二维码 返回二维码图片解析后的地址
        /// </summary>
        /// <param name="scene_str"></param>
        /// <returns></returns>
        //public string GetWxQCodeTicket_Limit_Str(string scene_str) 
        //{
        //    try
        //    {
        //        DAL.Log.Instance.Write("请求二维码参数" + scene_str, "GetWxQCodeTicket_Limit_Str");
        //        string jsonStr = "{\"action_name\": \"QR_LIMIT_STR_SCENE\", \"action_info\": {\"scene\": {\"scene_str\": \"" + scene_str + "\"}}}";
        //        DAL.Log.Instance.Write("请求" + jsonStr, "GetWxQCodeTicket_Limit_Str");
        //        JsonData responseData=GetWxQCodeTicket(jsonStr);
        //        string ticket = responseData["ticket"].ToString();
        //        string url = responseData["url"].ToString();

        //        WxQCode qCode = new WxQCode();
        //        qCode.action_name = "action_name";
        //        qCode.scene_str = scene_str;
        //        qCode.url = url;
        //        qCode.dat = DateTime.Now;
        //        int rtn= qCode.InsertAndReturnIdentity();
        //        return url;
        //    }
        //    catch (Exception ex) 
        //    {
        //        DAL.Log.Instance.Write(ex.ToString(), "Error_GetWxQCodeTicket_Limit_Str");
        //        return string.Empty;
        //    }
        //}


        /// <summary>
        /// 获取参数二维码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private JsonData GetWxQCodeTicket(string req)
        {
            Log.Instance.Write("请求"+req, "WXVariousApi_GetWxQCodeTicket");
            string url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + WxConfig.ACCESS_TOKEN;
            string result = WebRequestHelper.PostUTF8(url, req);
            DAL.Log.Instance.Write("请求：" + req + "，返回：" + result, "WXVariousApi_GetWxQCodeTicket");
            JsonData jdrtns = JsonMapper.ToObject(result);
            return jdrtns;
        }



        public WxData sendredpackToUser(RedPackOrder order, string openid, string client_ip)
        {
            try
            {
                string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
                WxData requestData = new WxData();
                requestData.SetValue("mch_billno", order.OrderNo);//商户订单号
                requestData.SetValue("mch_id", WxConfig.MchID);//商户号
                requestData.SetValue("wxappid", WxConfig.APPID);//公众账号ID
                requestData.SetValue("send_name", "杜康");//商户名称 WxConfig.ConfigName
                requestData.SetValue("re_openid", openid);
                requestData.SetValue("total_amount", (order.total_amount * 100).ToString("0"));
                requestData.SetValue("total_num", order.total_num);
                requestData.SetValue("wishing", order.wishing);
                requestData.SetValue("scene_id", "PRODUCT_2");
                requestData.SetValue("client_ip", client_ip);
                requestData.SetValue("act_name", order.act_name);
                requestData.SetValue("remark", order.remark);
                //requestData.SetValue("scene_id", order.scene_id);
                //requestData.SetValue("risk_info", "");
                //requestData.SetValue("consume_mch_id","");
                requestData.SetValue("nonce_str", GenerateNonceStr());//随机字符串

                //签名
                requestData.SetValue("sign", requestData.MakeSign(WxConfig.PayKey));
                string xml = requestData.ToXml();
                Log.Instance.Write("Request:" + xml, "sendredpackToUser");

                string response = WeModels.BaseHelper.HttpService.PostPageCert(url, xml);
                Log.Instance.Write("Response:" + response, "sendredpackToUser");


                WxData result = new WxData();
                result.FromXml(response, WxConfig.PayKey, false);
                return result;
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "sendredpackToUser_error");
                return null;
            }
        }


        /// <summary>
        /// 发送模版消息
        /// </summary>
        /// <param name="reqJson">请求json参数</param>
        /// <returns></returns>
        public string SendTempMsg(string reqJson)
        {
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + WxConfig.ACCESS_TOKEN + "";
            string rtn = WebRequestHelper.PostUTF8(url, reqJson);
            Log.Instance.Write("request:" + reqJson + ",response:" + rtn, "WXVariousApi_SendTempMsg");
            return rtn;
        }


        /// <summary>
        /// 下载多媒体素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetMaterial_Media(string media_id, string path) 
        {
            return "";
        }
        /// <summary>
        /// 删除素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public bool DelMedia(string media_id) 
        {
            try
            {
                string url = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=" + WxConfig.ACCESS_TOKEN;
                WxData data = new WxData();
                data.SetValue("media_id", media_id);
                string result = WebRequestHelper.PostUTF8(url, data.ToJson());
                JsonData jdrtns = JsonMapper.ToObject(result);
                return jdrtns["errcode"].ToString() == "0" ? true : false;
            }
            catch (Exception ex) 
            {
                Log.Instance.Write(ex.ToString(), "DelMedia_error");
                return false;
            }
        }
        /// <summary>
        /// 下载多媒体
        /// </summary>
        /// <param name="media_id"></param>
        public string GetMedia(string media_id,string path) 
        {
            try
            {

                string url = "http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=" + WxConfig.ACCESS_TOKEN + "&media_id=" + media_id;
                //string rtn = WebRequestHelper.Get(url);


                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                Stream stream = new FileStream(path, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                return "ok";
            }
            catch (Exception ex) 
            {
                Log.Instance.Write(ex.ToString(), "GetMedia_error");
                return "请求出现异常";
            }
        }


        /// <summary>
        /// 付款到用户
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        //public WxData PaymentToUser(C_UserTXOrder order,string openid,string spbill_create_ip)
        //{
        //    string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
        //    WxData requestData = new WxData();
        //    requestData.SetValue("mch_appid", WxConfig.APPID);//公众账号ID
        //    requestData.SetValue("mchid", WxConfig.MchID);//商户号
        //    requestData.SetValue("device_info", "");//微信支付分配的终端设备号  	    
        //    requestData.SetValue("nonce_str", GenerateNonceStr());//随机字符串
        //    requestData.SetValue("partner_trade_no", order.OrderNo);//商户订单编号
        //    requestData.SetValue("openid", openid);
        //    requestData.SetValue("check_name", "NO_CHECK");
        //    requestData.SetValue("re_user_name", "");
        //    requestData.SetValue("amount", (order.Amount * 100).ToString("0"));
        //    requestData.SetValue("desc", order.Explain);
        //    requestData.SetValue("spbill_create_ip", spbill_create_ip);

        //    //签名
        //    requestData.SetValue("sign", requestData.MakeSign(WxConfig.PayKey));
        //    string xml = requestData.ToXml();
        //    Log.Instance.Write("Request:" + xml, "Log_PaymentToUser");

        //    string response = WeModels.BaseHelper.HttpService.PostPageCert(url, xml);
        //    Log.Instance.Write("Response:" + response, "Log_PaymentToUser");


        //    WxData result = new WxData();
        //    result.FromXml(response, WxConfig.PayKey, false);
        //    return result;
        //}



        /// <summary>
        /// 微信上传多媒体文件
        /// </summary>
        /// <param name="filepath">文件绝对路径</param>
        public UpLoadInfo WxUpLoad(string filepath,MediaType mt)
        {
            try
            {

                    WebClient myWebClient = new WebClient();
                    myWebClient.Credentials = CredentialCache.DefaultCredentials;
                    string wxurl= string.Format("http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", WxConfig.ACCESS_TOKEN, mt.ToString());

                    byte[] responseArray = myWebClient.UploadFile(wxurl, "POST", filepath);
                        string result = System.Text.Encoding.Default.GetString(responseArray, 0, responseArray.Length);
                        if(result.Contains("media_id"))
                        {
                        return JsonConvert.DeserializeObject<UpLoadInfo>(result);
                        }
                        else
                        {//否则，写错误日志

                            Log.Instance.Write(result, "WxUpLoad_error");//写错误日志
                            return null;
                        }
            }
            catch (Exception ex) 
            {
                Log.Instance.Write(ex.ToString(), "WxUpLoad_error");//写错误日志
                return null;
            }
        }

        /// <summary>
        /// 创建公众号菜单 正确返回ok  否则为失败原因
        /// </summary>
        /// <param name="WxMenuJsonM"></param>
        /// <returns></returns>
        public string CreatMenu(WxMenuJsonModel WxMenuJsonM)
        {
            string req = JsonConvert.SerializeObject(WxMenuJsonM);
            string url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + WxConfig.ACCESS_TOKEN;

            //请求url以获取数据
            string result = WebRequestHelper.PostUTF8(url, req);
            MenuRtn MRtn = JsonConvert.DeserializeObject<MenuRtn>(result);
            if (MRtn.errcode == "0" && MRtn.errmsg.ToLower() == "ok")
            {
                return "ok";
            }
            return MRtn.errmsg;

        }

        /// <summary>
        /// JsapiTicket返回结果
        /// </summary>
        public class JsapiTicketRtn
        {
            public string errcode { get; set; }
            public string errmsg { get; set; }
            public string ticket { get; set; }
            public string expires_in { get; set; }
        }

        public class MenuRtn 
        {
            /// <summary>
            /// 正确返回0
            /// </summary>
            public string errcode { get; set; }
            /// <summary>
            /// 正确返回ok
            /// </summary>
            public string errmsg { get; set; }
        }

        public enum MediaType
        {
            /// <summary>
            /// 图片（image）: 1M，支持JPG格式
            /// </summary>
            image,
            /// <summary>
            /// 语音（voice）：2M，播放长度不超过60s，支持AMR\MP3格式
            /// </summary>
            voice,
            /// <summary>
            /// 视频（video）：10MB，支持MP4格式
            /// </summary>
            video,
            /// <summary>
            /// 缩略图（thumb）：64KB，支持JPG格式
            /// </summary>
            thumb
        }
        public class UpLoadInfo
        {
            /// <summary>
            /// 媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb，主要用于视频与音乐格式的缩略图）
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 媒体文件上传后，获取时的唯一标识
            /// </summary>
            public string media_id { get; set; }
            /// <summary>
            /// 媒体文件上传时间戳
            /// </summary>
            public string created_at { get; set; }
        }
    }
}
