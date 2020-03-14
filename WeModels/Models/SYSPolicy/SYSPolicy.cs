using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SYSPolicy
    {
        /// <summary>
        /// 获取代理须知页面
        /// </summary>
        /// <returns></returns>
        public static List<SYSPolicy> GetNavAdvs()
        {
            string strSql = "SELECT TOP 100000 * FROM [SYSPolicy]  order by sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSPolicy>(strSql, paramters);
        }

        public static List<SYSPolicy> GetNavAdvsMobile()
        {
            string strSql = "SELECT TOP 100000 * FROM [SYSPolicy] where IsShow='True' order by sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSPolicy>(strSql, paramters);
        }
    }
}
