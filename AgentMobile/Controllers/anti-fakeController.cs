using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace AgentMobile.Controllers
{
    public class anti_fakeController : Controller
    {
        //
        // GET: /anti-fake/

        public ActionResult query(string id)
        {
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            IsWx = true;
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;

            }
            if (id == null)
            {
                ViewData["fwcode"] = "";
            }
            else
            {
                ViewData["fwcode"] = id;
            }
            return View();
        }
        private static ModelQueryParam GetAddress(string IPAddress)
        {
            ModelQueryParam queryParam = new ModelQueryParam();
            try
            {
                string strContent = HttpPost("http://api.map.baidu.com/location/ip?ak=32f38c9491f2da9eb61106aaab1e9739&ip=" + IPAddress + "&coor=bd09ll");
                strContent = "[" + strContent + "]";
                JArray ja = JArray.Parse(strContent);
                JObject o = (JObject)ja[0];
                if (int.Parse(o["status"].ToString()) == 0)
                {
                    queryParam.Area = o["content"]["address_detail"]["province"].ToString() + " " + o["content"]["address_detail"]["city"].ToString();
                    queryParam.Province = o["content"]["address_detail"]["province"].ToString();
                    queryParam.City = o["content"]["address_detail"]["city"].ToString();
                }
                else if (int.Parse(o["status"].ToString()) == 1)
                {
                    queryParam.Area = "本机地址";
                }
                else
                {
                    queryParam.Area = "未知 未知";
                    queryParam.Province = "未知";
                    queryParam.City = "未知";
                }
            }
            catch
            {
                queryParam.Area = "未知";
            }

            return queryParam;
        }
        private static string HttpPost(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return Unicode2String(retString);
        }
        private static string Unicode2String(string source)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                         source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
        public ActionResult query1(string id)
        {
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            IsWx = true;
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;

            }
            if (id == null)
            {
                ViewData["fwcode"] = "";
            }
            else
            {
                ViewData["fwcode"] = id;
            }
            return View();
        }
        public ActionResult querypc(string id)
        {
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            IsWx = true;
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;

            }
            if (id == null)
            {
                ViewData["fwcode"] = "";
            }
            else
            {
                ViewData["fwcode"] = id;
            }
            return View();
        }
        public ActionResult queryenpc(string id)
        {
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            IsWx = true;
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;

            }
            if (id == null)
            {
                ViewData["fwcode"] = "";
            }
            else
            {
                ViewData["fwcode"] = id;
            }
            return View();
        }
        public ActionResult AntiFakeSel(string ID)
        {
  
            string ip = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            ModelQueryParam queryip = new ModelQueryParam();
            queryip = GetAddress(ip);
            string province = queryip.Province;
            string city = queryip.City;

            string msg = "此防伪码不存在，请您检查标签，确认防伪码是否输入正确，然后再请您试一次。";
            Scale scake = Scale.GetCAntiFake(ID);

            SelScale.GetSelScale();

            int countto = 0;
            //string img = "";
            if (scake != null)
            {
                int count = 0;  // scake.SelCount + 1;
                if (count == 1)
                {   
                    DateTime time = DateTime.Now;
                    msg = "此防伪码有效，您购买的是深圳市通程防伪科技有限公司所生产的正牌产品，请放心使用！";
                    //scake.SelectDate = time;
                        
                    SelScale selscale = new SelScale();
                    //查到当前标签的出货记录
                    ScaleOutStoke Stoke = ScaleOutStoke.GetSmallScaleListcode(ID);
                    
                    if (Stoke != null)
                    {
                        C_User user = new C_User();
                        if (Stoke.Consignee == "m2000")
                        {
                            //查到出货人信息
                            user = C_User.GetC_UserByUserName(Stoke.Shipper);
                            if (user.Province != province)
                            {   
                                selscale.warning = "窜货";
                            }
                            else
                            {
                                selscale.warning = "正常";
                            }
                            
                            selscale.Address = user.Province + user.City;
                        }
                        else
                        {
                            user = C_User.GetC_UserByUserName(Stoke.Consignee);
                            if (user.Province != province)
                            {
                                selscale.warning = "窜货";
                            }
                            else
                            {
                                selscale.warning = "正常";
                            }
                            
                            selscale.Address = user.Province + user.City;
                        }
                    }
                    
                    selscale.province = province;
                    selscale.city = city;
                    selscale.IP = ip;
                    selscale.AntiCode = ID;
                    selscale.InsertAndReturnIdentity();
                }
                else
                {
                    msg = "注意！此防伪码有效,但已被查询" + count +"次";
                }
                
                scake.UpdateByID();
                countto = count;
                return Content(countto + "|" + msg + "|" + scake.CreateTime );
            }
            return Content(countto + "|" + msg );
        }

        public static string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public string GetWebClientIp()
        {

            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null || System.Web.HttpContext.Current.Request.ServerVariables == null)
                {
                    return "";
                }

                string CustomerIP = "";

                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (CustomerIP == null)
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(CustomerIP, "unknown", true) == 0 || String.IsNullOrEmpty(CustomerIP))
                {
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return CustomerIP;
            }
            catch { }

            return userIP;

        }






        public static string str = "";
        public static int[] array = new int[16];
        public static System.Drawing.Color[] array1 = new System.Drawing.Color[16];
        public static string[] array2 = new string[16];
        public static System.Drawing.Color adsa1(string a0, int c)
        {
            System.Drawing.Color c0;
            string b4 = a0.ToString().Substring(a0.ToString().Length - 1, 1);
            if (b4 == "0")//红色#ff0000
            { c0 = Color.FromArgb(255, 231, 120, 23); }
            else if (b4 == "1")//蓝色#0000ff
            {
                c0 = Color.FromArgb(255, 255, 0, 255);

            }
            else if (b4 == "2")//绿色#00ff00
            { c0 = Color.FromArgb(255, 8, 84, 8); }
            else if (b4 == "3")//橙色ff6100
            { c0 = Color.FromArgb(255, 189, 169, 19); }
            else if (b4 == "4")//金黄色ffD700
            { c0 = Color.FromArgb(255, 30, 144, 255); }
            else if (b4 == "5")//金黄色ffD700
            { c0 = Color.FromArgb(255, 0, 0, 255); }
            else if (b4 == "6")//黑色#000000
            { c0 = Color.FromArgb(255, 0, 0, 0); }
            else if (b4 == "7")//金黄色ffD700
            { c0 = Color.FromArgb(255, 227, 85, 106); }
            else if (b4 == "8")//金黄色ffD700
            { c0 = Color.FromArgb(255, 255, 0, 0); }

            else //金黄色ffD700
            { c0 = Color.FromArgb(255, 0, 255, 127); }



            return c0;
        }

        public string BreakParallelFor3(String aaaa)
        {

            string filePath1 = aaaa + ".png";
            string id1 = aaaa;
            string fwcode = aaaa;
            string id = fwcode;
            string id2 = id.Substring(0, 1) + "," + id.Substring(1, 1) + "," + id.Substring(2, 1) + "," + id.Substring(3, 1) + "," + id.Substring(4, 1) + "," + id.Substring(5, 1) + "," + id.Substring(6, 1) + "," + id.Substring(7, 1) + "," + id.Substring(8, 1) + "," + id.Substring(9, 1) + "," + id.Substring(10, 1) + "," + id.Substring(11, 1) + "," + id.Substring(12, 1) + "," + id.Substring(13, 1) + "," + id.Substring(14, 1) + "," + id.Substring(15, 1);
            string id3 = id.Substring(4, 2) + "," + id.Substring(5, 2) + "," + id.Substring(6, 2) + "," + id.Substring(7, 2) + "," + id.Substring(8, 2) + "," + id.Substring(9, 2) + "," + id.Substring(10, 2) + "," + id.Substring(11, 2) + "," + id.Substring(12, 2) + "," + id.Substring(13, 2) + "," + id.Substring(14, 2) + ",";
            String[] a = id2.Split(',');
            String[] b = id3.Split(',');
            Bitmap image = new Bitmap(System.Web.HttpContext.Current.Server.MapPath("/images/Code.png"));

            image.SetResolution(200, 200);
            int w = 0;
            int h = 0;
            int wn = 8;
            int hn = 3;
            int bb = 1;

            Graphics g = Graphics.FromImage(image);
            Point point1 = new Point(w + wn * (int.Parse(b[1]) - int.Parse(a[4]) - bb), h + hn * (int.Parse(b[2]) - int.Parse(a[8]) - bb));
            Point point2 = new Point(w + wn * (int.Parse(b[1])), h + hn * (int.Parse(b[2])));
            Point point3 = new Point(w + wn * (int.Parse(b[1]) + int.Parse(a[6]) + bb), h + hn * (int.Parse(b[2]) + int.Parse(a[10]) + bb));


            Point point12 = new Point(w + wn * (int.Parse(b[2]) - int.Parse(a[12]) - bb), h + hn * (int.Parse(b[3]) - int.Parse(a[14]) - bb));
            Point point22 = new Point(w + wn * (int.Parse(b[2])), h + hn * (int.Parse(b[3])));
            Point point32 = new Point(w + wn * (int.Parse(b[2]) + int.Parse(a[5]) + bb), h + hn * (int.Parse(b[3]) + int.Parse(a[15]) + bb));

            Point point13 = new Point(w + wn * (int.Parse(b[3]) - int.Parse(a[14]) - bb), h + hn * (int.Parse(b[4]) - int.Parse(a[5]) - bb));
            Point point23 = new Point(w + wn * (int.Parse(b[3])), h + hn * (int.Parse(b[4])));
            Point point33 = new Point(w + wn * (int.Parse(b[3]) + int.Parse(a[15]) + bb), h + hn * (int.Parse(b[4]) + int.Parse(a[4]) + bb));

            Point point14 = new Point(w + wn * (int.Parse(b[4]) - int.Parse(a[12]) - bb), h + hn * (int.Parse(b[5]) - int.Parse(a[7]) - bb));
            Point point24 = new Point(w + wn * (int.Parse(b[4])), h + hn * (int.Parse(b[5])));
            Point point34 = new Point(w + wn * (int.Parse(b[4]) + int.Parse(a[13]) + bb), h + hn * (int.Parse(b[5]) + int.Parse(a[6])) + bb);


            Point point11 = new Point(w + wn * (int.Parse(b[5]) - int.Parse(a[10]) - bb), h + hn * (int.Parse(b[6]) - int.Parse(a[9]) - bb));
            Point point21 = new Point(w + wn * (int.Parse(b[5])), h + hn * (int.Parse(b[6])));
            Point point31 = new Point(w + wn * (int.Parse(b[5]) + int.Parse(a[11]) + bb), h + hn * (int.Parse(b[6]) + int.Parse(a[8]) + bb));

            Point point112 = new Point(w + wn * (int.Parse(b[6]) - int.Parse(a[8]) - bb), h + hn * (int.Parse(b[7]) - int.Parse(a[11]) - bb));
            Point point122 = new Point(w + wn * (int.Parse(b[6])), h + hn * (int.Parse(b[7])));
            Point point132 = new Point(w + wn * (int.Parse(b[6]) + int.Parse(a[9]) + bb), h + hn * (int.Parse(b[7]) + int.Parse(a[10]) + bb));

            Point point113 = new Point(w + wn * (int.Parse(b[7]) - int.Parse(a[6]) - bb), h + hn * (int.Parse(b[8]) - int.Parse(a[13]) - bb));
            Point point123 = new Point(w + wn * (int.Parse(b[7])), h + hn * (int.Parse(b[8])));
            Point point133 = new Point(w + wn * (int.Parse(b[7]) + int.Parse(a[7]) + bb), h + hn * (int.Parse(b[8]) + int.Parse(a[12]) + bb));

            Point point114 = new Point(w + wn * (int.Parse(b[8]) - int.Parse(a[4]) - bb), h + hn * (int.Parse(b[9]) - int.Parse(a[15]) - bb));
            Point point124 = new Point(w + wn * (int.Parse(b[8])), h + hn * (int.Parse(b[9])));
            Point point134 = new Point(w + wn * (int.Parse(b[8]) + int.Parse(a[5]) + bb), h + hn * (int.Parse(b[9]) + int.Parse(a[14]) + bb));


            Point[] curvePoints = { point1, point2, point3 };
            Point[] curvePoints2 = { point12, point22, point32 };
            Point[] curvePoints3 = { point13, point23, point33 };
            Point[] curvePoints4 = { point14, point24, point34 };

            Point[] curvePoints1 = { point11, point21, point31 };
            Point[] curvePoints12 = { point112, point122, point132 };
            Point[] curvePoints13 = { point113, point123, point133 };
            Point[] curvePoints14 = { point114, point124, point134 };
            //g.DrawLine(new Pen(Color.Silver),10, 120, 20, 36);

            GraphicsPath myPath = new GraphicsPath();
            GraphicsPath myPath2 = new GraphicsPath();
            GraphicsPath myPath3 = new GraphicsPath();
            GraphicsPath myPath4 = new GraphicsPath();

            GraphicsPath myPath1 = new GraphicsPath();
            GraphicsPath myPath12 = new GraphicsPath();
            GraphicsPath myPath13 = new GraphicsPath();
            GraphicsPath myPath14 = new GraphicsPath();
            //AddCurve(点阵,起点,终点,弯曲程度)

            Color ca = adsa1(a[5], 0);
            float size = 10f;

            myPath.AddCurve(curvePoints, 0, 2, 1f);
            Pen myPen = new Pen(ca, size);
            g.DrawPath(myPen, myPath);

            ca = adsa1(a[9], 0);
            myPath2.AddCurve(curvePoints2, 0, 2, 0.8f);
            Pen myPen2 = new Pen(ca, size);
            g.DrawPath(myPen2, myPath2);

            ca = adsa1(a[10], 0);
            myPath3.AddCurve(curvePoints3, 0, 2, 0.8f);
            Pen myPen3 = new Pen(ca, size);
            g.DrawPath(myPen3, myPath3);

            ca = adsa1(a[11], 0);
            myPath4.AddCurve(curvePoints4, 0, 2, 0.8f);
            Pen myPen4 = new Pen(ca, size);
            g.DrawPath(myPen4, myPath4);

            ca = adsa1(a[12], 0);
            myPath1.AddCurve(curvePoints1, 0, 2, 0.8f);
            Pen myPen1 = new Pen(ca, size);
            g.DrawPath(myPen1, myPath1);

            ca = adsa1(a[13], 0);
            myPath12.AddCurve(curvePoints12, 0, 2, 0.8f);
            Pen myPen12 = new Pen(ca, size);
            g.DrawPath(myPen12, myPath12);

            ca = adsa1(a[14], 0);
            myPath13.AddCurve(curvePoints13, 0, 2, 0.8f);
            Pen myPen13 = new Pen(ca, size);
            g.DrawPath(myPen13, myPath13);

            ca = adsa1(a[15], 0);
            myPath14.AddCurve(curvePoints14, 0, 2, 0.8f);
            Pen myPen14 = new Pen(ca, size);
            g.DrawPath(myPen14, myPath14);
            Color ca1 = Color.Black;
            SolidBrush burshText1 = new SolidBrush(ca1);
            g.DrawString(a[7].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 120, 45);
            g.DrawString(a[8].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 400, 45);
            g.DrawString(a[9].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 650, 45);

            g.DrawString(a[10].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 120, 157);
            g.DrawString(a[11].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 400, 157);
            g.DrawString(a[12].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 650, 157);

            g.DrawString(a[13].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 120, 276);
            g.DrawString(a[14].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 400, 276);
            g.DrawString(a[15].ToString(), new System.Drawing.Font("黑体", 20.5f), burshText1, 650, 276);

            string cardUrl = "/images/code/" + filePath1;
            if (!Directory.Exists(Server.MapPath("~/images/code/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/images/code/"));
            }
            image.Save(Server.MapPath(cardUrl));

            g.Dispose();
            image.Dispose();
            return cardUrl.ToString();
        }


        public bool IsWx { get; set; }
    }
}
