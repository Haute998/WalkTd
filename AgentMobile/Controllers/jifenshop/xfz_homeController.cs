using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using DAL;

namespace AgentMobile.Controllers
{
    public class xfz_homeController : Controller
    {
        //
        // GET: /xfz_home/


        public ActionResult wxlogin(string url)
        {
            try
            {
                string openid = "";
                string access_token = "";

                if (!string.IsNullOrWhiteSpace(WeConfig.WxDebug))
                {
                    openid = WeConfig.WxDebug;
                    access_token = "";
                }


                else
                {

                    JsApi jsapi = new JsApi(this);

                    string codeUrl = jsapi.GetOpenidAndAccessToken_snsapi_userinfo();
                    if (codeUrl != string.Empty)
                    {
                        return Redirect(codeUrl);
                    }
                    openid = jsapi.openid;
                    access_token = jsapi.access_token;
                }



                //用户信息，包括微信信息
                C_ConsumerWxVM userVM = new C_ConsumerWxVM();
                userVM.LoadUserVMByOpenid(openid);

                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                WXUserInfo wx_userinfo = VariousApi.GetUserInfo(openid);



                if (wx_userinfo == null)
                {
                    wx_userinfo = new WXUserInfo();
                }



                //未关注获取头像
                if (wx_userinfo.subscribe == "0")
                {
                    WXUserInfo new_Wx_UserInfo = VariousApi.GetUserInfo0(openid, access_token);

                    if (new_Wx_UserInfo != null)
                    {
                        wx_userinfo.nickname = new_Wx_UserInfo.nickname;
                        wx_userinfo.sex = new_Wx_UserInfo.sex;
                        wx_userinfo.province = new_Wx_UserInfo.province;
                        wx_userinfo.city = new_Wx_UserInfo.city;
                        wx_userinfo.country = new_Wx_UserInfo.country;
                        wx_userinfo.headimgurl = new_Wx_UserInfo.headimgurl;
                        wx_userinfo.unionid = new_Wx_UserInfo.unionid;
                    }

                }



                if (userVM == null || userVM.user == null)
                {
                    string guidCode = DAL.MD5Helper.GetMD5UTF8(Request.UserHostAddress + "," + Guid.NewGuid().ToString());


                    userVM.user = new C_Consumer();
                    userVM.userWxInfo = new C_UserWxInfo();
                    //用户信息赋值
                    userVM.user.UserName = "wx" + (C_Consumer.GetTopUseID() + 1 + 1000 + guidCode.SubStringSafe(0,4));
                    userVM.user.Pwd = "";
                    userVM.user.DatReg = DateTime.Now;
                    userVM.user.Mobile = "";
                    userVM.user.Type = "消费者";
                    userVM.userWxInfo.openid = openid;
                    userVM.userWxInfo.accesstoken = access_token;
                    userVM.userWxInfo.nickname = wx_userinfo.nickname;
                    userVM.userWxInfo.sex = wx_userinfo.sex;
                    userVM.userWxInfo.unionid = wx_userinfo.unionid;
                    userVM.userWxInfo.headimgurl = wx_userinfo.headimgurl;
                    userVM.userWxInfo.subscribe = (wx_userinfo.subscribe == "0" ? false : true);
                    userVM.userWxInfo.country = wx_userinfo.country;
                    userVM.userWxInfo.subscribe_time = Common.ConvertToDateTen(wx_userinfo.subscribe_time);
                    userVM.userWxInfo.language = wx_userinfo.language;

                    userVM.AddUser();
                    userVM.userWxInfo.C_UserName = userVM.user.UserName;
                }
                userVM.userWxInfo.C_ConsumerUserName = userVM.user.UserName;
                string error = string.Empty;
                string userName = userVM.user.UserName;
                userVM.userWxInfo.accesstoken = access_token;
                userVM.userWxInfo.nickname = wx_userinfo.nickname;
                userVM.userWxInfo.groupid = wx_userinfo.groupid;
                userVM.userWxInfo.headimgurl = wx_userinfo.headimgurl;
                userVM.userWxInfo.subscribe = (wx_userinfo.subscribe == "0" ? false : true);
                userVM.userWxInfo.country = wx_userinfo.country;
                userVM.userWxInfo.subscribe_time = Common.ConvertToDateTen(wx_userinfo.subscribe_time);
                userVM.userWxInfo.language = wx_userinfo.language;


                if (!string.IsNullOrWhiteSpace(userVM.userWxInfo.nickname))
                {
                    new System.Threading.Thread(delegate()
                    {
                        userVM.UpdateUserWxInfo();
                    }).Start();
                }
                Session["xfz_UserName"] = userName;
                return Redirect(HttpUtility.UrlDecode(url));
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "WXLogin_error");
                return View(ErrorPage.ViewName, new ErrorPage { Message = ex.ToString() });
            }
        }

    }
}
