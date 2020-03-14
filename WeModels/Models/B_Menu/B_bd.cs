using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_bd : BaseSearch
    {
        /// <summary>
        /// 多条件
        /// </summary>
        public string keyword { get; set; }
        public int cs { get; set; }
  
         public static B_bd GetEntityBycs(string SmallCode)
        {
            string strSql = "SELECT count(*) as cs FROM [B_bd] WHERE sn=@sn";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@sn", SmallCode) };

            return DAL.EntityDataHelper.LoadData2Entity<B_bd>(strSql, paramters);
        }
         public static B_bd bxq(string SmallCode)
         {
             string strSql = "SELECT dat FROM [B_bd] WHERE sn=@sn";
             System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@sn", SmallCode) };

             return DAL.EntityDataHelper.LoadData2Entity<B_bd>(strSql, paramters);
         }
         public static B_bd login(string phone)
         {
             string strSql = "select * FROM B_BD WHERE phone=@phone";
             System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@phone", phone)
            };

             return DAL.EntityDataHelper.LoadData2Entity<B_bd>(strSql, paramters);
         }
         public static B_bd cha(string SN)
         {
             string strSql = "select phone FROM B_BD WHERE SN=@SN";
             System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@SN", SN)
            };

             return DAL.EntityDataHelper.LoadData2Entity<B_bd>(strSql, paramters);
         }
    }
}
