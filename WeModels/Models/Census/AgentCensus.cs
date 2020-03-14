using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public static class AgentCensus
    {
        /// <summary>
        /// 获取每年订单销售额
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="Type">销售类型</param>
        /// <returns></returns>
        public static string GetAgentCount(int Year)
        {
            string BeginTime = GetTimeCommon.GetTheYearBeginTime(Year).ToString();
            string EndTime = GetTimeCommon.GetTheYearEndTime(Year).ToString();
            string strSql = "select * from [C_User] where  state='已审核' and  DatVerify>=@BeginTime and DatVerify<@EndTime";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@BeginTime",BeginTime),
                 new System.Data.SqlClient.SqlParameter("@EndTime",EndTime),
                  
            };
            List<C_User> list = DAL.EntityDataHelper.FillData2Entities<C_User>(strSql, paramters);
            List<int> Salelist = new List<int>();
            List<C_User> listMonthSale = new List<C_User>();
            int Endyear = 0;
            int EndMonth = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (i == 12)
                {
                    Endyear = Year + 1;
                    listMonthSale = list.Where(a => a.DatVerify >= DateTime.Parse(Year + "-12-01 00:00:00") && a.DatVerify < DateTime.Parse(Endyear + "-01-01 00:00:00")).ToList();
                }
                else
                {
                    EndMonth = i + 1;
                    listMonthSale = list.Where(a => a.DatVerify >= DateTime.Parse(Year + "-" + i + "-01 00:00:00") && a.DatVerify < DateTime.Parse(Year + "-" + EndMonth + "-01 00:00:00")).ToList();
                }
                Salelist.Add(listMonthSale.Count);
            }

            return JsonConvert.SerializeObject(Salelist);
        }
        
    }
    public class SearchAgentIncrease:BaseSearch
    {
        /// <summary>
        /// 多条件
        /// </summary>
        public string  keyword { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int C_UserTypeID { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string DatCreateMon { get; set; }
    }
    public class AgentIncrease
    {
        /// <summary>
        /// 代理数量
        /// </summary>
        public decimal agentcount { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// 这个月增长数量
        /// </summary>
        public int counts { get; set; }
        /// <summary>
        /// 团队人数
        /// </summary>
        public int Teamcount { get; set; }
        /// <summary>
        /// 团队总人数
        /// </summary>
        public int Team { get; set; }

    }
}
