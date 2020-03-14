using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace WeModels
{
    /// <summary>
    /// 用户和微信信息视图
    /// </summary>
    public class C_UserWxVM
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public C_WxUser user { get; set; }
        /// <summary>
        /// 用户对应的微信信息
        /// </summary>
        public C_UserWxInfo userWxInfo { get; set; }

        /// <summary>
        /// 根据用户ID获取用户
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public void LoadUserVMByC_UserID(int C_UserID)
        {
            userWxInfo = EntityDataHelper.LoadData2Entity<C_UserWxInfo>("SELECT top 1 * FROM [C_UserWxInfo] where C_UserID=@C_UserID", C_UserID);
            user = EntityDataHelper.LoadData2Entity<C_WxUser>("SELECT top 1 * FROM [C_WxUser] where ID=@C_UserID", C_UserID);
        }


        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public void LoadUserVMByUserName(string userName)
        {
            user = EntityDataHelper.LoadData2Entity<C_WxUser>("SELECT top 1 * FROM [C_WxUser] where UserName=@userName", userName);
            userWxInfo = EntityDataHelper.LoadData2Entity<C_UserWxInfo>("SELECT top 1 * FROM [C_UserWxInfo] where C_UserName=@UserName", user.UserName);
        }

        /// <summary>
        /// 根据微信openid获取用户
        /// </summary>
        /// <param name="openid"></param>
        public void LoadUserVMByOpenid(string openid)
        {
            userWxInfo = EntityDataHelper.LoadData2Entity<C_UserWxInfo>("SELECT top 1 * FROM [C_UserWxInfo] where openid=@openid", openid);
            if (userWxInfo != null)
            {
                user = EntityDataHelper.LoadData2Entity<C_WxUser>("SELECT top 1 * FROM [C_WxUser] where UserName=@C_UserName", userWxInfo.C_UserName);
            }
        }
        /// <summary>
        /// 更新用户微信信息（根据用户ID）
        /// </summary>
        /// <returns></returns>
        public bool UpdateUserWxInfo()
        {
            string strSql = "UPDATE [C_UserWxInfo] SET nickname=@nickname,headimgurl=@headimgurl,groupid=@groupid,accesstoken=@accesstoken,subscribe=@subscribe,country=@country,subscribe_time=@subscribe_time,language=@language WHERE C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@nickname", userWxInfo.nickname),
                new System.Data.SqlClient.SqlParameter("@headimgurl", userWxInfo.headimgurl),
                new System.Data.SqlClient.SqlParameter("@groupid", userWxInfo.groupid),
                new System.Data.SqlClient.SqlParameter("@accesstoken", userWxInfo.accesstoken),
                new System.Data.SqlClient.SqlParameter("@C_UserName", userWxInfo.C_UserName),
                new System.Data.SqlClient.SqlParameter("@subscribe", userWxInfo.subscribe),
                new System.Data.SqlClient.SqlParameter("@country", userWxInfo.country),
                new System.Data.SqlClient.SqlParameter("@subscribe_time", userWxInfo.subscribe_time),
                new System.Data.SqlClient.SqlParameter("@language", userWxInfo.language)
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        public string AddUser()
        {
            user.InsertAndReturnIdentity();
            userWxInfo.C_UserName = user.UserName;
            userWxInfo.InsertAndReturnIdentity();

            //开通钱包
            C_UserWallet wallet = new C_UserWallet();
            wallet.UserName = user.UserName;
            wallet.Money = 0;
            wallet.InsertAndReturnIdentity();
            return user.UserName;
        }


        /// <summary>
        /// 更新关注状态
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="isSubscribe">是否关注</param>
        /// <returns></returns>
        public static int UpdateSubscribe(string openID, bool isSubscribe)
        {
            string strSql = "";
            if (isSubscribe == true)
            {
                strSql = "UPDATE [C_UserWxInfo] SET subscribe=@subscribe,subscribe_time=getdate() WHERE openid=@openid;";
            }
            else
            {
                strSql = "UPDATE [C_UserWxInfo] SET subscribe=@subscribe WHERE openid=@openid;";
            }
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@openid",openID),
                new System.Data.SqlClient.SqlParameter("@subscribe",isSubscribe)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 根据微信openid获取用户
        /// </summary>
        /// <param name="openid"></param>
        public void LoadUserVMByunionid(string unionid)
        {
            try
            {
                userWxInfo = EntityDataHelper.LoadData2Entity<C_UserWxInfo>("SELECT top 1 * FROM [C_UserWxInfo] where unionid=@unionid", unionid);
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString() + "1", "userVM");
            }
            try
            {
                if (userWxInfo != null)
                {
                    user = EntityDataHelper.LoadData2Entity<C_WxUser>("SELECT top 1 * FROM [C_WxUser] where UserName=@C_UserName", userWxInfo.C_UserName);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "userVM");
            }
        }
    }
}
