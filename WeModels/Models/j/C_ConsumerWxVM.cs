using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_ConsumerWxVM
    {

        /// <summary>
        /// 用户信息
        /// </summary>
        public C_Consumer user { get; set; }
        /// <summary>
        /// 用户对应的微信信息
        /// </summary>
        public C_UserWxInfo userWxInfo { get; set; }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public void LoadUserVMByUserName(string userName)
        {
            user = EntityDataHelper.LoadData2Entity<C_Consumer>("SELECT top 1 * FROM [C_Consumer] where UserName=@userName", userName);
            userWxInfo = EntityDataHelper.LoadData2Entity<C_UserWxInfo>("SELECT top 1 * FROM [C_UserWxInfo] where C_ConsumerUserName=@UserName", user.UserName);
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
                user = EntityDataHelper.LoadData2Entity<C_Consumer>("SELECT top 1 * FROM [C_Consumer] where UserName=@C_ConsumerUserName", userWxInfo.C_ConsumerUserName);
            }
        }
        /// <summary>
        /// 更新用户微信信息（根据用户ID）
        /// </summary>
        /// <returns></returns>
        public bool UpdateUserWxInfo()
        {
            string strSql = "UPDATE [C_UserWxInfo] SET nickname=@nickname,headimgurl=@headimgurl,groupid=@groupid,accesstoken=@accesstoken,subscribe=@subscribe,country=@country,subscribe_time=@subscribe_time,language=@language WHERE C_ConsumerUserName=@C_ConsumerUserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@nickname", userWxInfo.nickname),
                new System.Data.SqlClient.SqlParameter("@headimgurl", userWxInfo.headimgurl),
                new System.Data.SqlClient.SqlParameter("@groupid", userWxInfo.groupid),
                new System.Data.SqlClient.SqlParameter("@accesstoken", userWxInfo.accesstoken),
                new System.Data.SqlClient.SqlParameter("@C_ConsumerUserName", userWxInfo.C_ConsumerUserName),
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
            userWxInfo.C_ConsumerUserName = user.UserName;
            userWxInfo.InsertAndReturnIdentity();

            return user.UserName;
        }


    }
}
