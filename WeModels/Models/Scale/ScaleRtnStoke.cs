using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class ScaleRtnStoke
    {
        public static List<ScaleRtnStoke> GetSmallCodeList(string SmallCode)
        {
            string SqlStr = "SELECT * FROM ScaleRtnStoke WHERE SmallCode=@SmallCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@SmallCode",SmallCode)
             };
            return DAL.EntityDataHelper.FillData2Entities<ScaleRtnStoke>(SqlStr, Parameter);
        }

        public static List<RtnStockRecord> GetRtnStockRecord(string UserName, int PageSize, int PageIndex, int StartTimestamp, int EndTimestamp, string KeyWords, out int totalCount)
        {
            string where = string.Empty;

            where = string.Format(" Shipper='{0}'", UserName);

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" and Name+OrderNo+ProductName like '%" + KeyWords + "%'";
            }

            if (StartTimestamp != 0)
            {
                where += string.Format(" and ReturnTime>={0} ", StartTimestamp);
            }
            if (EndTimestamp != 0)
            {
                where += string.Format(" and ReturnTime<={0} ", EndTimestamp);
            }

            string strSql = "exec dbo.Common_PageList 'View_RtnStockRecord','*','ReturnTime desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };

            List<RtnStockRecord> RtnRecordList = DAL.EntityDataHelper.FillData2Entities<RtnStockRecord>(strSql, paramters);

            strSql = "select count(*) from View_RtnStockRecord where " + where;
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return RtnRecordList;
        }

        public static List<RtnStockRecord> GetRtnStockCount(string UserName, int PageSize, int PageIndex, int StartTimestamp, int EndTimestamp, string KeyWords, out int totalCount)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" ProductNumber+ProductName like '%" + KeyWords + "%'";
            }

            string strSql = "exec dbo.Common_PageList N'dbo.GetRtnStockCount(''" + UserName + "'', " + StartTimestamp + "," + EndTimestamp + ")','*','Qty desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {                       
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };
            List<RtnStockRecord> OrderList = DAL.EntityDataHelper.FillData2Entities<RtnStockRecord>(strSql, paramters);
            strSql = "select count(*) from dbo.GetRtnStockCount(@UserName, @StartTimestamp,@EndTimestamp) " + (string.IsNullOrEmpty(where) ? "" : " where " + where);
            System.Data.SqlClient.SqlParameter[] param = {
                            new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                            new System.Data.SqlClient.SqlParameter("@StartTimestamp",StartTimestamp),
                            new System.Data.SqlClient.SqlParameter("@EndTimestamp",EndTimestamp),
                                                         };
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return OrderList;
        }

        public static List<ScaleCode_Simple> GetRtnStockDetail(
                            string UserName,
                            string OrderNo,
                            string ProductNo,
                            int PageSize,
                            int PageIndex,
                            int Timestamp,
                            string Consignee,
                            string KeyWords,
                            out int totalCount)
        {
            string where = string.Empty;

            where = string.Format("Shipper='{0}' and OrderNo='{1}' and ProducctNo='{2}' and ReturnTime={3} and Consignee='{4}'", UserName, OrderNo, ProductNo, Timestamp, Consignee);

            if (!string.IsNullOrWhiteSpace(KeyWords))
            {
                where += @" and isnull(BigCode,'')+isnull(MiddleCode,'')+SmallCode like '%" + KeyWords + "%'";
            }

            string strSql = "exec dbo.Common_PageList 'ScaleRtnStoke','*','SmallCode desc',@where,@PageSize,@PageIndex";
            System.Data.SqlClient.SqlParameter[] paramters = {                       
                            new System.Data.SqlClient.SqlParameter("@PageSize",PageSize),
                            new System.Data.SqlClient.SqlParameter("@PageIndex",PageIndex),
                            new System.Data.SqlClient.SqlParameter("@where",where),
                };
            List<ScaleCode_Simple> CodeList = DAL.EntityDataHelper.FillData2Entities<ScaleCode_Simple>(strSql, paramters);

            strSql = "select count(*) from ScaleRtnStoke " + (string.IsNullOrEmpty(where) ? "" : " where " + where);
            System.Data.SqlClient.SqlParameter[] param = null;
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, param);
            totalCount = Convert.ToInt32(obj);

            return CodeList;
        }
    }
}
