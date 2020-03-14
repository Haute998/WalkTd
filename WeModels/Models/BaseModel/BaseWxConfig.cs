using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class BaseWxConfig
    {
        /// <summary>
        /// 根据ID将当前微信配置设为活动状态
        /// </summary>
        /// <param name="ID">微信配置ID</param>
        /// <returns></returns>
        public static int IsActiveByID(int ID)
        {
            string strSql = "UPDATE [BaseWxConfig] SET Active=(case when ID=@ID then 1 else 0 end) where EXISTS(SELECT ID FROM [BaseWxConfig] WHERE ID=@ID)";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",ID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 读取首条为活动状态的微信配置（ACCESS_TOKEN并不是最新的，不需要用到ACCESS_TOKEN时可直接调用此方法）
        /// </summary>
        /// <returns></returns>
        public static BaseWxConfig GetWxConfig()
        {
            string strSql = "SELECT top 1 * FROM [BaseWxConfig] WHERE IsActive=1";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.LoadData2Entity<BaseWxConfig>(strSql, paramters);
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = @"UPDATE [BaseWxConfig] SET APPID=@APPID,APPSECRET=@APPSECRET,MchID=@MchID,PayKey=@PayKey,NotifyUrl=@NotifyUrl,IsActive=1,ACCESS_TOKEN='' WHERE ID=@ID;
            UPDATE [BaseWxConfig] SET IsActive=0 where ID<>@ID
            ";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@APPID",_appid),
                new System.Data.SqlClient.SqlParameter("@APPSECRET",_appsecret),
                new System.Data.SqlClient.SqlParameter("@MchID",_mchid),
                new System.Data.SqlClient.SqlParameter("@PayKey",_paykey),
                new System.Data.SqlClient.SqlParameter("@NotifyUrl",_notifyurl)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

    }
}
