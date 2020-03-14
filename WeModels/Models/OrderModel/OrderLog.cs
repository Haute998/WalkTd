using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class OrderLog
    {
        /// <summary>
        /// 日志添加
        /// </summary>
        /// <returns></returns>
        public static void LogAdd(string OrderNo, string LogType, string Logs, string Oper)
        {
            try
            {

                string strSql = "INSERT INTO [OrderLog] (OrderNo,LogType,Logs,Dat,Oper) values (@OrderNo,@LogType,@Logs,@Dat,@Oper);SELECT CAST(scope_identity() AS int);";
                System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@OrderNo",OrderNo),
                new System.Data.SqlClient.SqlParameter("@LogType",LogType),
                new System.Data.SqlClient.SqlParameter("@Logs",Logs),
                new System.Data.SqlClient.SqlParameter("@Dat",DateTime.Now),
                new System.Data.SqlClient.SqlParameter("@Oper",Oper),
            };
                DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "LogAdd_error");
            }
        }
    }
}
