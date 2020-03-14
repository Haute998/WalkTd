using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class cuanhuoController : Controller
    {
        //
        // GET: /cuanhuo/
        public ActionResult pre(int Error=0) 
        {
            if (Error==1)
            {
                ViewData["Error"] = "您输入的S/N码有误！";
            }
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
            return View();
        }
        public ActionResult bx()
        {
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
            return View();
        }
        public ActionResult yesbx(string sn)
        {
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
            //B_bd cnt = B_bd.bxq(sn);
            //ViewData["dat"] = cnt.dat;
            //ViewData["dat1"]= cnt.dat.AddMonths(12);
            return View();
        }
        public ActionResult nobx(string sn)
        {
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
            //B_bd cnt = B_bd.bxq(sn);
            //ViewData["dat"] = cnt.dat;
            //ViewData["dat1"] = cnt.dat.AddMonths(12);
            return View();
        }
        public ActionResult pre1()
        {
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
            return View();
        }
        public ActionResult mima()
        {
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
            return View();
        }
        public ContentResult tomima(string mima)
        {
            if (string.IsNullOrWhiteSpace(mima))
            {
                return Content("密码不能为空");
            }
            B_User user = B_User.GetPassWord(mima);

            if (user == null)
            {
                return Content("密码错误");
            }

            return Content("ok");
        }
        public ActionResult Index(string ID)
        {
            try
            {
                // string O_ID = "", ProductNumber = "", ProductName = "", Name = "", Province = "", DatCreate = "", kw = "", ProductImg = "";
                string O_ID = "产品未经授权，谨防假冒！";
                string ProductNumber = "产品未经授权，谨防假冒！";
                string ProductName = "产品未经授权，谨防假冒！";
                string Name = "产品未经授权，谨防假冒！";
                string Province = "产品未经授权，谨防假冒！";
                string DatCreate = "产品未经授权，谨防假冒！";
                string kw = "产品未经授权，谨防假冒！";
                string ProductImg = "";
                string ycd = "产品未经授权，谨防假冒！";
                string zzs = "产品未经授权，谨防假冒！";
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    List<ScaleOutStoke> Scale = ScaleOutStoke.GetSmallScaleList(ID);
                    if (Scale.Count > 0)
                    {
                        ScaleOutStokeShow show = ScaleOutStoke.GetProductC_UserByBig(ID);
                        if (show != null)
                        {
                            Name = show.Name;
                            ProductName = show.ProductName;
                            ProductNumber = show.ProductNumber;
                            ProductImg = show.ProductImg;
                            O_ID = show.OutOrderNo;
                            kw = show.kw.ToString();
                            ycd = "中国";
                            zzs = "zunko";
                            C_User user = C_User.GetC_UserByUserName(show.Consignee);
                            Province = user == null ? "" : user.Province + user.City;
                            DatCreate = CommonFunc.GetDateTimeFromTimestamp(show.CreateTime).ToString("yyyy-MM-dd");
                            DAL.Log.Instance.Write("  " + show.CreateTime + "  || " + DatCreate, "111111");
                        }
                    }
                    else
                    {
                        ScaleOutStokeShow showbig = ScaleOutStoke.GetProductC_UserByBig(ID);
                        if (showbig != null && showbig.Name != null && showbig.ProductName != null)
                        {
                            Name = showbig.Name;
                            ProductName = showbig.ProductName;
                            ProductNumber = showbig.ProductNumber;
                            ProductImg = showbig.ProductImg;
                            O_ID = showbig.OutOrderNo;
                            kw = showbig.kw.ToString();
                            ycd = "中国";
                            zzs = "zunko";
                            C_User user = C_User.GetC_UserByUserName(showbig.Consignee);

                            Province = user == null ? "" : user.Province + user.City;
                            DatCreate = CommonFunc.GetDateTimeFromTimestamp(showbig.CreateTime).ToString("yyyy-MM-dd");
                            DAL.Log.Instance.Write("  " + showbig.CreateTime + "  || " + DatCreate, "111111");
                        }
                    }
                }
                /*if (string.IsNullOrWhiteSpace(O_ID) && string.IsNullOrWhiteSpace(ProductName))
                {
                    return Redirect("/cuanhuo/pre?Error=1");
                }*/
                ViewData["O_ID"] = O_ID;
                ViewData["ProductNumber"] = ProductNumber;
                ViewData["ProductName"] = ProductName;
                ViewData["Name"] = Name;
                ViewData["ProductImg"] = ProductImg;
                ViewData["Province"] = Province;
                ViewData["DatCreate"] = DatCreate;
                ViewData["kw"] = kw;
                ViewData["ycd"] = ycd;
                ViewData["zzs"] = zzs;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.Message, "防窜货查询出错!");
            }
            return View();
        }
        public ActionResult Index1(string ID)
        {
            try
            {
                string O_ID = "Beware of fake products without authorization";
                string ProductNumber = "Beware of fake products without authorization";
                string ProductName = "Beware of fake products without authorization";
                string Name = "Beware of fake products without authorization";
                string Province = "Beware of fake products without authorization";
                string DatCreate = "Beware of fake products without authorization";
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    List<ScaleOutStoke> Scale = ScaleOutStoke.GetSmallScaleList(ID);
                    if (Scale.Count > 0)
                    {
                        ScaleOutStokeShow show = ScaleOutStoke.GetProductC_UserByBig(ID);
                        if (show != null)
                        {
                            Name = show.Name;
                            ProductName = show.ProductName;
                            ProductNumber = show.ProductNumber;
                            O_ID = show.OutOrderNo;

                            C_User user = C_User.GetC_UserByUserName(show.Consignee);
                            Province = user == null ? "" : user.Province + user.City;
                            DatCreate =CommonFunc.GetDateTimeFromTimestamp(show.CreateTime).ToString("yyyy-MM-dd");

                        }
                    }
                    else
                    {
                        ScaleOutStokeShow showbig = ScaleOutStoke.GetProductC_UserByBig(ID);
                        if (showbig != null && showbig.Name != null && showbig.ProductName != null)
                        {
                            Name = showbig.Name;
                            ProductName = showbig.ProductName;
                            ProductNumber = showbig.ProductNumber;
                            O_ID = showbig.OutOrderNo;

                            C_User user = C_User.GetC_UserByUserName(showbig.Consignee);

                            Province = user == null ? "" : user.Province + user.City;
                            DatCreate = CommonFunc.GetDateTimeFromTimestamp(showbig.CreateTime).ToString("yyyy-MM-dd");
                        }
                    }
                }

                ViewData["O_ID"] = O_ID;
                ViewData["ProductNumber"] = ProductNumber;
                ViewData["ProductName"] = ProductName;
                ViewData["Name"] = Name;
                ViewData["Province"] = Province;
                ViewData["DatCreate"] = DatCreate;
            }
            catch(Exception ex)
            {
                DAL.Log.Instance.Write(ex.Message,"查询出错！");
            }
            return View();
        }
        public bool IsWx { get; set; }
    }
}
