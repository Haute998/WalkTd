using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace YstSmsDemoBASE64
{
    public class YstSmsDemoBASE64
    {
        #region 参数配置
        //static LogClass log = new LogClass();

        public static int requestcunt = 0;
        public static int timeoutcout = 0;

        public string Urlsend = "http://www.yescloudtree.cn:28001";
        public string Urlquery = "http://www.yescloudtree.cn:28002";

        public string UserName = "ssf";//(必填)
        public string Password = "1B9A638ECECDD727A120810CEC9F877A";//(必填)需MD5加密
        public string Mobile = "15151981693";//(必填)多个号码用英文状态下分号隔开
        public string Message = HttpUtility.UrlEncode("【1231】房产动态码456789，您本次支付的订单贷款金额：100.50元。最终还款日：2017-12-31，请按时还款！ ", Encoding.UTF8);

        public string MsgID = "";//选填
        public string ExtCode = "";//(选填)扩展码
        Encoding encoding = Encoding.GetEncoding("utf-8");

        #endregion


        #region BASE64加密短信群发
        public static string BASE64SendSmsMult(string contents, string Mobile)
        {
            string Urlsend = "http://www.yescloudtree.cn:28001";
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string UserName = "tcfw";//(必填)
            string Password = "E9C93F76EDDEB783F8BD92B806769611";//(必填)需MD5加密
            string baseMessage = Base64Encode(HttpUtility.UrlEncode(contents, Encoding.UTF8));
            DateTime begintime = System.DateTime.Now;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "sendsmsbase64");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            parameters.Add("Mobile", Mobile);
            parameters.Add("Message", baseMessage);
            parameters.Add("IsP2p", "0");
            parameters.Add("MsgID", "");
            parameters.Add("ExtCode", "");

            HttpWebResponse response = CreatePostHttpResponse(Urlsend, parameters, encoding);
            //打印返回值  
            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html 
            String[] strsResult = Regex.Split(result, ":", RegexOptions.IgnoreCase);
            ///  Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>短信群发" + Result(int.Parse(strsResult[0])) + result);
            DateTime endtime = System.DateTime.Now;
            TimeSpan ts = endtime - begintime;
            double Milliseconds = ts.TotalMilliseconds;
            return Result(int.Parse(strsResult[0])) + result;
        }
        public void BASE64SendSmsMults()
        {
            string baseMessage = Base64Encode(Message);
            DateTime begintime = System.DateTime.Now;
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "sendsmsbase64");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            parameters.Add("Mobile", Mobile);
            parameters.Add("Message", baseMessage);
            parameters.Add("IsP2p", "0");
            parameters.Add("MsgID", MsgID);
            parameters.Add("ExtCode", ExtCode);

            HttpWebResponse response = CreatePostHttpResponse(Urlsend, parameters, encoding);
            //打印返回值  
            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html 
            String[] strsResult = Regex.Split(result, ":", RegexOptions.IgnoreCase);
            ///  Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>短信群发" + Result(int.Parse(strsResult[0])) + result);
            DateTime endtime = System.DateTime.Now;
            TimeSpan ts = endtime - begintime;
            double Milliseconds = ts.TotalMilliseconds;
            if ((int)Milliseconds > 2000)
            {
                timeoutcout = timeoutcout + 1;
                // log.WriteError("Post TimeOut", "第" + requestcunt.ToString() + "次,请求耗时" + Milliseconds.ToString() + "ms>>");

            }

            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "<<群发方式>>正在请求第{0}次,本次请求耗时{1}ms,返回结果{2}", requestcunt.ToString(), Milliseconds.ToString(), Result(int.Parse(strsResult[0])) + result);

        }
        #endregion

        #region 获取状态报告
        public void ReportSms()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "ReportSms");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            DateTime begintime = System.DateTime.Now;
            HttpWebResponse response = CreatePostHttpResponse(Urlquery, parameters, encoding);
            //打印返回值  

            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html 
          //  log.WriteError("report_result:", result);
            String[] strsResult = Regex.Split(result, ":", RegexOptions.IgnoreCase);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>获取状态报告" + Result(int.Parse(strsResult[0])) + result);
            DateTime endtime = System.DateTime.Now;
            TimeSpan ts = endtime - begintime;
            double Milliseconds = ts.TotalMilliseconds;
            Console.WriteLine("获取状态报告耗时：" + Milliseconds);
            Thread.Sleep(5000);

            ReportSms();
        }
        #endregion

        #region 获取状态报告含状态时间
        public void ReportSmsTime()
        {



            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "ReportSmsTime");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            DateTime begintime = System.DateTime.Now;
            HttpWebResponse response = CreatePostHttpResponse(Urlquery, parameters, encoding);
            //打印返回值  

            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html 
         //   log.WriteError("reportdone_result:", result);
            String[] strsResult = Regex.Split(result, ":", RegexOptions.IgnoreCase);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>获取状态报告" + Result(int.Parse(strsResult[0])) + result);
            DateTime endtime = System.DateTime.Now;
            TimeSpan ts = endtime - begintime;
            double Milliseconds = ts.TotalMilliseconds;
            Console.WriteLine("获取状态报告耗时：" + Milliseconds);
            Thread.Sleep(2000);

            ReportSmsTime();


            //  ReportSmsTime 方法名 
            // 2017-10-24 09:52:30>>获取状态报告成功,0:<StatusPacks><StatusPack><Id></Id><Mobile>15151981693</Mobile><Status>DELIVRD</Status><ErrCode>DELIVRD</ErrCode><DoneTime>2017/9/21 13:59:39</DoneTime></StatusPack><StatusPack><Id></Id><Mobile>15151981693</Mobile><Status>DELIVRD</Status><ErrCode>DELIVRD</ErrCode><DoneTime>2017/9/21 14:05:30</DoneTime></StatusPack></StatusPacks>

        }
        #endregion

        #region 获取上行回复
        public void RevetSms()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "MoSms");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            HttpWebResponse response = CreatePostHttpResponse(Urlquery, parameters, encoding);
            //打印返回值  
            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html splitMo(result);
           // log.WriteError("mo_result:", result);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>" + result);
            // splitMo(result);
            Thread.Sleep(5000);
            RevetSms();
            //2017-10-24 10:00:28>>0:<MOPacks><MOPack><DestId>1069105170390001</DestId><Mobile>15151981693</Mobile><Message><![CDATA[%e6%95%a2%e6%ac%ba%e8%b4%9f%e4%bd%a0]]></Message><MOTime>2017/10/24 9:57:43</MOTime></MOPack></MOPacks>

        }
        #endregion

        #region 获取余额
        public void RemainSms()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("Action", "RemainSms");
            parameters.Add("UserName", UserName);
            parameters.Add("Password", Password);
            HttpWebResponse response = CreatePostHttpResponse(Urlquery, parameters, encoding);
            //打印返回值
            Stream stream = response.GetResponseStream();   //获取响应的字符串流  
            StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
            string result = sr.ReadToEnd();   //从头读到尾，放到字符串html 
            String[] strsResult = Regex.Split(result, ":", RegexOptions.IgnoreCase);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>获取余额" + Result(int.Parse(strsResult[0])) + result);
            Thread.Sleep(30 * 1000);
            RemainSms();
        }
        #endregion

        #region 上行回复解析
        public void splitMo(string xmlstr)
        {
            string xml = "";
            string innerText = "";
            string motime = "";
            string destid = "";
            string message = "";
            string mobile = "";
            string pattern = "<MOPacks>(?<account>.*?)</MOPacks>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            XmlDocument document = new XmlDocument();
            String[] strsResult = Regex.Split(xmlstr, ":", RegexOptions.IgnoreCase);
            if (regex.IsMatch(xmlstr))
            {
                foreach (Match match in regex.Matches(xmlstr))
                {
                    xml = match.Groups[0].Value;
                    document.LoadXml(xml);
                    XmlNodeList list = document.SelectNodes("MOPacks");

                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (XmlNode node in list)
                        {
                            foreach (XmlNode node2 in node.SelectNodes("MOPack"))
                            {
                                destid = node2.SelectSingleNode("DestId").InnerText;
                                innerText = node2.SelectSingleNode("Mobile").InnerText;
                                motime = node2.SelectSingleNode("MOTime").InnerText;
                                message = HttpUtility.UrlDecode(node2.SelectSingleNode("Message").InnerText, Encoding.UTF8);

                                mobile = "";

                                if (innerText.StartsWith("86"))
                                {
                                    mobile = innerText.Substring(2, innerText.Length - 2);
                                }
                                else
                                {
                                    mobile = innerText;
                                }
                            }

                            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>获取上行" + Result(int.Parse(strsResult[0])) + xmlstr);

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ">>获取上行" + Result(int.Parse(strsResult[0])) + xmlstr);

            }

        }
        #endregion

        #region HttpPost
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding charset)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            requestcunt = requestcunt + 1;//请求次数记数器
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            request.Timeout = 300000;

            //发送POST数据  
            // Console.WriteLine("post开始");

            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        i++;
                    }
                }
                byte[] data = Encoding.ASCII.GetBytes(buffer.ToString());
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            string[] values = request.Headers.GetValues("Content-Type");

            return request.GetResponse() as HttpWebResponse;
        }
        #endregion

        #region MD5加密
        public static string GetMD5Str(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            // return sb.ToString().ToLower();//小写
            return sb.ToString().ToUpper();//大写
        }
        #endregion

        #region BASE64加密
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }
        #endregion

        #region 接口返回值说明
        public static String Result(int i)
        {

            String r = "";
            if (i == 0)
            {
                r = "成功,";
            }
            else if (i == 1)
            {
                r = "失败，原因：目的号码太长（最多100个),错误码：";
            }
            else if (i == 2)
            {
                r = "失败，原因：超过今天的最大发送量,错误码：";
            }
            else if (i == 3)
            {
                r = "失败，原因：所剩于的发送总量低于您现在的发送量,错误码：";
            }
            else if (i == 4)
            {
                r = "失败，原因：信息提交失败,错误码：";
            }
            else if (i == 5)
            {
                r = "失败，原因：出现未知情况，请联系管理员,错误码：";
            }
            else if (i == 6)
            {
                r = "失败，原因：点对点手机号码与短信内容数量不匹配,错误码：";
            }
            else if (i == 7)
            {
                r = "失败，原因：Action参数错误,错误码：";
            }
            else if (i == 8)
            {
                r = "失败，原因：系统故障,错误码：";
            }
            else if (i == 9)
            {
                r = "失败，原因：用户名密码不正确,错误码：";
            }
            else if (i == 10)
            {
                r = "失败，原因：定时时间格式错误,错误码：";
            }
            else if (i == 20)
            {
                r = "失败，原因：账户不支持闪信,错误码：";
            }
            else if (i == 21)
            {
                r = "失败，原因：闪信不支持长短信发送，请确认字数保证在70字内含签名,错误码：";
            }
            else if (i == 11)
            {
                r = "失败，原因：定时时间小于当前时间,需大于当前时间30分钟,错误码：";
            }
            else if (i == 99)
            {
                r = "失败，原因：超出许可连接数,错误码：";
            }
            else if (i == 30)
            {
                r = "失败，原因：存在屏蔽词";
            }
            else
            {
                r = "失败，未知错误,错误码：";
            }
            return r;
        }
        #endregion
    }
}
