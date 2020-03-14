using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/

        /// <summary>
        /// 绑定微信
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult WXBind(string url)
        {
            try
            {
                JsApi jsapi = new JsApi(this);

                string codeUrl = jsapi.GetOpenidAndAccessToken_snsapi_userinfo();
                if (codeUrl != string.Empty)
                {
                    return Redirect(codeUrl);
                }

                
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                WXUserInfo wx_userinfo = VariousApi.GetUserInfo(jsapi.openid);

                C_UserWxInfo.UnBindWxByC_UserName(CurrentUser.UserName);//解绑微信

                //用户信息，包括微信信息
                C_UserWxInfo wxInfo = C_UserWxInfo.GetInfoByOpenid(jsapi.openid);
                if (wxInfo == null)
                {
                    wxInfo = new C_UserWxInfo();
                    wxInfo.C_UserName = CurrentUser.UserName;
                    wxInfo.openid = jsapi.openid;
                    wxInfo.accesstoken = jsapi.access_token;
                    wxInfo.nickname = wx_userinfo.nickname;
                    wxInfo.sex = wx_userinfo.sex;
                    wxInfo.unionid = wx_userinfo.unionid;
                    wxInfo.headimgurl = wx_userinfo.headimgurl;
                    wxInfo.subscribe = (wx_userinfo.subscribe == "0" ? false : true);
                    wxInfo.country = wx_userinfo.country;
                    wxInfo.subscribe_time = Common.ConvertToDateTen(wx_userinfo.subscribe_time);
                    wxInfo.language = wx_userinfo.language;
                    wxInfo.ID=wxInfo.InsertAndReturnIdentity();//新的ID
                }
                else 
                {
                    C_UserWxInfo.BindWxByC_UserName(CurrentUser.UserName, wxInfo.ID);
                }



                return Redirect(HttpUtility.UrlDecode(url));
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "WXBind_error");
                return View(ErrorPage.ViewName, new ErrorPage { Message = "绑定微信失败"});
            }
        }

    }
}
