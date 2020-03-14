using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class controlController : Controller
    {
        //
        // GET: /control/

        public string to()
        {
            try
            {
                string requestJson = Encoding.UTF8.GetString(Request.BinaryRead(Request.TotalBytes));
                request req = JsonConvert.DeserializeObject<request>(requestJson);

                if (req == null)
                {
                    return "请求数据为空";
                }
                if (DAL.MD5Helper.GetMD5UTF8("tongchengcontrol") != req.key)
                {
                    return "接口秘钥不正确";
                }
                //if (!string.IsNullOrWhiteSpace(req.msg))
                //{
                //    SYSNotifyMsg.sendSysMsg("", req.msg);
                //}

                //写入文件
                string filePath = AppDomain.CurrentDomain.BaseDirectory;
                if (!System.IO.Directory.Exists(filePath + "auth\\")) { System.IO.Directory.CreateDirectory(filePath + "auth\\"); }
                filePath = filePath + "auth\\";

                using (FileStream fileStream = new FileStream(filePath + "auth.log", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        //authkey
                        streamWriter.WriteLine(req.authkey);
                        //authvalue
                        streamWriter.WriteLine(req.authvalue);
                        streamWriter.WriteLine(req.msg);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string query()
        {
            request req = new request();
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
                req.authkey = auth[0];
                req.authvalue = auth[1];
                req.msg = auth[2];
                return JsonConvert.SerializeObject(req);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public class request
        {
            /// <summary>
            /// 接口秘钥 MD5
            /// </summary>
            public string key { get; set; }
            /// <summary>
            /// 消息  会通知客户
            /// </summary>
            public string msg { get; set; }
            /// <summary>
            /// 授权key
            /// </summary>
            public string authkey { get; set; }
            /// <summary>
            /// 授权值
            /// </summary>
            public string authvalue { get; set; }
        }

    }
}
