using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class SYSIntegralCodeAreaSearch : BaseSearch 
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
    public partial class SYSIntegralCodeArea
    {
        /// <summary>
        /// 条码数
        /// </summary>
        //public int cnt { get; set; }
        public int EditByID()
        {
            string strSql = "UPDATE [SYSIntegralCodeArea] SET AreaName=@AreaName,cnt=@cnt WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@cnt",_cnt),
                new System.Data.SqlClient.SqlParameter("@AreaName",_areaname)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }



        public static SYSIntegralCodeArea GetEntitysl(int AreaID)
        {
            string strSql = "SELECT cnt FROM [SYSIntegralCodeArea] WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", AreaID) };

            return DAL.EntityDataHelper.LoadData2Entity<SYSIntegralCodeArea>(strSql, paramters);
        }

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
            strSql = string.Format("DELETE FROM [SYSIntegralCodeArea] WHERE ID in ({0});UPDATE [SYSIntegralCode] SET AreaID=0 WHERE AreaID in({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
    }
}
