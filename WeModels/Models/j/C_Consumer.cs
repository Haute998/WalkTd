using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_ConsumerSearch : BaseSearch 
    {
        public string keyword { get; set; }
    }
    public partial class C_Consumer
    {
        /// <summary>
        /// 微信昵称
        /// </summary>
        public string wx_name { get; set; }
        //微信地址
        public string wx_address { get; set; }
        /// <summary>
        /// 微信性别
        /// </summary>
        public string wx_sex { get; set; }

        public string wx_headurl { get; set; }

        public string pwdconfirm { get; set; }

        public static int GetTopUseID()
        {
            string strSql = "SELECT ISNULL(MAX(ID),1000) FROM [C_Consumer]";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object TopUseID = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return TopUseID == null ? 0 : Convert.ToInt32(TopUseID);
        }


        public static C_Consumer GetEntityByUserName(string UserName)
        {
            string strSql = "SELECT * FROM [C_Consumer] WHERE UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_Consumer>(strSql, paramters);
        }


        public static C_Consumer GetEntityByUserNamepwd(string UserName,string pwd)
        {
            string strSql = "SELECT * FROM [C_Consumer] WHERE UserName=@UserName and Pwd=@Pwd";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                             new System.Data.SqlClient.SqlParameter("@Pwd", pwd)};

            return DAL.EntityDataHelper.LoadData2Entity<C_Consumer>(strSql, paramters);
        }

        public static int GetCCount(string type)
        {
            string strSql = "SELECT count(1) FROM [C_Consumer] WHERE  Type='"+type+"'";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }


        /// <summary>
        /// 增加积分
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Integral"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static int addIntegral(string UserName, int Integral, System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "UPDATE [C_Consumer] SET jf+=@jf WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@jf",Integral),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 增加已用积分
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="jf"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public static int addjfused(string UserName, int jf, System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "UPDATE [C_Consumer] SET jf_used+=@jf_used WHERE UserName=@UserName;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@jf_used",jf),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }




        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool ToAudits(int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("UPDATE [C_Consumer] SET Stat='已审核' WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }


    }
}
