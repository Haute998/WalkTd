using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class LotteryActivitysAreaRedPack
    {
        public static LotteryActivitysAreaRedPack GetEntityActArea(int ActivityID, int AreaID)
        {
            string strSql = "SELECT * FROM [LotteryActivitysAreaRedPack] WHERE ActivityID=@ActivityID and AreaID=@AreaID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ActivityID", ActivityID),
                                                             new System.Data.SqlClient.SqlParameter("@AreaID", AreaID)};

            return DAL.EntityDataHelper.LoadData2Entity<LotteryActivitysAreaRedPack>(strSql, paramters);
        }


        public static LotteryActivitysAreaRedPack GetEntityActivityID(int ActivityID)
        {
            string strSql = "SELECT * FROM [LotteryActivitysAreaRedPack] WHERE ActivityID=@ActivityID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ActivityID", ActivityID) };

            return DAL.EntityDataHelper.LoadData2Entity<LotteryActivitysAreaRedPack>(strSql, paramters);
        }

        public static int EditRedCntByID(int id, int redcnt)
        {
            string strSql = "UPDATE [LotteryActivitysAreaRedPack] SET RedCnt=@RedCnt WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@RedCnt",redcnt),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public int EditByID()
        {
            string strSql = "UPDATE [LotteryActivitysAreaRedPack] SET MaxPrice=@MaxPrice,MinPrice=@MinPrice WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@MaxPrice",_maxprice),
                new System.Data.SqlClient.SqlParameter("@MinPrice",_minprice),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
