using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.BaseHelper
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        //public static string Post(string xml, string url, bool isUseCert, int timeout)
        //{
        //    System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

        //    string result = "";//返回结果

        //    HttpWebRequest request = null;
        //    HttpWebResponse response = null;
        //    Stream reqStream = null;

        //    try
        //    {
        //        //设置最大连接数
        //        ServicePointManager.DefaultConnectionLimit = 200;
        //        //设置https验证方式
        //        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        //        {
        //            ServicePointManager.ServerCertificateValidationCallback =
        //                    new RemoteCertificateValidationCallback(CheckValidationResult);
        //        }

        //        /***************************************************************
        //        * 下面设置HttpWebRequest的相关属性
        //        * ************************************************************/
        //        request = (HttpWebRequest)WebRequest.Create(url);

        //        request.Method = "POST";
        //        request.Timeout = timeout * 1000;

        //        //设置代理服务器
        //        //WebProxy proxy = new WebProxy();                          //定义一个网关对象
        //        //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
        //        //request.Proxy = proxy;

        //        //设置POST的数据类型和长度
        //        request.ContentType = "text/xml";
        //        byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
        //        request.ContentLength = data.Length;

        //        //是否使用证书
        //        if (isUseCert)
        //        {
        //            //string path = HttpContext.Current.Request.PhysicalApplicationPath;

        //            Log.Instance.Write(QinConfig.SSLCERT_PATH, "wxpaycertUrl");

        //            X509Certificate2 cert = new X509Certificate2(QinConfig.SSLCERT_PATH, QinConfig.SSLCERT_PASSWORD);
        //            request.ClientCertificates.Add(cert);
        //            Log.Instance.Write("PostXml used cert", "WxPayApi");
        //        }

        //        //往服务器写入数据
        //        reqStream = request.GetRequestStream();
        //        reqStream.Write(data, 0, data.Length);
        //        reqStream.Close();

        //        //获取服务端返回
        //        response = (HttpWebResponse)request.GetResponse();

        //        //获取服务端返回数据
        //        StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //        result = sr.ReadToEnd().Trim();
        //        sr.Close();
        //    }
        //    catch (System.Threading.ThreadAbortException e)
        //    {
        //        Log.Instance.Write("Thread - caught ThreadAbortException - resetting.", "HttpService");
        //        Log.Instance.Write("Exception message:" + e.Message, "HttpService");
        //        System.Threading.Thread.ResetAbort();
        //    }
        //    catch (WebException e)
        //    {
        //        Log.Instance.Write( e.ToString(),"HttpService");
        //        if (e.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            Log.Instance.Write("StatusCode : " + ((HttpWebResponse)e.Response).StatusCode, "HttpService");
        //            Log.Instance.Write( "StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription, "HttpService");
        //        }
        //        throw new Exception(e.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Instance.Write(e.ToString(), "HttpService");
        //        throw new Exception(e.ToString());
        //    }
        //    finally
        //    {
        //        //关闭连接和流
        //        if (response != null)
        //        {
        //            response.Close();
        //        }
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        ///    证书版
        /// </summary>
        /// <param name="posturl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string PostPageCert(string posturl, string postData)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            byte[] data = encoding.GetBytes(postData);
            // 准备请求...  
            try
            {
                //CerPath证书路径
                string certPath = VConfig.SSLCERT_PATH;
                //证书密码
                string password = VConfig.SSLCERT_PASSWORD;
                X509Certificate2 cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath, password, X509KeyStorageFlags.MachineKeySet);

                // 设置参数  
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.ContentLength = data.Length;
                request.ClientCertificates.Add(cert);
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据  
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求  
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码  
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;

            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "PostPageCert");
                return string.Empty;
            }
        }

        /// <summary>
        /// 处理http GET请求，返回数据
        /// </summary>
        /// <param name="url">请求的url地址</param>
        /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
        public static string Get(string url)
        {
            System.GC.Collect();
            string result = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            //请求url以获取数据
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                response = (HttpWebResponse)request.GetResponse();

                //获取HTTP返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                Log.Instance.Write(e.ToString(), "HttpService");
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                Log.Instance.Write(e.ToString(), "HttpService");
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    Log.Instance.Write("StatusCode : " + ((HttpWebResponse)e.Response).StatusCode, "HttpService");
                    Log.Instance.Write("StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription, "HttpService");
                }
                throw new Exception(e.ToString());
            }
            catch (Exception e)
            {
                Log.Instance.Write(e.ToString(), "HttpService");
                throw new Exception(e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
    }
}
