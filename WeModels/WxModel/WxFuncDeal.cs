using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DAL;

namespace WeModels.WxModel
{
    public class WxFuncDeal
    {
        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static void WXLogin(JsApi jsapi)
        {
            try
            {
                //用户信息，包括微信信息
                C_UserWxVM userVM = new C_UserWxVM();
                Log.Instance.Write(jsapi.openid, "OpenId_Func");

                userVM.LoadUserVMByOpenid(jsapi.openid);

                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                WXUserInfo wx_userinfo = VariousApi.GetUserInfo(jsapi.openid);

                //未关注获取头像
                if (wx_userinfo.subscribe == "0")
                {
                    WXUserInfo new_Wx_UserInfo = VariousApi.GetUserInfo0(jsapi.openid, jsapi.access_token);

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
                    if (wx_userinfo.unionid.Length > 0)
                    {
                        userVM.LoadUserVMByunionid(wx_userinfo.unionid);
                    }
                }

                if (userVM == null || userVM.user == null)
                {
                    userVM.user = new C_WxUser();
                    userVM.userWxInfo = new C_UserWxInfo();
                    //用户信息赋值
                    userVM.user.UserName = "wx" + (C_User.GetTopUseID() + 1 + 1000);
                    userVM.user.PassWord = "";
                    userVM.user.IsValid = true;
                    userVM.user.DatRegister = DateTime.Now;
                    userVM.user.NickName = wx_userinfo.nickname;
                    userVM.user.PortraitUrl = wx_userinfo.headimgurl;
                    userVM.userWxInfo.openid = jsapi.openid;
                    userVM.userWxInfo.accesstoken = jsapi.access_token;
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
                else
                {

                    userVM.user.NickName = wx_userinfo.nickname;
                    userVM.user.PortraitUrl = wx_userinfo.headimgurl;
                    userVM.userWxInfo.openid = jsapi.openid;
                    userVM.userWxInfo.accesstoken = jsapi.access_token;
                    userVM.userWxInfo.nickname = wx_userinfo.nickname;
                    userVM.userWxInfo.sex = wx_userinfo.sex;
                    userVM.userWxInfo.unionid = wx_userinfo.unionid;
                    userVM.userWxInfo.headimgurl = wx_userinfo.headimgurl;
                    userVM.userWxInfo.subscribe = (wx_userinfo.subscribe == "0" ? false : true);
                    userVM.userWxInfo.country = wx_userinfo.country;
                    userVM.userWxInfo.subscribe_time = Common.ConvertToDateTen(wx_userinfo.subscribe_time);
                    userVM.userWxInfo.language = wx_userinfo.language;
                    userVM.user.UpdateByID();
                }
                string error = string.Empty;
                string userName = userVM.user.UserName;
                userVM.userWxInfo.accesstoken = jsapi.access_token;
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
                Common.SetCookie("WxUserName", userName);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "WXLogin_error");
            }
        }
    }
}
