using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels;

namespace WeModels
{
    public  class SaleCensus
    {
        /// <summary>
        ///饼图产品名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///饼图产品所占比例
        /// </summary>
        public decimal y { get; set; }

        /// <summary>
        /// 获取每年订单销售额
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="Type">销售类型</param>
        /// <returns></returns>
    
        /// 出货量统计
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static string GetCountSclae(int Year)
        {
            string BeginTime = GetTimeCommon.GetTheYearBeginTime(Year).ToString();
            string EndTime = GetTimeCommon.GetTheYearEndTime(Year).ToString();
            string strSql = "select * from [ScaleOutStoke] where  Shipper='总部' and  DatCreate>=@BeginTime and DatCreate<@EndTime";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@BeginTime",BeginTime),
                 new System.Data.SqlClient.SqlParameter("@EndTime",EndTime),
                  
            };
            List<ScaleOutStoke> list = DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(strSql, paramters);
            List<int> Salelist = new List<int>();
            List<ScaleOutStoke> listMonthSale = new List<ScaleOutStoke>();
            int Endyear = 0;
            int EndMonth = 0;
            for (int i = 1; i <= 12; i++)
            {
                //if (i == 12)
                //{
                //    Endyear = Year + 1;
                //    listMonthSale = list.Where(a => a.DatCreate >= DateTime.Parse(Year + "-12-01 00:00:00") && a.DatCreate < DateTime.Parse(Endyear + "-01-01 00:00:00")).ToList();
                //}
                //else
                //{
                //    EndMonth = i + 1;
                //    listMonthSale = list.Where(a => a.DatCreate >= DateTime.Parse(Year + "-" + i + "-01 00:00:00") && a.DatCreate < DateTime.Parse(Year + "-" + EndMonth + "-01 00:00:00")).ToList();
                //}
                int sales = 0;
                foreach (ScaleOutStoke item in listMonthSale)
                {
                    sales += 1;
                }
                Salelist.Add(sales);
            }

            return JsonConvert.SerializeObject(Salelist);
        }





        public static string GetCountC_Consumer(int Year,string Type)
        {
            string BeginTime = GetTimeCommon.GetTheYearBeginTime(Year).ToString();
            string EndTime = GetTimeCommon.GetTheYearEndTime(Year).ToString();
            string strSql = "select * from [C_Consumer] where DatReg>=@BeginTime and DatReg<@EndTime and Type='"+Type+"'";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@BeginTime",DateTime.Parse(BeginTime)),
                 new System.Data.SqlClient.SqlParameter("@EndTime",DateTime.Parse(EndTime)),
                  
            };
            List<C_Consumer> list = DAL.EntityDataHelper.FillData2Entities<C_Consumer>(strSql, paramters);
            List<int> Salelist = new List<int>();
            List<C_Consumer> listMonthSale = new List<C_Consumer>();
            int Endyear = 0;
            int EndMonth = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (i == 12)
                {
                    Endyear = Year + 1;
                    listMonthSale = list.Where(a => a.DatReg >= DateTime.Parse(Year + "-12-01 00:00:00") && a.DatReg < DateTime.Parse(Endyear + "-01-01 00:00:00")).ToList();
                }
                else
                {
                    EndMonth = i + 1;
                    listMonthSale = list.Where(a => a.DatReg >= DateTime.Parse(Year + "-" + i + "-01 00:00:00") && a.DatReg < DateTime.Parse(Year + "-" + EndMonth + "-01 00:00:00")).ToList();
                }
                int sales = 0;
                foreach (C_Consumer item in listMonthSale)
                {
                    sales += 1;
                }
                Salelist.Add(sales);
            }

            return JsonConvert.SerializeObject(Salelist);
        }








        /// <summary>
        /// 返利统计
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static string GetSaleRebate(int Year)
        {
            string BeginTime = GetTimeCommon.GetTheYearBeginTime(Year).ToString();
            string EndTime = GetTimeCommon.GetTheYearEndTime(Year).ToString();
            string strSql = "select * from [C_UserRebate] where  State='已发放' and  DatVerity>=@BeginTime and DatVerity<@EndTime";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@BeginTime",BeginTime),
                 new System.Data.SqlClient.SqlParameter("@EndTime",EndTime),
                  
            };
            List<C_UserRebate> list = DAL.EntityDataHelper.FillData2Entities<C_UserRebate>(strSql, paramters);
            List<decimal> Salelist = new List<decimal>();
            List<C_UserRebate> listMonthSale = new List<C_UserRebate>();
            int Endyear = 0;
            int EndMonth = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (i == 12)
                {
                    Endyear = Year + 1;
                    listMonthSale = list.Where(a => a.DatVerity >= DateTime.Parse(Year + "-12-01 00:00:00") && a.DatVerity < DateTime.Parse(Endyear + "-01-01 00:00:00")).ToList();
                }
                else
                {
                    EndMonth = i + 1;
                    listMonthSale = list.Where(a => a.DatVerity >= DateTime.Parse(Year + "-" + i + "-01 00:00:00") && a.DatVerity < DateTime.Parse(Year + "-" + EndMonth + "-01 00:00:00")).ToList();
                }
                decimal sales = 0;
                foreach (C_UserRebate item in listMonthSale)
                {
                    sales += item.Money;
                }
                Salelist.Add(sales);
            }

            return JsonConvert.SerializeObject(Salelist);
        }
      
    
    }
}
