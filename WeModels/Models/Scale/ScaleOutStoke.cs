using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace WeModels
{
    public class ScaleOutStokeCount
    { 
        public int Year{get;set;}
        public int Month{get;set;}
        public int MCount { get; set; }
    }

    public partial class ScaleOutStoke
    {
        public string Code { get; set; }
        public string Province { get; set; }

        /// <summary>
        /// 根据订单号查询
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static List<ScaleOutStoke> GetListByOrderNo(string orderNo)
        {
            string strSql = "SELECT * FROM [ScaleOutStoke] where O_ID=@O_ID";
            System.Data.SqlClient.SqlParameter[] paramters = {
                                                               new System.Data.SqlClient.SqlParameter("@O_ID",orderNo)
                                                           };

            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(strSql, paramters);
        }
        public static int DeleteOutScaleRtnState(string code)
        {
            string SqlStr = "Delete from ScaleOutStoke WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }
        public string GetSalesOrder(int Year)
        {
            string strSql = "select [Year],[Month],COUNT([Month]) MCount from "+
                            "(select *,dbo.GetTimestampYear(CreateTime) as [Year],dbo.GetTimestampMonth(CreateTime) as [Month] "+
                            "from ScaleOutStoke )def where [Year]=@Year "+
                            "group by [Year],[Month]";

            SqlParameter[] paramters = { new SqlParameter("@Year", Year), };
            List<ScaleOutStokeCount> list = DAL.EntityDataHelper.FillData2Entities<ScaleOutStokeCount>(strSql, paramters);

            List<int> Salelist = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ScaleOutStokeCount ScaleMCount = list.Where(a => a.Year == Year && a.Month == i).FirstOrDefault();
                int Count = ScaleMCount != null ? ScaleMCount.MCount : 0;

                Salelist.Add(Count);
            }

            return JsonConvert.SerializeObject(Salelist);
        }
        public static List<ScaleOutStoke> GetcODEScaleList(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE BIGCODE=@CODE OR SMALLCODE=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(SqlStr, Parameter);
        }

        public static List<ScaleOutStoke> GetSmallCodeList(string SmallCode)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE SmallCode=@SmallCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@SmallCode",SmallCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(SqlStr, Parameter);
        }

        public static bool GetInScale(string code)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOutStoke WHERE SMALLCODE=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        /// 判断出货大标是否有用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<ScaleOutStoke> GetBigScaleList(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE BIGCODE=@CODE and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(SqlStr, Parameter);
        }

        public static List<ScaleOutStoke> GetListByAntiCode(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE Anticode=@CODE and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(SqlStr, Parameter);
        }
        /// <summary>
        /// 判断出或小标是否有用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<ScaleOutStoke> GetSmallScaleList(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE SmallCode=@CODE and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleOutStoke>(SqlStr, Parameter);
        }


        public static ScaleOutStoke  GetSmallScaleListdz(string code)
        {
            string SqlStr = "SELECT ScaleOutStoke.*,C_User.Province as Province FROM ScaleOutStoke left join C_User on ScaleOutStoke.Consignee=C_User.UserName WHERE SmallCode=@SmallCode or BigCode=@BigCode and ScaleOutStoke.State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@SmallCode",code),
                      new System.Data.SqlClient.SqlParameter("@BigCode",code)
             };
            return DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
        }
        public static ScaleOutStoke GetSmallScaleListcode(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE State='启用' and ID in(select OutId from ScaleOut_Anti where AntiCode=@AntiCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@AntiCode",code)
             };
            return DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
        }
        public static ScaleOutStoke GetSmallScaleListsn(string code)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE BigCode=@BigCode or SmallCode=@SmallCode ";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@BigCode",code),
                      new System.Data.SqlClient.SqlParameter("@SmallCode",code)
             };
            return DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
        }
        /// <summary>
        /// 判断大标是否入库
        /// </summary>
        /// <returns></returns>
        public static bool GetBigInScale(string code)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOutStoke WHERE BIGCODE=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        /// 判断是否有出货库存
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static bool GetC_UserOutScale(string C_user, string Scale)
        {
            string SqlStr = "SELECT COUNT(*) FROM ScaleOutStoke WHERE Consignee=@C_user and SmallCode=@Scale and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        /// 获取出货数量
        /// </summary>
        /// <param name="C_user"></param>
        /// <returns></returns>
        public static ScaleCodeCount GetC_UserOutCount(string where)
        {
            string SqlStr = "select COUNT(distinct case when BigCode<>'' then BigCode else null end) as BigCount," +
                            "COUNT(distinct case when MiddleCode<>'' then MiddleCode else null end) as MiddleCount," +
                            "COUNT(SmallCode) as SmallCount from ScaleOutStoke where Shipper='总部' " + where;
            System.Data.SqlClient.SqlParameter[] Parameter =null;
            return DAL.EntityDataHelper.LoadData2Entity<ScaleCodeCount>(SqlStr, Parameter);
        }
        /// <summary>
        ///禁用库存
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static bool GetUpdateC_UserOutScale(string C_user, string Scale)
        {
            string SqlStr = "UPDATE ScaleOutStoke set STATE='禁用' WHERE Consignee=@C_user and SmallCode=@Scale and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
        public static bool GetInsertC_UserOutScale(string OrderNo, string C_user, string Scale)
        {
            string SqlStr = "SELECT * FROM  ScaleOutStoke  WHERE Consignee=@C_user and SmallCode=@Scale and State='禁用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };

            ScaleOutStoke OutStoke = DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
            OutStoke.Shipper = C_user;
            OutStoke.State = "启用";
            OutStoke.CreateTime = CommonFunc.GetNowTimestamp();
            OutStoke.Consignee = Order.GetOrderByOrderNo(OrderNo).UserName;
            int rtn = OutStoke.InsertAndReturnIdentity();
            return rtn > 0 ? true : false;
        }
        public static bool GetInsertAgentOutScale(string C_user, string Scale, string Agent)
        {
            string SqlStr = "SELECT * FROM  ScaleOutStoke  WHERE Consignee=@C_user and SmallCode=@Scale and State='禁用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };

            ScaleOutStoke OutStoke = DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
            OutStoke.Shipper = C_user;
            OutStoke.CreateTime = CommonFunc.GetNowTimestamp();
            OutStoke.State = "启用";
            OutStoke.Consignee = Agent;
            int rtn = OutStoke.InsertAndReturnIdentity();
            return rtn > 0 ? true : false;

        }
        /// <summary>
        /// 判断是否货在手里
        /// </summary>
        /// <param name="C_User"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetC_UserRtnScale(string C_User, string code)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOutStoke WHERE SMALLCODE=@CODE and ShipperId=@C_User";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code),
                                                                new System.Data.SqlClient.SqlParameter("@C_User",C_User)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        /// 判断是否货在手里
        /// </summary>
        /// <param name="C_User"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetC_UserRtnScales(string C_User, string code)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOutStoke WHERE SMALLCODE=@CODE and Shipper=@C_User and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code),
                                                                new System.Data.SqlClient.SqlParameter("@C_User",C_User)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 判断下级是否发货此小标条码
        /// </summary>
        /// <param name="C_User"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsSubOutStock_Small(string smallcode)
        {
            string SqlStr = "select count(*) from ScaleOutStoke where Shipper<>'总部' and ID in(select OutId from ScaleOut_Small where SmallCode=@SmallCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                            new System.Data.SqlClient.SqlParameter("@SmallCode",smallcode)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 判断下级是否发货此中标条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static bool IsSubOutStock_Middle(string middlecode)
        {
            string SqlStr = "select count(*) from ScaleOutStoke where Shipper<>'总部' and ID in(select OutId from ScaleOut_Middle where MiddleCode=@MiddleCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                            new System.Data.SqlClient.SqlParameter("@MiddleCode",middlecode)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 判断下级是否发货此中标条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static bool IsSubOutStock_BigCode(string bigcode)
        {
            string SqlStr = "select count(*) from ScaleOutStoke where Shipper<>'总部' and ID in(select OutId from ScaleOut_Big where BigCode=@BigCode)";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                            new System.Data.SqlClient.SqlParameter("@BigCode",bigcode)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 批量判断下级是否发货此条码
        /// </summary>
        /// <param name="C_User"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool BatchIsSubOutStock(string CodeSet)
        {
            string SqlStr = "select count(*) from ScaleOutStoke where Shipper<>'总部' and ID in(select OutId from ScaleOut_Small where SmallCode in (" + CodeSet + "))";
            System.Data.SqlClient.SqlParameter[] Parameter =null;
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        public static int GetUpdateOutScaleRtnState(string code)
        {
            string SqlStr = "Update ScaleOutStoke set State='已退货' WHERE SmallCode=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@CODE",code)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }
        /// <summary>
        /// 出货给代理(代理)
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="Scale"></param>
        /// <param name="Scale">收货人</param>
        /// <returns></returns>
        public static string GetBoolAgentOutScale(string C_user, string Scale, string Agent)
        {
            if (!ScaleOutStoke.GetC_UserOutScale(C_user, Scale))
            {
                return "没有库存";
            }
            ScaleOutStoke.GetUpdateC_UserOutScale(C_user, Scale);

            if (ScaleOutStoke.GetInsertAgentOutScale(C_user, Scale, Agent))
            {
                return "ok";
            }
            return "出错了!";
        }

        public static List<BarCode> GetOutStockID(string UserName,string SmallCodeSet)
        {
            string strSQL = "select ID,SmallCode from ScaleOutStoke where Consignee=@UserName and State='启用' and ID in (select OutId from ScaleOut_Small where SmallCode in(" + SmallCodeSet + "))";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@UserName",UserName)
             };
            return DAL.EntityDataHelper.FillData2Entities<BarCode>(strSQL, Parameter);
        }

        public static int ToOutStockAgent(string UserName, string AgentUser, string IDSet, string OutOrderNo)
        {
            string strSQL = "insert into ScaleOutStoke(Shipper,BigCode,MiddleCode,SmallCode,AntiCode,ProductNo,Consignee,State,CreateTime,OutOrderNo,OutWay)"+
                            "select @UserName,BigCode,MiddleCode,SmallCode,AntiCode,ProductNo,@AgentUser,'启用',dbo.GetNowTimestamp(),@OutOrderNo,1 from ScaleOutStoke where ID in (" + IDSet + ");" +
                            "update ScaleOutStoke set [State]='禁用' where ID in (" + IDSet + ");";

            System.Data.SqlClient.SqlParameter[] Parameter ={
                        new System.Data.SqlClient.SqlParameter("@AgentUser",AgentUser),
                        new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                        new System.Data.SqlClient.SqlParameter("@OutOrderNo",OutOrderNo),
             };
            return DAL.SqlHelper.ExecuteNonQuery(strSQL, Parameter);
        }

        /// <summary>
        /// 出货给代理(订单)
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="Scale"></param>
        /// <param name="Scale">收货人</param>
        /// <returns></returns>
        public static string GetBoolC_UserOutScale(string OrderNo, string C_user, string Scale)
        {
            if (!ScaleOutStoke.GetC_UserOutScale(C_user, Scale))
            {
                return "没有库存";
            }
            ScaleOutStoke.GetUpdateC_UserOutScale(C_user, Scale);
            if (ScaleOutStoke.GetInsertC_UserOutScale(OrderNo, C_user, Scale))
            {
                return "ok";
            }
            return "出错了!!";
        }
        /// <summary>
        /// 获取首页信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        //public static ScaleOutStokeShow GetProductC_User(string code)
        //{
        //    string SqlStr = "select s.BigCode BigCode,s.SmallCode SmallCode,s.DatCreate DatCreate,s.O_ID O_ID,s.P_Name P_Name,s.kw kw,s.P_ID P_ID,c.Name Name,s.Consignee,c.Province,c.City  from (select s.SmallCode SmallCode,s.Consignee Consignee, s.BigCode BigCode,s.DatCreate DatCreate,s.O_ID O_ID,p.ProductName P_Name,p.kw kw,P_ID P_ID from ScaleOutStoke s left join Product p on s.P_ID=p.ProductNumber where s.SmallCode='" + code + "' and s.State='启用') s left join C_User c on s.Consignee=c.UserName";
        //    System.Data.SqlClient.SqlParameter[] sqlparameter = { new System.Data.SqlClient.SqlParameter("@SmallCode", code) };
        //    return DAL.EntityDataHelper.LoadData2Entity<ScaleOutStokeShow>(SqlStr, sqlparameter);
        //}

        public static ScaleOutStokeShow GetProductC_UserByBig(string code)
        {
            string SqlStr = "select s.SmallCode,s.Consignee, s.BigCode,s.CreateTime,s.OutOrderNo,p.ProductName,p.kw,ProductNo,ProductNumber,ProductImg,c.Name,c.Province,c.City " +
                            "from ScaleOutStoke s left join Product p on s.ProductNo=p.ProductNumber left join C_User c on s.Consignee=c.UserName "+
                            "where s.SmallCode=@SmallCode and s.State='启用' ";
            System.Data.SqlClient.SqlParameter[] sqlparameter = { new System.Data.SqlClient.SqlParameter("@SmallCode", code) };
            return DAL.EntityDataHelper.LoadData2Entity<ScaleOutStokeShow>(SqlStr, sqlparameter);
        }

        public static List<BarCode> GetCanRtnCode(string username,string code)
        { 
            try
            {
                string strSQL = "select SmallCode from ScaleOutStoke where [State]='启用' and Shipper=@UserName and (BigCode=@Code or MiddleCode=@Code or SmallCode=@Code)";
                System.Data.SqlClient.SqlParameter[] Parameter ={
                                                        new System.Data.SqlClient.SqlParameter("@UserName",username),
                                                        new System.Data.SqlClient.SqlParameter("@Code",code)
                                                    };
                return DAL.EntityDataHelper.FillData2Entities<BarCode>(strSQL, Parameter);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "获取扫描条码错误");
                return null;
            }
        }

        public static List<BarCode> GetRtnStockID(string UserName, string SmallCodeSet)
        {
            string strSQL = "select ID,SmallCode from ScaleOutStoke where Shipper=@UserName and State='启用' and ID in (select OutId from ScaleOut_Small where SmallCode in(" + SmallCodeSet + "))";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@UserName",UserName)
             };
            return DAL.EntityDataHelper.FillData2Entities<BarCode>(strSQL, Parameter);
        }

        /// <summary>
        /// 分页查询获取出货记录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="StartTimestamp"></param>
        /// <param name="EndTimestamp"></param>
        /// <param name="KeyWords"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<OutStokeRecord> GetOutStockRecord(string UserName, int PageSize, int PageIndex, int StartTimestamp, int EndTimestamp, string KeyWords, out int totalCount)
        {
            string where = string.Empty;

            where = string.Format(" Shipper='{0}'", UserName);

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" and Name+OutOrderNo+ProductName like '%" + KeyWords + "%'";
            }

            if (StartTimestamp != 0)
            {
                where += string.Format(" and CreateTime>={0} ", StartTimestamp);
            }
            if (EndTimestamp != 0)
            {
                where += string.Format(" and CreateTime<={0} ", EndTimestamp);
            }

            string strSql = "exec dbo.Common_PageList 'View_OutScaleRecord','*','CreateTime desc,OutOrderNo desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };

            List<OutStokeRecord> OrderList = DAL.EntityDataHelper.FillData2Entities<OutStokeRecord>(strSql, paramters);

            strSql = "select count(*) from View_OutScaleRecord where " + where;
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;

        }

        /// <summary>
        /// 分页查询获取出货统计
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="StartTimestamp"></param>
        /// <param name="EndTimestamp"></param>
        /// <param name="KeyWords"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<OutStokeRecord> GetOutStockCount(string UserName, int PageSize, int PageIndex, int StartTimestamp, int EndTimestamp, string KeyWords, out int totalCount)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" ProductNo+ProductName like '%" + KeyWords + "%'";
            }

            string strSql = "exec dbo.Common_PageList N'dbo.GetOutStockCount(''" + UserName + "'', " + StartTimestamp + "," + EndTimestamp + ")','*','Qty desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {                       
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };
            List<OutStokeRecord> OrderList = DAL.EntityDataHelper.FillData2Entities<OutStokeRecord>(strSql, paramters);
            strSql = "select count(*) from dbo.GetOutStockCount(@UserName, @StartTimestamp,@EndTimestamp) " + (string.IsNullOrEmpty(where) ? "" : " where " + where);
            System.Data.SqlClient.SqlParameter[] param = {
                            new System.Data.SqlClient.SqlParameter("@UserName",UserName),  
                            new System.Data.SqlClient.SqlParameter("@StartTimestamp",StartTimestamp),
                            new System.Data.SqlClient.SqlParameter("@EndTimestamp",EndTimestamp),  
                                                         };
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;
        }

        /// <summary>
        /// 获取出货记录明细
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="OutOrderNo"></param>
        /// <param name="ProductNo"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="Timestamp"></param>
        /// <param name="Consignee"></param>
        /// <param name="KeyWords"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<ScaleCode_Simple> GetOutStockDetail(
                                    string UserName,
                                    string OutOrderNo,
                                    string ProductNo, 
                                    int PageSize, 
                                    int PageIndex, 
                                    int Timestamp,
                                    string Consignee, 
                                    string KeyWords, 
                                    out int totalCount
            )
        {
            string where = string.Empty;

            where = string.Format("Shipper='{0}' and OutOrderNo='{1}' and ProductNo='{2}' and CreateTime={3} and Consignee='{4}'", UserName, OutOrderNo, ProductNo, Timestamp, Consignee);

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" and isnull(BigCode,'')+isnull(MiddleCode,'')+SmallCode like '%" + KeyWords + "%'";
            }

            string strSql = "exec dbo.Common_PageList 'ScaleOutStoke','*','SmallCode desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {                       
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };
            List<ScaleCode_Simple> CodeList = DAL.EntityDataHelper.FillData2Entities<ScaleCode_Simple>(strSql, paramters);

            strSql = "select count(*) from ScaleOutStoke " + (string.IsNullOrEmpty(where) ? "" : " where " + where);
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return CodeList;
        }

        // 判断中标是否已出货
        public static bool IsMiddleOutStock(string MiddleCode)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOut_Middle WHERE MiddleCode=@MiddleCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                        new System.Data.SqlClient.SqlParameter("@MiddleCode",MiddleCode)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        // 判断大标是否已出货
        public static bool IsBigOutStock(string BigCode)
        {
            string SqlStr = "SELECT count(*) FROM ScaleOut_Big WHERE BigCode=@BigCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                        new System.Data.SqlClient.SqlParameter("@BigCode",BigCode)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
    }
}
