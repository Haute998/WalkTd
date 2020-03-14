using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class SYSCompanyProfile
    {
        /// <summary>
        /// 获取简介
        /// </summary>
        /// <returns></returns>
        public static SYSCompanyProfile GetProfile()
        {
            string strSql = "select top 1 * FROM [SYSCompanyProfile] order by id desc";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.LoadData2Entity<SYSCompanyProfile>(strSql, paramters);
        }

        public static int SaveTmp(string Tmp)
        {
            string strSql = "UPDATE [SYSCompanyProfile] SET Tmp=@Tmp";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@Tmp",Tmp)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
