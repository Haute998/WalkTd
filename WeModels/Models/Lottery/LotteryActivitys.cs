using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class LotteryActivitysSearch:BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }

        /// <summary>
        /// 活动状态  进行中/未开始/已结束
        /// </summary>
        public string stat { get; set; }
    }

    public class PrizeAttr
    {
        public int ID { get; set; }
        public bool IsPrize { get; set; }
        public bool IsNot { get; set; }
        public int PrizeID { get; set; }
        public string ResultStr { get; set; }
    }
    public partial class LotteryActivitys
    {
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public int EditByID()
        {
            string strSql = "UPDATE [LotteryActivitys] SET Title=@Title,DatB=@DatB,DatE=@DatE,IsTimeLimit=@IsTimeLimit,IsActive=@IsActive,Explain=@Explain,FailType=@FailType,FailMsg=@FailMsg,FailPrizeID=@FailPrizeID WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",_id),
                new System.Data.SqlClient.SqlParameter("@Title",Title),
                new System.Data.SqlClient.SqlParameter("@DatB",DatB),
                new System.Data.SqlClient.SqlParameter("@DatE",DatE),
                new System.Data.SqlClient.SqlParameter("@IsTimeLimit",IsTimeLimit),
                new System.Data.SqlClient.SqlParameter("@IsActive",IsActive),
                new System.Data.SqlClient.SqlParameter("@Explain",Explain),
                new System.Data.SqlClient.SqlParameter("@FailType",FailType),
                new System.Data.SqlClient.SqlParameter("@FailMsg",FailMsg),
                new System.Data.SqlClient.SqlParameter("@FailPrizeID",FailPrizeID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        ///// <summary>
        ///// 根据主键获取对象
        ///// （当数据库表第一行是int自增主键时生成此方法）
        ///// </summary>
        //public static LotteryActivitys GetEntityByID(int id)
        //{
        //    string strSql = "SELECT ID,Title,DatB,DatE,IsTimeLimit,IsValid,IsActive,Explain,FailType,FailMsg,FailPrizeID FROM [LotteryActivitys] WHERE ID=@ID";
        //    System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

        //    return DAL.EntityDataHelper.LoadData2Entity<LotteryActivitys>(strSql, paramters);
        //}
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
            strSql = string.Format("DELETE FROM [LotteryActivitys] WHERE ID in ({0});", idsSql);
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0;
        }
    }
}
