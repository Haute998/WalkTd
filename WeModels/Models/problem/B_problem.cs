using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_problem: BaseSearch
    {
        /// <summary>
        /// 多条件
        /// </summary>
        public string keyword { get; set; }
        public static List<B_problem> GetEntitysAllc()
        {
            string strSql = "SELECT TOP 100000 ID,problem,lag FROM [B_problem] where lag='c'";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<B_problem>(strSql, paramters);
        }
        public static List<B_problem> GetEntitysAlle()
        {
            string strSql = "SELECT TOP 100000 ID,problem,lag FROM [B_problem] where lag='e'";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<B_problem>(strSql, paramters);
        }
    }
}
