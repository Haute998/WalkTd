using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class RedPackShare
    {
        /// <summary>
        /// 根据Ticket 查询出类别
        /// </summary>
        /// <param name="RedTicket"></param>
        /// <returns></returns>
        public static RedPackShare GetEntityByTicket(string RedTicket)
        {
            string strSql = "SELECT ID,Code,RedCnt,ReceiveCnt,UserName,RedTicket FROM [RedPackShare] WHERE RedTicket=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", RedTicket) };

            return DAL.EntityDataHelper.LoadData2Entity<RedPackShare>(strSql, paramters);
        }

        /// <summary>
        /// 根据主键获取对象
        /// （当数据库表第一行是int自增主键时生成此方法）
        /// </summary>
        public static RedPackShare GetEntityByCode(string Code)
        {
            string strSql = "SELECT ID,Code,RedCnt,ReceiveCnt,UserName,RedTicket FROM [RedPackShare] WHERE Code=@Code";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Code", Code) };

            return DAL.EntityDataHelper.LoadData2Entity<RedPackShare>(strSql, paramters);
        }
        public static RedPackShare GetEntityByUserName(string UserName)
        {
            string strSql = "SELECT ID,Code,RedCnt,ReceiveCnt,UserName,RedTicket FROM [RedPackShare] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<RedPackShare>(strSql, paramters);
        }
    }
}
