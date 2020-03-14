using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_UserMail
    {
        public static C_UserMail GetIsNow(string username)
        {
            string strSql = "SELECT top 1 * FROM [C_UserMail] WHERE UserName=@username and IsNow=1 order by id desc";
            System.Data.SqlClient.SqlParameter[] paramters ={
                 new System.Data.SqlClient.SqlParameter("@username",username)};

            return DAL.EntityDataHelper.LoadData2Entity<C_UserMail>(strSql, paramters);
        }
        /// <summary>
        /// 标为当前地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string ToIsNowByID(int id,string username)
        {
            try
            {
                string strSql = "UPDATE [C_UserMail] SET IsNow=1 WHERE ID=@ID;";
                System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@username",username)
                 };
                DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);


                strSql = "UPDATE [C_UserMail] SET IsNow=0 WHERE ID<>@ID and UserName=@username;";
                DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
                return "ok";
            }
            catch (Exception ex) 
            {
                DAL.Log.Instance.Write(ex.ToString(), "ToIsNowByID_error");
                return "网络异常";
            }
        }
    }
}
