using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeModels;
using WeModels.WxModel;
using WeModels.Models.C_UserModel;

namespace WeBusiness.ApiMobile
{
    [RoutePrefix("ApiMobile/MobileUser")]
    public class MobileUserController : ApiBaseMobileController
    {
        [ApiPermissionFilter]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult Login(string UserName, string Password)
        {
            DAL.Log.Instance.Write("UserName:" + UserName + ",Password:" + Password);

            RequestResult result = new RequestResult();

            C_User user = C_User.GetLoginByPassWord(UserName, Password);
            if (user == null)
            {
                result.message = "用户名或密码错误";
                result.success = false;
                return result;
            }

            string IsUserToken = MobileUserMsg.PDAUserLogin(user.UserName);     // 验证重复登录

            CacheMobileUser muser = new CacheMobileUser();
            muser.UserID = user.ID;
            muser.UserName = user.UserName;
            muser.Timestamp = CommonFunc.GetNowMTimestamp();
            muser.UserToken = string.IsNullOrEmpty(IsUserToken) ? MobileUserMsg.CreateUserToken() : IsUserToken;
            //muser.AuthCodeList = B_RoleRights.GetEntitysByRoleID(user.C_UserTypeID);  // 权限控制

            MobileUserMsg.CacheMobileUserList.Add(muser);

            UserLoginInfo UserInfo = new UserLoginInfo();
            UserInfo.UserToken = muser.UserToken;
            UserInfo.ExpireDate = DateTime.Now.AddHours(20).ToString("yyyy-MM-dd HH:mm:ss");

            result.data = UserInfo;
            result.timestamp = CommonFunc.GetNowTimestamp();
            result.message = "登录成功";
            result.success = true;

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult LoginOut()
        {
            RequestResult result = new RequestResult();
            if (MobileUserMsg.RemoveUser(UserToken))
            {
                result.message = "登出成功";
                result.success = true;
            }
            else
            {
                result.message = "登出失败";
                result.success = false;
            }

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult ModifyPassword(string oldpwd,string newpwd,string twonewpwd)
        {
            RequestResult result = new RequestResult();
            try
            {
                C_User user = C_User.GetUserByUserName(MobileUser.UserName);

                if (newpwd != twonewpwd)
                {
                    result.message = "确认密码不一致";
                    result.success = true;
                }
                else if (user.PassWord != oldpwd)
                {
                    result.message = "原密码不正确";
                    result.success = true;
                }
                else
                {
                    user.PassWord = newpwd;
                    user.UpdateByID();

                    MobileUserMsg.RemoveUser(UserToken);

                    result.message = "成功";
                    result.success = true;
                }
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }
            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUserInfo()
        {
            RequestResult result = new RequestResult();

            C_UserVM user = C_UserVM.GetVMByID(MobileUser.ID);
            result.data = user;
            result.message = "成功";
            result.success = true;

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetDirectSubUserList(int pageindex, int pagesize, string keyword = "")
        {
            RequestResult result = new RequestResult();
            try
            {
                int totalCount = 0;
                List<ShowSubUser> UserList = C_User.GetDirectSubUser(MobileUser.ID, keyword, pagesize, pageindex, out totalCount);

                #region 无用 稳定后删除
                //List<SubUserCount> ListSubUser = new List<SubUserCount>();

                //foreach (C_User user in UserList)
                //{ 
                //    SubUserCount subUser=ListSubUser.Where(u=>u.UserTypeId==user.C_UserTypeID).FirstOrDefault();

                //    if (subUser != null)
                //    {
                //        ShowSubUser showUser = new ShowSubUser();
                //        showUser.ID = user.ID;
                //        showUser.UserName = user.UserName;
                //        showUser.Name = user.Name;
                //        showUser.Phone = user.Phone;
                //        subUser.UserList.Add(showUser);
                //        subUser.Count = subUser.UserList.Count;

                //        ListSubUser.Add(subUser);
                //    }
                //    else
                //    {
                //        SubUserCount newSubUser = new SubUserCount();
                //        newSubUser.UserTypeId = user.C_UserTypeID;
                //        newSubUser.UserTypeName = user.C_UserTypeName;

                //        ShowSubUser showUser = new ShowSubUser();
                //        showUser.ID = user.ID;
                //        showUser.UserName = user.UserName;
                //        showUser.Name = user.Name;
                //        showUser.Phone = user.Phone;
                //        newSubUser.UserList.Add(showUser);
                //        newSubUser.Count = 1;

                //        ListSubUser.Add(newSubUser);
                //    }
                //}
                #endregion

                result.data = UserList;
                result.pages = pageindex;
                result.total = totalCount;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }

            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult GetUserSubType()
        {
            RequestResult result = new RequestResult();
            try
            {
                List<C_UserType_Simple> UserTypeList = C_UserType.GetLowerByUserID(MobileUser.C_UserTypeID);
                result.data = UserTypeList;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }
            return result;
        }

        [ApiMobileOpenFilter(false)]
        [AcceptVerbs("GET", "POST", "OPTIONS")]
        public RequestResult AddUser(int typeid, string name, string contact, string phone, string area, string address)
        {
            RequestResult result = new RequestResult();
            try
            {
                #region 验证
                string msg=string.Empty;
                bool IsOK=VerifyAddInfo(typeid, name, contact, phone, area, address,out msg);
                if (!IsOK)
                {
                    result.message = msg;
                    result.success = false;
                    return result;
                }
                #endregion

                C_User user = new C_User();
                user.C_UserTypeID = typeid;
                user.Name = name;
                user.wxNo = contact;
                user.Phone=phone;

                string[] AreaArray = area.Split(',');
                user.Province = AreaArray[0] != null ? AreaArray[0] : "";
                user.City = AreaArray[1] != null ? AreaArray[1] : "";
                user.Area = AreaArray[2] != null ? AreaArray[2] : "";
                user.WxQRCode = address;
                user.DatPwdChange = DateTime.Now;
                user.DatCreate = DateTime.Now;
                user.IsValid = true;
                user.PassWord = user.Phone.Substring(user.Phone.Length - 6, 6);
                user.Chief = MobileUser.ID;
                user.state = "已审核";
                user.ID = user.InsertAndReturnIdentity();

                C_UserType usertype = C_UserType.GetEntityByLever(typeid);
                int idLen = user.ID.ToString().Length;
                string idDQ = user.ID.ToString();
                if (idLen < 5)
                {
                    string zero = new string('0', 5 - idLen);
                    idDQ = zero + idDQ;
                }
                user.UserName = usertype.TypeCode + idDQ;
                user.Identifier = usertype.TypeCode + idDQ;
                user.UpdateByID();

                result.data = user.ID;
                result.message = "成功";
                result.success = true;
            }
            catch (Exception ex)
            {
                result.message = "失败,error:" + ex.Message;
                result.success = false;
            }
            return result;
        }

        private bool VerifyAddInfo(int typeid, string name, string contact, string phone, string area, string address, out string msg)
        {
            bool IsOK = false;

            if (typeid == 0)
            {
                msg = "请选择经销商级别";
            }
            else if (typeid == MobileUser.C_UserTypeID)
            {
                msg = "经销商级别必需小于你的级别";
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                msg = "经销商名称不能为空";
            }
            else if (string.IsNullOrWhiteSpace(contact))
            {
                msg = "联系人不能为空";
            }
            else if (string.IsNullOrWhiteSpace(phone))
            {
                msg = "电话号码不能为空";
            }
            else if (phone.Length != 11)
            {
                msg = "请输入正确的手机号码";
            }
            else if (string.IsNullOrWhiteSpace(area))
            {
                msg = "请选择所在地区";
            }
            else
            {
                msg = "正确";
                IsOK = true;
            }

            return IsOK;
        }
    }

    public class UserLoginInfo
    {
        public string UserToken { get; set; }
        public string ExpireDate { get; set; }
    }
}
