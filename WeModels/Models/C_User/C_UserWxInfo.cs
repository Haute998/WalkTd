using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_UserWxInfo
    {
        /// <summary>
        /// 根据openid获取
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static C_UserWxInfo GetInfoByOpenid(string openid)
        {
            string strSql = "SELECT * FROM [C_UserWxInfo] WHERE openid=@openid";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@openid", openid) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserWxInfo>(strSql, paramters);
        }
        /// <summary>
        /// 根据C_UserName获取
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static C_UserWxInfo GetInfoByC_UserName(string C_UserName)
        {
            string strSql = "SELECT * FROM [C_UserWxInfo] WHERE C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserWxInfo>(strSql, paramters);
        }

        /// <summary>
        /// 解绑微信
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <returns></returns>
        public static int UnBindWxByC_UserName(string C_UserName)
        {
            string strSql = "UPDATE [C_UserWxInfo] SET C_UserName='' WHERE C_UserName=@C_UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@C_UserName",C_UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 绑定微信
        /// </summary>
        /// <param name="C_UserName"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static int BindWxByC_UserName(string C_UserName,int ID)
        {
            string strSql = "UPDATE [C_UserWxInfo] SET C_UserName=@C_UserName WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@C_UserName",C_UserName),
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 更新基本信息
        /// </summary>
        /// <returns></returns>
        public bool UpdateUserWxInfo()
        {
            string strSql = "UPDATE [C_UserWxInfo] SET nickname=@nickname,headimgurl=@headimgurl,groupid=@groupid,accesstoken=@accesstoken,subscribe=@subscribe,country=@country,subscribe_time=@subscribe_time,language=@language WHERE C_UserName=@C_UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@nickname", nickname),
                new System.Data.SqlClient.SqlParameter("@headimgurl", headimgurl),
                new System.Data.SqlClient.SqlParameter("@groupid", groupid),
                new System.Data.SqlClient.SqlParameter("@accesstoken", accesstoken),
                new System.Data.SqlClient.SqlParameter("@C_UserName", C_UserName),
                new System.Data.SqlClient.SqlParameter("@subscribe", subscribe),
                new System.Data.SqlClient.SqlParameter("@country", country),
                new System.Data.SqlClient.SqlParameter("@subscribe_time",subscribe_time),
                new System.Data.SqlClient.SqlParameter("@language",language)
            };
            return DAL.SqlHelper.ExecuteNonQuery(strSql, paramters) > 0;
        }
    }
}
