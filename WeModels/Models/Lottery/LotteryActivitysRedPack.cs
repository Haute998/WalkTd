using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class LotteryActivitysRedPack
    {
        public string AreaName { get; set; }
        public static List<LotteryActivitysRedPack> GetAllRadParam()
        {
            string strSql = "SELECT a.ID, a.AreaID, a.MaxPrice, a.MinPrice, a.Rate, b.AreaName,a.CreateTime "+
                            "FROM dbo.LotteryActivitysRedPack AS a LEFT OUTER JOIN dbo.SYSIntegralCodeArea AS b ON a.AreaID = b.ID ";

            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<LotteryActivitysRedPack>(strSql, paramters);
        }

        public static List<LotteryActivitysRedPack> GetEntityActArea(int ActivityID, int AreaID)
        {
            string strSql = "SELECT * FROM [LotteryActivitysRedPack] WHERE ActivityID=@ActivityID and AreaID=@AreaID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ActivityID", ActivityID),
                                                             new System.Data.SqlClient.SqlParameter("@AreaID", AreaID)};

            return DAL.EntityDataHelper.FillData2Entities<LotteryActivitysRedPack>(strSql, paramters);
        }
    }
}
