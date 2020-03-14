using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class InStockScale
    {

        /// <summary>
        /// 判断小标是否入库
        /// </summary>
        /// <returns></returns>
        public static bool GetInScale(string code)
        {
            string SqlStr = "select count(*) from Scale where IsInto=1 and ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@SmallCode",code)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 判断大标是否入库
        /// </summary>
        /// <returns></returns>
        public static bool GetBigInScale(string code)
        {
            string SqlStr = "select count(*) from Scale where IsInto=1 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
        public static List<Scale> GetBigScaleList(string code)
        {
            string SqlStr = "select * from Scale where IsInto=1 and ID in(select ScaleId from Scale_Big where BigCode=@BigCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@BigCode",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }
        public static List<Scale> GetSmallScaleList(string code)
        {
            string SqlStr = "select * from Scale where IsInto=1 and ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@SmallCode",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<Scale>(SqlStr, Parameter);
        }
        public static int getcount(string code)
        {
            string strSql = "select count(*) from Scale where IsInto=1 and ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode)";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@SmallCode",code),
               
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        public static int updatecount(string code,string cp)
        {
            string strSql = "update Scale set ProductNo=@ProductNo where IsInto=1 and ID in(select ScaleId from Scale_Small where SmallCode=@SmallCode)";
            
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@SmallCode",code),
                new System.Data.SqlClient.SqlParameter("@ProductNo",cp)
               
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
     
    }
}
