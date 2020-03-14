using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SYSAgentKnow
    {
        /// <summary>
        /// 获取代理须知页面
        /// </summary>
        /// <returns></returns>
        public static List<SYSAgentKnow> GetNavAdvs()
        {
            string strSql = "SELECT TOP 100000 * FROM [SYSAgentKnow]  order by sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSAgentKnow>(strSql, paramters);
        }
        /// <summary>
        /// 获取手机版
        /// </summary>
        /// <returns></returns>
        public static List<SYSAgentKnow> GetNavAdvsMobile()
        {
            string strSql = "SELECT TOP 100000 * FROM [SYSAgentKnow] where IsShow='True'  order by sort";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<SYSAgentKnow>(strSql, paramters);
        }

    }
}
