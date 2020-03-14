using AgentMobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using System.Threading;
using DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentMobile.Controllers
{
    public class PrizeDrawController : HBBaseController
    {   
        public ActionResult Prize()
        {
            //if (Request.UserAgent.ToLower().Contains("micromessenger"))
            //{
            //    IsWx = "1";
            //}
            //else
            //{
            //    IsWx = "0";
            //}

            ViewData["IsWx"] = IsWx;
            //if (IsWx=="1")
            //{
            //    try
            //    {
            //        WXVariousApi VariousApi = new WXVariousApi();

            //        VariousApi.LoadWxConfigIncidentalAccess_token();
            //        string nonceStr = WXVariousApi.GenerateNonceStr();
            //        string timestamp = WXVariousApi.GenerateTimeStamp();
            //        ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
            //        ViewData["nonceStr"] = nonceStr;
            //        ViewData["timestamp"] = timestamp;
            //        ViewData["AppID"] = VariousApi.WxConfig.APPID;

            //        ReGetOpenId(VariousApi.WxConfig.APPID, VariousApi.WxConfig.APPSECRET);

            //        Session["ACCESS_TOKEN"] = VariousApi.WxConfig.ACCESS_TOKEN;
            //    }
            //    catch(Exception ex)
            //    {
            //        DAL.Log.Instance.Write(ex.Message,"转盘抽奖_微信配置错误");
            //    }
            //}

            LotteryActivitys activity = LotteryPrizes.GetEntityByPage(2);
            if (activity == null)
            {   
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            else
            {   
                ViewData["activity"] = activity;
                return View();
            }
        }

        private string ReGetOpenId(string APPID, string AAPPSECRET)
        {
            string UserOpenId = "";
            try
            {
                
                WXVariousApi VariousApi = new WXVariousApi();
                string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;//获取当前url
                if (Session["openid"] == null || Session["openid"].ToString() == "")
                {
                    //先要判断是否是获取code后跳转过来的
                    if (System.Web.HttpContext.Current.Request.QueryString["code"] == "" || System.Web.HttpContext.Current.Request.QueryString["code"] == null)
                    {
                        // Code为空时，先获取Code
                        string GetCodeUrls = VariousApi.GetCodeUrl(url, APPID);
                        System.Web.HttpContext.Current.Response.Redirect(GetCodeUrls);  // 先跳转到微信的服务器，取得code后会跳回来这页面的
                    }
                    else
                    {
                        // Code非空，已经获取了code后跳回来啦，现在重新获取openid
                        string openid = "";
                        openid = VariousApi.GetOauthAccessOpenId(System.Web.HttpContext.Current.Request.QueryString["Code"], APPID, AAPPSECRET);// 重新取得用户的openid
                        
                        Session["openid"] = openid;
                        UserOpenId = openid;
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Write("Error：" + ex.Message, "微信获取信息错误");
            }

            return UserOpenId;
        }

        /// <summary>
        /// 通过OpenID获取微信用户信息
        /// </summary>
        /// <returns></returns>
        private WXUserInfo GetUserInfo(string openid, string ACCESS_TOKEN)
        {
            try
            {
                WxData data = new WxData();
                data.SetValue("access_token", ACCESS_TOKEN);
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
                DAL.Log.Instance.Write(ex.ToString(), "获取微信用户信息错误");
                throw new Exception(ex.ToString());
            }
        }

        public ActionResult PrizeRec()
        {
            string IsWx = "";
            if (Request.UserAgent.ToLower().Contains("micromessenger"))
            {
                IsWx = "1";
            }
            else
            {
                IsWx = "0";
            }

            ViewData["IsWx"] = IsWx;
            if (IsWx == "1")
            {
                WXVariousApi VariousApi = new WXVariousApi();

                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;

                ReGetOpenId(VariousApi.WxConfig.APPID, VariousApi.WxConfig.APPSECRET);

                Session["ACCESS_TOKEN"] = VariousApi.WxConfig.ACCESS_TOKEN;
            }


            return View();
        }
        public ActionResult LoadOutPrizeList(string WxNo)
        {
            string where = string.Empty;
            string UserOpenId = "";
            if (Session["openid"] != null)
            {
                UserOpenId = Session["openid"].ToString();
                where = " and UserOpenId='" + UserOpenId + "'";
            }
            else
            {
                where = " and UserOpenId='000'";
            }

            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.strForm = string.Format(@"( select * from dbo.LotteryRecord where IsNot=0 {0} ) as Show", where);
            page.strSelect = " * ";
            page.strWhere = "";
            page.strOrder = "Dat desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        // 找回奖品
        public ContentResult RetrievePrizes(string SecurityCode)
        {
            string ResultStr = "";

            try
            {
                WXUserInfo userInfo = new WXUserInfo();
                string UserOpenId = "";
                if (Session["openid"] != null)
                {
                    UserOpenId = Session["openid"].ToString();
                    string ACCESS_TOKEN = Session["ACCESS_TOKEN"].ToString();

                    WXVariousApi VariousApi = new WXVariousApi();
                    userInfo = GetUserInfo(UserOpenId, ACCESS_TOKEN);
                }

                LotteryRecord lotteryRecord = LotteryRecord.GetRecByIntegralCode(SecurityCode);
                if (lotteryRecord != null && !lotteryRecord.IsNot)
                {
                    if (lotteryRecord.UserOpenId == "12345" || lotteryRecord.UserOpenId=="")
                    {
                        lotteryRecord.UserOpenId = Session["openid"].ToString();
                        lotteryRecord.UserWxName = userInfo.nickname;
                        lotteryRecord.UserWxImg = userInfo.headimgurl;

                        int iRet = lotteryRecord.UpdateByID();
                        if (iRet > 0) ResultStr = "ok";
                        else ResultStr = "找回奖品失败，请联系管理员！";
                    }
                    else
                    {
                        ResultStr = "此防伪码不参与奖品找回！";
                    }
                }
                else
                {
                    ResultStr = "此防伪码没有中奖！";
                }
            }
            catch (Exception ex)
            {
                ResultStr = "找回奖品异常！error:" + ex.Message;
            }

            return Content(ResultStr);
        }

        /// <summary>
        /// 读取应付返利列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private ActionResult GetOutRebateListPage(LotteryRecordSearch condition, string where)
        {
            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@"( select * from dbo.LotteryRecord where 1=1 {0} ) as Show", where);
            page.strSelect = " * ";
            page.strWhere = "";
            page.strOrder = "Dat desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取奖品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetPrize(int id)
        {
            List<LotteryPrizes> prizes = LotteryPrizes.GetPrizesByActivityID(id);
            if (prizes != null && prizes.Count > 0)
            {
                return Json(prizes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="fwm"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public ActionResult toPrizeDraw(int ActivityID, string fwm, string Phone,string Name)
        {
            //if (string.IsNullOrWhiteSpace(Phone))
            //{
            //    return Content("fail|手机号不能为空");
            //}
            //if (Phone.Length != 11)
            //{
            //    return Content("fail|手机号有误");
            //}
            //if (string.IsNullOrWhiteSpace(Name))
            //{
            //    return Content("fail|姓名不能为空");
            //}

            WXUserInfo userInfo = new WXUserInfo();
            //string UserOpenId = "";
            //if (Session["openid"] != null)
            //{
                //UserOpenId = Session["openid"].ToString();
                //string ACCESS_TOKEN = Session["ACCESS_TOKEN"].ToString();



                //userInfo=GetUserInfo(UserOpenId, ACCESS_TOKEN);
            //}

            WXVariousApi VariousApi = new WXVariousApi();
            string UserOpenId = ReGetOpenId(VariousApi.WxConfig.APPID, VariousApi.WxConfig.ACCESS_TOKEN);
            userInfo = VariousApi.GetUserInfo(UserOpenId);

            PrizeAttr drawrtn = LotteryPrizes.toPrizeDraw(fwm, ActivityID, Phone, Name, UserOpenId, userInfo);
            return Json(drawrtn, JsonRequestBehavior.AllowGet);
        }

        public ContentResult UploadPrizeInfo(int ID, string Phone, string Name,string Address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Phone))
                {
                    return Content("手机号不能为空");
                }
                if (Phone.Length != 11)
                {
                    return Content("手机号有误");
                }
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return Content("姓名不能为空");
                }
                if (string.IsNullOrWhiteSpace(Address))
                {
                    return Content("姓名不能为空");
                }

                LotteryRecord rec = LotteryRecord.GetEntityByID(ID);
                rec.Phone = Phone;
                rec.Name = Name;
                rec.RecAddress = Address;

                rec.UpdateByID();

                return Content("ok");
            }
            catch(Exception ex)
            {
                return Content("提交出错，请联系客服！");
            }
        }
    }
}
