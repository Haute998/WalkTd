using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class RepeatHelper
    {
        /// <summary>
        /// 数据去重（一个值去重）
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="attr">字段名</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public static int NoRepeat(string table, string attr, string val, int ID)
        {
            string strSql = string.Format("select count(1) from [{0}] where {1}=@val and ID<>@ID", Common.Filter(table), Common.Filter(attr));
            System.Data.SqlClient.SqlParameter[] paramters = { 
              new System.Data.SqlClient.SqlParameter("@val", val),
              new System.Data.SqlClient.SqlParameter("@ID", ID)};
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }
        public static int NoRepeatThreeAnd(string table, string attr, string val, string attr2, string val2, string attr3, string val3, int ID)
        {
            string strSql = string.Format("select count(1) from [{0}] where {1}=@val and {2}=@val2 and {3}=@val3 and ID<>@ID", Common.Filter(table), Common.Filter(attr), Common.Filter(attr2), Common.Filter(attr3));
            System.Data.SqlClient.SqlParameter[] paramters = { 
              new System.Data.SqlClient.SqlParameter("@val", val),
              new System.Data.SqlClient.SqlParameter("@val2", val2),
                new System.Data.SqlClient.SqlParameter("@val3", val3),
              new System.Data.SqlClient.SqlParameter("@ID", ID)};
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }
        /// <summary>
        /// 数据去重（两个值去重）【and】
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="attr">字段名1</param>
        /// <param name="val">值1</param>
        /// <param name="attr2">字段2</param>
        /// <param name="val2">值2</param>
        /// <returns></returns>
        public static int NoRepeatTwoAnd(string table, string attr, string val, string attr2, string val2, int ID)
        {
            string strSql = string.Format("select count(1) from [{0}] where {1}=@val and {2}=@val2 and ID<>@ID", Common.Filter(table), Common.Filter(attr), Common.Filter(attr2));
            System.Data.SqlClient.SqlParameter[] paramters = { 
              new System.Data.SqlClient.SqlParameter("@val", val),
              new System.Data.SqlClient.SqlParameter("@val2", val2),
              new System.Data.SqlClient.SqlParameter("@ID", ID)};
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }

        /// <summary>
        /// 数据去重（两个值去重）【or】
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="attr">字段名1</param>
        /// <param name="val">值1</param>
        /// <param name="attr2">字段2</param>
        /// <param name="val2">值2</param>
        /// <returns></returns>
        public static int NoRepeatTwoOr(string table, string attr, string val, string attr2, string val2, int ID)
        {
            string strSql = string.Format("select count(1) from [{0}] where ({1}=@val or {2}=@val2) and ID<>@ID", Common.Filter(table), Common.Filter(attr), Common.Filter(attr2));
            System.Data.SqlClient.SqlParameter[] paramters = { 
              new System.Data.SqlClient.SqlParameter("@val", val),
              new System.Data.SqlClient.SqlParameter("@val2", val2),
              new System.Data.SqlClient.SqlParameter("@ID", ID)};
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }
    }
}
