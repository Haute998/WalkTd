using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;
using WeModels.WxModel;
using WeModels.Models.C_UserModel;

namespace WeBusiness.Api
{
    [RoutePrefix("ApiPDA/User")]
    public class UserController : ApiBaseController
    {
        [ApiPermissionFilter]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult Login(string DeviceCode, string UserName, string Password)
        {
            RequestResult result = new RequestResult();
            try
            {
                PDA pda = PDA.GetEntityByKeys(DeviceCode);
                if (pda == null)
                {
                    result.code = 1009;
                    result.message = "该设备验证不通过,禁止访问服务器";
                    result.success = false;

                    PDALog.Write("用户登录", "登录", "", UserName, string.Format("DeviceCode:{0}", DeviceCode), result.message);
                    return result;
                }

                PDAUser user = PDAUser.PDAUserLogin(UserName, Password);
                if (user == null)
                {
                    result.message = "用户名或密码错误";
                    result.success = false;

                    PDALog.Write("用户登录", "登录", "", UserName, string.Format("DeviceCode:{0}", DeviceCode), result.message);
                    return result;
                }

                string IsUserToken = PDAUserMsg.PDAUserLogin(DeviceCode, UserName);     // 验证重复登录

                CachePDAUser muser = new CachePDAUser();
                muser.DeviceCode = DeviceCode;
                muser.UserID = user.ID;
                muser.UserName = user.PUserName;
                muser.Timestamp = CommonFunc.GetNowMTimestamp();
                muser.UserToken = string.IsNullOrEmpty(IsUserToken) ? PDAUserMsg.CreateUserToken() : IsUserToken;
                //muser.AuthCodeList = B_RoleRights.GetEntitysByRoleID(user.C_UserTypeID);  // 权限控制

                PDAUserMsg.CacheMobileUserList.Add(muser);

                UserLoginInfo UserInfo = new UserLoginInfo();
                UserInfo.UserToken = muser.UserToken;
                UserInfo.ExpireDate = DateTime.Now.AddHours(20).ToString("yyyy-MM-dd HH:mm:ss");

                result.data = UserInfo;
                result.timestamp = CommonFunc.GetNowTimestamp();
                result.message = "登录成功";
                result.success = true;

                user.LastLoginTime = DateTime.Now;
                user.LastLoginDCode = DeviceCode;
                user.UpdateByID();

                PDALog.Write("用户登录", "登录", "", user.PUserName + "-" + user.PRealName, string.Format("DeviceCode:{0}", DeviceCode), result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("PDA登录出错：" + ex.Message, "PDA登录出错");
            }

            return result;
        }

        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult LoginOut()
        {
            RequestResult result = new RequestResult();
            try
            {
                if (PDAUserMsg.RemoveUser(UserToken))
                {
                    result.message = "登出成功";
                    result.success = true;
                }
                else
                {
                    result.message = "登出失败";
                    result.success = false;
                }

                PDALog.Write("退出登录", "退出", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("PDA用户退出出错：" + ex.Message, "PDA用户退出出错");
            }

            return result;
        }
        
        [ApiOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUserInfo()
        {
            RequestResult result = new RequestResult();
            try
            {
                result.data = PdaUser;
                result.message = "成功";
                result.success = true;

                PDALog.Write("获取用户信息", "获取", "", PdaUser.PUserName + "-" + PdaUser.PRealName, "", result.message);
            }
            catch (Exception ex)
            {
                result.code = 500;
                result.message = "服务出错";
                result.success = false;
                DAL.Log.Instance.Write("获取用户信息出错：" + ex.Message, "PDA上传出错");
            }

            return result;
        }
    }

    public class UserLoginInfo
    {
        public string UserToken { get; set; }
        public string ExpireDate { get; set; }
    }
}
