using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    //ssss
    public class LotteryRecordSearch : BaseSearch  
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNumber { get; set; }
        public string NickName { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        public string PrizeName { get; set; }
        /// <summary>
        /// 中奖时间开始
        /// </summary>
        public string DatB { get; set; }
        /// <summary>
        /// 中奖时间结束
        /// </summary>
        public string DatE { get; set; }
        public string redArea { get; set; }
        /// <summary>
        /// 获奖时间
        /// </summary>
        public string Dat { get; set; }
        public static string StrWhere(LotteryRecordSearch condition)
        {
            string where = string.Empty;
            //流水号
            if (!string.IsNullOrWhiteSpace(condition.SerialNumber))
            {
                where += " and SerialNumber like '%" + Common.Filter(condition.SerialNumber) + "%' ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (SerialNumber like '%{0}%' or UserName like '%{0}%' 
                                           or ActivityTitle like '%{0}%' or PrizeLevel like '%{0}%' 
                                           or PrizeName like '%{0}%' or IntegralCode like '%{0}%') ", condition.keyword);
            }
            //获奖时间
            if (!string.IsNullOrWhiteSpace(condition.DatB))
            {
                where += string.Format(" and DatB >='{0} 00:00:00' ", Common.Filter(condition.DatB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatE))
            {
                where += string.Format(" and DatE <'{0} 23:59:59' ", Common.Filter(condition.DatE));
            }
            if (string.IsNullOrWhiteSpace(condition.redArea) == false && condition.redArea != "-请选择-") 
            {
                where += string.Format(" and redArea='{0}' ", condition.redArea);
            }

            //奖品名称
            if (!string.IsNullOrWhiteSpace(condition.PrizeName))
            {
                where += string.Format(" and PrizeName ='{0}' ", Common.Filter(condition.PrizeName));
            }
            return where;
        }
    }
    public partial class LotteryRecord
    {
        /// <summary>
        /// 读取所有实例，限制10万条。
        /// </summary>
        public static List<LotteryRecord> GetEntitysbywhere(string SQL)
        {
            string strSql = "SELECT TOP 100000 ID,SerialNumber,UserName,ActivityID,ActivityTitle,Dat,IntegralCode,redMoney,redArea,ip,province FROM [LotteryRecord]  where 1=1 " + SQL;
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<LotteryRecord>(strSql, paramters);
        }
        public static List<LotteryRecord> GetEntitysbytg(string SQL)
        {
            string strSql = "SELECT TOP 100000 LotteryRecord.ID,LotteryRecord.SerialNumber,LotteryRecord.UserName,LotteryRecord.ActivityID,LotteryRecord.ActivityTitle,LotteryRecord.Dat,LotteryRecord.IntegralCode,LotteryRecord.redMoney,LotteryRecord.redArea,LotteryRecord.ip,LotteryRecord.province FROM [LotteryRecord] left join C_WxUser on LotteryRecord.UserName=C_WxUser.UserName where C_WxUser.Email<>'T' " + SQL;
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.FillData2Entities<LotteryRecord>(strSql, paramters);
        }
        public static List<LotteryRecord> GetCntUserNameIntegralCode(string UserName, string IntegralCode)
        {
            string strSql = "SELECT TOP 100000 * FROM [LotteryRecord] where UserName=@UserName and IntegralCode=@IntegralCode ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                             new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode)};

            return DAL.EntityDataHelper.FillData2Entities<LotteryRecord>(strSql, paramters);
        }


        public static List<LotteryRecord> GetCntIntegralCode(string IntegralCode)
        {
            string strSql = "SELECT TOP 100000 * FROM [LotteryRecord] where  IntegralCode=@IntegralCode ";
            System.Data.SqlClient.SqlParameter[] paramters = { 
                                                             new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode)};

            return DAL.EntityDataHelper.FillData2Entities<LotteryRecord>(strSql, paramters);
        }

        public static LotteryRecord GetCntIntegralCodetg(string IntegralCode)
        {
            string strSql = "SELECT TOP 1 * FROM [LotteryRecord] WHERE IntegralCode=@IntegralCode order by ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode) };

            return DAL.EntityDataHelper.LoadData2Entity<LotteryRecord>(strSql, paramters);
        }
        public static List<LotteryRecord> GetcountNumber(decimal redMoney)
        {
            string SqlStr = "SELECT * FROM [LotteryRecord] WHERE redMoney=@redMoney";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@redMoney",redMoney)
             };
            return DAL.EntityDataHelper.FillData2Entities<LotteryRecord>(SqlStr, Parameter);
        }
        /// <summary>
        /// 根据流水号获取记录
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        public static LotteryRecord GetRecBySerialNumber(string SerialNumber)
        {
            string strSql = "SELECT * FROM [LotteryRecord] WHERE SerialNumber=@SerialNumber";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@SerialNumber", SerialNumber) };

            return DAL.EntityDataHelper.LoadData2Entity<LotteryRecord>(strSql, paramters);
        }
        public static string GetC_UserCountCennsus()
        {
            //  string strSql = "select top(10) sum(redMoney) Count,UserName name from LotteryRecord  group by UserName order by Count desc ";
            string strSql = "select name,Count From desktophb ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<C_CountCensus> list = DAL.EntityDataHelper.FillData2Entities<C_CountCensus>(strSql, paramters);
            List<Census> censusList = new List<Census>();
            foreach (C_CountCensus item in list)
            {
                censusList.Add(new Census
                {
                    name = item.name,
                    data = new int[] { item.Count }
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }
        public static string GetMonC_userCountCensus(DateTime mon)
        {
            string data = "";
            string strSql = string.Format(@"select Moth.*,isnull((select sum(redMoney)from LotteryRecord where  month(dat)=Number and datepart(yy,dat)=datepart(yy,@mon)),0) sl from Moth ");

            //select Moth.*,isnull((select sum(redMoney)from LotteryRecord where  month(dat)=Number and datepart(yy,dat)=datepart(yy,@mon)),0) sl from Moth 
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@mon", mon) };
            List<C_CountCensus> list = DAL.EntityDataHelper.FillData2Entities<C_CountCensus>(strSql, paramters);

            List<UserCensus> censusList = new List<UserCensus>();
            foreach (C_CountCensus item in list)
            {
                censusList.Add(new UserCensus
                {
                    name = item.name,
                    data = item.name
                });
            }
            if (!string.IsNullOrWhiteSpace(data))
            {
                data = data.Substring(0, data.Length - 1);
                data = "[" + data + "]";
            }

            return data;
        }

        /// <summary>
        /// 根据流水号获取记录
        /// </summary>
        /// <param name="SerialNumber"></param>
        /// <returns></returns>
        public static LotteryRecord GetRecByIntegralCode(string IntegralCode)
        {
            string strSql = "SELECT * FROM [LotteryRecord] WHERE IntegralCode=@IntegralCode";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode) };

            return DAL.EntityDataHelper.LoadData2Entity<LotteryRecord>(strSql, paramters);
        }
    }
}
