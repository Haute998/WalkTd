using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class LotteryActivitysAreaRedPackPriceSearch:BaseSearch
    {
        public int LAARP_ID { get; set; }
        public string keyword { get; set; }
    }
    public partial class LotteryActivitysAreaRedPackPrice
    {
        public int fieldGetCntByLAARPP_ID { get; set; }
        public int fieldGetCntByAreaID { get; set; }
        public string AreaName { get; set; }

        public static List<LotteryActivitysAreaRedPackPrice> GetEntitysByLAARP_ID__(int laarp_id, int AreaID)
        {
            string strSql = @"SELECT ID,LAARP_ID,MaxPrice,MinPrice,Rate,
                            (select count(1) from SYSIntegralCode where LAARPP_ID=ID) as fieldGetCntByLAARPP_ID
                            ,(select count(1) from SYSIntegralCode where AreaID=@AreaID) as fieldGetCntByAreaID
                            FROM [LotteryActivitysAreaRedPackPrice] 
                            WHERE LAARP_ID=@LAARP_ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@LAARP_ID", laarp_id),
                                                             new System.Data.SqlClient.SqlParameter("@AreaID", AreaID)};

            return DAL.EntityDataHelper.FillData2Entities<LotteryActivitysAreaRedPackPrice>(strSql, paramters);
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
            strSql = string.Format("DELETE FROM [LotteryActivitysAreaRedPackPrice] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }

        public static List<LotteryActivitysAreaRedPackPrice> GetAllRadParam()
        {
            string strSql = "SELECT a.ID, a.LAARP_ID, a.MaxPrice, a.MinPrice, a.Rate, a.shiwu, b.AreaName " +
                          "FROM dbo.LotteryActivitysAreaRedPackPrice AS a LEFT OUTER JOIN dbo.SYSIntegralCodeArea AS b ON a.LAARP_ID = b.ID";

            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<LotteryActivitysAreaRedPackPrice>(strSql, paramters);
        }
    }
}
