using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class RebateSearch : BaseSearch
    {
        /// <summary>
        /// 多条件查询
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 唯一
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 创建月  格式 yyyy/MM/00
        /// </summary>
        public string DatCreateMon { get; set; }
        /// <summary>
        /// 推荐返利/拿货返利
        /// </summary>
        public string Cat { get; set; }
        /// <summary>
        /// 发放状态
        /// </summary>
        public string State { get; set; }
        public string OrderNo { get; set; }

        /// <summary>
        /// 拿货返利条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string StrWhere_nh(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and C_User.Name like '%" + condition.keyword + "%'";
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateMon))
            {
                condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

                DateTime DatMon = new DateTime();
                DateTime.TryParse(condition.DatCreateMon, out DatMon);

                where += string.Format("  and datepart(mm,C_UserRebate.DatCreat)=datepart(mm,'{0}') ", DatMon.ToString("yyyy/MM/dd"));
            }
            if (!string.IsNullOrWhiteSpace(condition.State))
            {
                where += string.Format(" and C_UserRebate.[State]='{0}'", condition.State);
            }
            return where;
        }
        /// <summary>
        /// 推荐返利条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string StrWhere_tj(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name like '%" + condition.keyword + "%'";
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateMon))
            {
                condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

                DateTime DatMon = new DateTime();
                DateTime.TryParse(condition.DatCreateMon, out DatMon);

                where += string.Format("  and datepart(mm,DatCreat)=datepart(mm,'{0}') ", DatMon.ToString("yyyy/MM/dd"));
            }
            if (!string.IsNullOrWhiteSpace(condition.State))
            {
                where += string.Format(" and C_UserRebate.[State]='{0}'", condition.State);
            }
            return where;
        }

    }
    public partial class C_UserRebate
    {
        public int ProductCnt { get; set; }
        /// <summary>
        /// 返利人电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 返利人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 下单人姓名
        /// </summary>
        public string order_CName { get; set; }

        public int 年 { get; set; }
        public int 月 { get; set; }
        public decimal 返利合计 { get; set; }

        public decimal 订单总额 { get; set; }
        /// <summary>
        /// 拿货人
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        public string userTypeName { get; set; }
        /// <summary>
        /// 拿货人级别
        /// </summary>
        public string GLever { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal SumPrice { get; set; }





        /// <summary>
        /// 我推荐的人数
        /// </summary>
        /// <returns></returns>
        public static int getMy_tjs_cnt(string Introducer)
        {
            string strSql = "SELECT Count(1) FROM [C_User] WHERE Introducer=@Introducer ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Introducer", Introducer) };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }


        /// <summary>
        /// 获取拿货返利  UserName不能为空
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static decimal GetRebateMoney_nh(string UserName, DateTime datMon, string State, string Issuer)
        {
            decimal GetOrderRateMaxPrice = BaseParameters.getPrice("GetOrderRateMaxPrice");//拿货返利门槛金额
            decimal GetOrderRate = BaseParameters.getRate("GetOrderRate");//拿货返利

            string SqlStr = string.Format(@" select  
                                            case when sum(C_UserRebate.[OrderMoney])>=@GetOrderRateMaxPrice 
                                            then sum(C_UserRebate.[OrderMoney])*@GetOrderRate
                                            else 0 end
                                            返利合计
                                            from C_UserRebate
                                            where C_UserRebate.Cat='拿货返利' and datepart(mm,C_UserRebate.DatCreat)=datepart(mm,@datMon)
                                            and C_UserRebate.R_People=@UserName and C_UserRebate.[State]=@State ");
            if (!string.IsNullOrWhiteSpace(Issuer))
            {
                SqlStr += " and Issuer=@Issuer ";
            }
            System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@GetOrderRateMaxPrice", GetOrderRateMaxPrice),
                                                                new System.Data.SqlClient.SqlParameter("@GetOrderRate", GetOrderRate),
                                                                new System.Data.SqlClient.SqlParameter("@datMon", datMon),
                                                                new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@State", State),
                                                                 new System.Data.SqlClient.SqlParameter("@Issuer", Issuer)};
            return Convert.ToDecimal(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));
        }
        /// <summary>
        /// 获取推荐返利   支持UserName为空查所有
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="datMon"></param>
        /// <returns></returns>
        public static decimal GetRebateMoney_tj(string UserName, DateTime datMon, string State, string Issuer)
        {
            string SqlStr = string.Format(@" select ISNULL(SUM(Money),0) from C_UserRebate where Cat='推荐返利'
                                             and datepart(mm,C_UserRebate.DatCreat)=datepart(mm,@datMon) and [State]=@State ");
            if (!string.IsNullOrWhiteSpace(Issuer))
            {
                SqlStr += " and Issuer=@Issuer ";
            }
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                SqlStr += " and R_People=@UserName ";
            }
            System.Data.SqlClient.SqlParameter[] SqlParameter = {
                                                                new System.Data.SqlClient.SqlParameter("@datMon", datMon),
                                                                new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@State", State),
                                                                new System.Data.SqlClient.SqlParameter("@Issuer", Issuer)};
            return Convert.ToDecimal(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));
        }

        /// <summary>
        /// 历史返利
        /// </summary>
        /// <param name="username"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static decimal GetHistoryMoney(string username, string where)
        {
            try
            {
                string sql = "select ISNULL(SUM(Money),0) from C_UserRebate where R_People=@R_People and datepart(mm,DatCreat)<>datepart(mm,@DatMon)  " + where;
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@R_People", username),
              new System.Data.SqlClient.SqlParameter("@DatMon", DateTime.Now)};
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string str = obj == null ? "" : obj.ToString();
                decimal money = 0;
                decimal.TryParse(str, out money);
                return money;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "C_UserRebate_GetHistoryMoney_error");
                return 0;
            }
        }



        /// <summary>
        /// 获取拿货返利
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static decimal GetRebateMoneyHistory_nh(string UserName, string State)
        {
            decimal GetOrderRateMaxPrice = BaseParameters.getPrice("GetOrderRateMaxPrice");//拿货返利门槛金额
            decimal GetOrderRate = BaseParameters.getRate("GetOrderRate");//拿货返利

            string SqlStr = string.Format(@" select  
                                            case when sum(C_UserRebate.[OrderMoney])>=@GetOrderRateMaxPrice 
                                            then sum(C_UserRebate.[OrderMoney])*@GetOrderRate
                                            else 0 end
                                            返利合计
                                            from C_UserRebate
                                            where C_UserRebate.Cat='拿货返利' and datepart(mm,C_UserRebate.DatCreat)<>datepart(mm,@datMon)
                                            and C_UserRebate.R_People=@UserName and C_UserRebate.[State]=@State ");
            System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@GetOrderRateMaxPrice", GetOrderRateMaxPrice),
                                                                new System.Data.SqlClient.SqlParameter("@GetOrderRate", GetOrderRate),
                                                                new System.Data.SqlClient.SqlParameter("@datMon", DateTime.Now),
                                                                new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@State", State)};
            return Convert.ToDecimal(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));
        }
        /// <summary>
        /// 获取某代理历史推荐返利
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="datMon"></param>
        /// <returns></returns>
        public static decimal GetRebateMoneyHistory_tj(string UserName, string State)
        {
            string SqlStr = string.Format(@" select sum(Money) from C_UserRebate where R_People=@UserName and Cat='推荐返利'
                                             and datepart(mm,C_UserRebate.DatCreat)<>datepart(mm,@datMon) and [State]=@State ");
            System.Data.SqlClient.SqlParameter[] SqlParameter = {
                                                                new System.Data.SqlClient.SqlParameter("@datMon", DateTime.Now),
                                                                new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@State", State)};
            object result = DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter);
            if (Convert.IsDBNull(result))
            {
                return 0;
            }
            return Convert.ToDecimal(result);
        }


        /// <summary>
        /// 返利发放
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="DatMon"></param>
        /// <param name="Cat">拿货返利/推荐返利</param>
        /// <returns></returns>
        public static string UpdateRebateState(string UserName, DateTime DatMon, string Cat, string Issuer)
        {
            decimal money = 0;
            if (Cat == "拿货返利")
            {
                money = C_UserRebate.GetRebateMoney_nh(UserName, DatMon, "未发放", Issuer);//返利金额
            }
            else if (Cat == "推荐返利")
            {
                money = C_UserRebate.GetRebateMoney_tj(UserName, DatMon, "未发放", Issuer);//返利金额
            }
            else
            {
                return "未知的返利类型";
            }
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string strSql = "UPDATE C_UserRebate set State='已发放',DatVerity=getdate() where R_People=@UserName and datepart(mm,DatCreat)=datepart(mm,@DatMon) and Cat=@Cat and  C_UserRebate.Issuer=@Issuer";
                    System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@DatMon", DatMon),
                                                                        new System.Data.SqlClient.SqlParameter("@Cat", Cat),
                                                                        new System.Data.SqlClient.SqlParameter("@Issuer", Issuer)};
                    int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, SqlParameter);

                    C_User.reduceMoneyByUserName(tran, UserName, money);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "UpdateRebateState_Error");
                    return "发放失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return "ok";
        }
        /// <summary>
        /// 取消返利
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="DatMon"></param>
        /// <param name="Cat">拿货返利/推荐返利</param>
        /// <returns></returns>
        public static string UpdateRebateStateCancel(string UserName, DateTime DatMon, string Cat, string Issuer)
        {
            decimal money = 0;
            if (Cat == "拿货返利")
            {
                money = C_UserRebate.GetRebateMoney_nh(UserName, DatMon, "已发放", Issuer);//返利金额
            }
            else if (Cat == "推荐返利")
            {
                money = C_UserRebate.GetRebateMoney_tj(UserName, DatMon, "已发放", Issuer);//返利金额
            }
            else
            {
                return "未知的返利类型";
            }
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    string strSql = "UPDATE C_UserRebate set State='未发放' where R_People=@UserName and datepart(mm,DatCreat)=datepart(mm,@DatMon) and Cat=@Cat and C_UserRebate.Issuer=@Issuer";
                    System.Data.SqlClient.SqlParameter[] SqlParameter = { new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                                new System.Data.SqlClient.SqlParameter("@DatMon", DatMon),
                                                                        new System.Data.SqlClient.SqlParameter("@Cat", Cat),
                                                                        new System.Data.SqlClient.SqlParameter("@Issuer", Issuer)};
                    int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, SqlParameter);

                    C_User.addMoneyByUserName(tran, UserName, money);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "UpdateRebateStateCancel_Error");
                    return "发放失败";
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return "ok";
        }
    }


    public class RebateStatistics
    {
        /// <summary>
        /// 本月返利
        /// </summary>
        public decimal thisMonMoney { get; set; }
        /// <summary>
        /// 本月推荐返利
        /// </summary>
        public decimal thisMonMoneyTJ { get; set; }
        /// <summary>
        /// 本月拿货返利
        /// </summary>
        public decimal thisMonMoneyNH { get; set; }
        /// <summary>
        /// 历史累计返利
        /// </summary>
        public decimal HistoryMoney { get; set; }
        /// <summary>
        /// 历史推荐返利
        /// </summary>
        public decimal HistoryMoneyTJ { get; set; }
        /// <summary>
        /// 历史拿货明细
        /// </summary>
        public decimal HistoryMoneyNH { get; set; }
        /// <summary>
        /// 我推荐的人数
        /// </summary>
        public int My_tjs_cnt { get; set; }


    }

    public class OutRebateStatistics
    {
        /// <summary>
        /// 上月应付返利
        /// </summary>
        public decimal lastMonMoney { get; set; }
    }


    /// <summary>
    /// 返利计算
    /// </summary>
    public class C_UsersRebateMoney
    {
        /// <summary>
        /// 代理ID
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获得的返利金额
        /// </summary>
        public decimal getMoney { get; set; }
    }

}
