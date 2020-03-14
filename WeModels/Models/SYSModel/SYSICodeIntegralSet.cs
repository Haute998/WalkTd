using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSICodeIntegralSetSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }


        public static string StrWhere(SYSICodeIntegralSetSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (CodePrefix like '%{0}%')", condition.keyword);
            }
            return where;
        }
    }
    public partial class SYSICodeIntegralSet
    {
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static bool ToDels(int[] ids)
        {
            string idsSql = string.Empty;
            foreach (int i in ids)
            {
                idsSql += i + ",";
            }
            idsSql = idsSql.TrimEnd(',');
            string strSql = string.Empty;
            strSql = string.Format("DELETE FROM [SYSICodeIntegralSet] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
        /// <summary>
        /// 根据前缀获取数据
        /// </summary>
        /// <param name="CodePrefix"></param>
        /// <returns></returns>
        public static SYSICodeIntegralSet GetEntityByCodePrefix(string CodePrefix)
        {
            string strSql = "SELECT * FROM [SYSICodeIntegralSet] WHERE CodePrefix=@CodePrefix";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@CodePrefix", CodePrefix) };

            return DAL.EntityDataHelper.LoadData2Entity<SYSICodeIntegralSet>(strSql, paramters);
        }
    }
}
