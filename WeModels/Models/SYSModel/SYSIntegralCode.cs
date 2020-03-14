using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{

    public class SYSIntegralCodeSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string StrWhere(SYSIntegralCodeSearch condition)
        {
            string where = string.Empty;
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (WaterCode like '%{0}%' or IntegralCode like '%{0}%') ", condition.keyword);
            }
            return where;
        }
    }
    public partial class SYSIntegralCode
    {
        public static int AddRedCntedByID(int id)
        {
            string strSql = "UPDATE [SYSIntegralCode] SET RedCnted+=1 WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }


        public static int GetCntByAreaID(int AreaID)
        {
            try
            {
                string sql = "select count(1) from SYSIntegralCode where AreaID=@AreaID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@AreaID", AreaID)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string rtn = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(rtn, out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "SYSIntegralCode_GetCntByLAARPP_ID_error");
                return 0;
            }
        }

        public static int GetCntByLAARPP_ID(int LAARPP_ID)
        {
            try
            {
                string sql = "select count(1) from SYSIntegralCode where LAARPP_ID=@LAARPP_ID";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@LAARPP_ID", LAARPP_ID)  };
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string rtn= obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(rtn,out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "SYSIntegralCode_GetCntByLAARPP_ID_error");
                return 0;
            }
        }


        public static int StateByID(int id, string stat, int LAARPP_ID)
        {
            string strSql = "UPDATE [SYSIntegralCode] SET State=@State,LAARPP_ID=@LAARPP_ID WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@State","已抽奖"),
                   new System.Data.SqlClient.SqlParameter("@LAARPP_ID",LAARPP_ID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        ///     清空区域设置
        /// </summary>
        /// <param name="AreaID"></param>
        /// <returns></returns>
        public static int ClearAreaID(int AreaID)
        {
            string strSql = "UPDATE [SYSIntegralCode] SET AreaID=0 WHERE AreaID=@AreaID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@AreaID",AreaID),
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 批量设置区域
        /// </summary>
        /// <param name="WaterCodeBStr"></param>
        /// <param name="WaterCodeEStr"></param>
        /// <param name="AreaID"></param>
        /// <returns></returns>
        public static string AreaIDSets(string WaterCodeBStr, string WaterCodeEStr, int AreaID)
        {

            int WaterCodeB = 0;
            int.TryParse(WaterCodeBStr, out WaterCodeB);

            int WaterCodeE = 0;
            int.TryParse(WaterCodeEStr, out WaterCodeE);

            if (WaterCodeB <= 0)
            {
                return "输入的第一个流水码有误";
            }

            if (WaterCodeE <= 0)
            {
                return "输入的第二个流水码有误";
            }
            if (WaterCodeB > WaterCodeE)
            {
                return "第一个流水码必须小于第二个流水码";
            }
            
         
            string strSql = @"UPDATE [dbo].[SYSIntegralCode]
                            SET [AreaID] = @AreaID
                            WHERE convert(int,[WaterCode])>=@WaterCodeB and convert(int,[WaterCode])<=@WaterCodeE";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@AreaID",AreaID),
                new System.Data.SqlClient.SqlParameter("@WaterCodeB",WaterCodeB),
                new System.Data.SqlClient.SqlParameter("@WaterCodeE",WaterCodeE)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt>0?"ok":"设置失败";
        }

        /// <summary>
        /// 是否已存在
        /// </summary>
        /// <returns></returns>
        public bool IsHave()
        {
            try
            {
                string sql = "select count(1) from SYSIntegralCode where WaterCode=@WaterCode or IntegralCode=@IntegralCode";
                System.Data.SqlClient.SqlParameter[] paramters = {
              new System.Data.SqlClient.SqlParameter("@WaterCode", WaterCode),
              new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode)};
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string rtn = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(rtn, out cnt);
                return cnt > 0 ? true : false;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "SYSIntegralCode_IsHave_error");
                return false;
            }
        }

        /// <summary>
        /// 根据积分码获取
        /// </summary>
        /// <param name="IntegralCode"></param>
        /// <returns></returns>
        public static SYSIntegralCode GetEntityByIntegralCode(string IntegralCode)
        {
            string strSql = "SELECT * FROM [SYSIntegralCode] WHERE IntegralCode=@IntegralCode";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode) };

            return DAL.EntityDataHelper.LoadData2Entity<SYSIntegralCode>(strSql, paramters);
        }
        /// <summary>
        /// 红包是否激活
        /// 是否出货
        /// </summary>
        /// <param name="IntegralCode"></param>
        /// <returns></returns>
        public static bool IsActivatedHB(string IntegralCode)
        {
            try
            {
                string strSql = "select CodeState from Scale where AntiCode=@IntegralCode";
                System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@IntegralCode", IntegralCode) };
                var state = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
                return state == "已出货" ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }
            //select CodeState from Scale where AntiCode=''; 

  
        /// <summary>
        /// 更新状态 根据主键 【事务版】
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stat">已使用/未使用</param>
        /// <returns></returns>
        public static int UpdateStatByID(int id, string stat, System.Data.SqlClient.SqlTransaction tran)
        {
            string strSql = "UPDATE [SYSIntegralCode] SET State=@State WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@State",stat)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 设置奖品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prizeID"></param>
        /// <returns></returns>
        public static int SetPrizeByID(int id, int prizeID)
        {
            string strSql = "UPDATE [SYSIntegralCode] SET PrizesID=@PrizesID WHERE ID=@ID;";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@ID",id),
                new System.Data.SqlClient.SqlParameter("@PrizesID",prizeID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 防伪码数量
        /// </summary>
        /// <returns></returns>
        public static int GetIntegralCodeCnt()
        {
            try
            {
                string sql = "select count(1) from SYSIntegralCode";
                System.Data.SqlClient.SqlParameter[] paramters = null;
                object obj = DAL.SqlHelper.ExecuteScalar(sql, paramters);
                string cntStr = obj != null ? obj.ToString() : string.Empty;
                int cnt = 0;
                int.TryParse(cntStr, out cnt);
                return cnt;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "LotteryPrizes_GetIntegralCodeCnt");
                return 0;
            }
        }

    }
}
