using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
        public class SYSIntegralCodePrizesVMSearch : BaseSearch
        {
            /// <summary>
            /// 关键字
            /// </summary>
            public string keyword { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public string State { get; set; }
            /// <summary>
            /// 奖品ID
            /// </summary>
            public int PrizesID { get; set; }
            /// <summary>
            /// 活动ID
            /// </summary>
            public int ActivityID { get; set; }
            /// <summary>
            /// 区域ID
            /// </summary>
            public int AreaID { get; set; }
            /// <summary>
            /// 查询条件
            /// </summary>
            /// <param name="condition"></param>
            /// <returns></returns>
            public static string StrWhere(SYSIntegralCodePrizesVMSearch condition)
            {
                string where = string.Empty;
                //关键字搜索
                if (!string.IsNullOrWhiteSpace(condition.keyword))
                {
                    condition.keyword = Common.Filter(condition.keyword);
                    where += string.Format(@" and ([SYSIntegralCode].WaterCode like '%{0}%' or [SYSIntegralCode].IntegralCode like '%{0}%') ", condition.keyword);
                }
                return where;
            }
        }
    public partial class SYSIntegralCodePrizesVM : SYSIntegralCode
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        public string activityName { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        public string PrizeName { get; set; }
        /// <summary>
        /// 奖品等级
        /// </summary>
        public string PrizeLevel { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 获取产品码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SYSIntegralCodePrizesVM GetVMByID(int id)
        {
            string strSql = @"SELECT [SYSIntegralCode].*,isnull(LotteryPrizes.PrizeName,'') PrizeName,isnull(LotteryActivitys.Title,'') activityName FROM [SYSIntegralCode] 
                                   left join LotteryPrizes on [SYSIntegralCode].PrizesID=LotteryPrizes.ID 
                                   left join LotteryActivitys on LotteryPrizes.ActivityID=LotteryActivitys.ID WHERE ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

            return DAL.EntityDataHelper.LoadData2Entity<SYSIntegralCodePrizesVM>(strSql, paramters);
        }
    }
}
