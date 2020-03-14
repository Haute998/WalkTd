using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class RtnStockScale
    {
        public static bool GetBoolRtnScale(string code)
        {
            string SqlStr = "SELECT count(*) FROM ScaleRtnStoke WHERE SMALLCODE=@CODE";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@CODE",code)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        ///禁用库存
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static bool GetUpdateC_UserOutScale(string C_user, string Scale)
        {
            string SqlStr = "UPDATE ScaleOutStoke set STATE='已退货' WHERE ShipperId=@C_user and SmallCode=@Scale and State='启用'";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        ///禁用库存
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static bool GetUpdateC_UserScale(string Scale)
        {
            string SqlStr = "UPDATE Scale set CodeState='已退货' WHERE  SmallCode=@Scale ";
            System.Data.SqlClient.SqlParameter[] Parameter ={  
                   new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
        /// <summary>
        ///启用上级发货
        /// </summary>
        /// <param name="C_user"></param>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static bool GetUpdateC_UserScale(string C_user, string Scale)
        {
            string SqlStr = "UPDATE ScaleOutStoke set STATE='启用' WHERE Consignee=@C_user and SmallCode=@Scale";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
        public static bool GetInsertC_UserRtnScale(string C_user, string Scale)
        {
            string SqlStr = "SELECT * FROM ScaleOutStoke WHERE ShipperId=@C_user and SmallCode=@Scale";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                                                               new System.Data.SqlClient.SqlParameter("@C_user",C_user),
                                                                new System.Data.SqlClient.SqlParameter("@Scale",Scale)
                                                           };

            ScaleOutStoke OutStoke = DAL.EntityDataHelper.LoadData2Entity<ScaleOutStoke>(SqlStr, Parameter);
            ScaleRtnStoke rtn = new ScaleRtnStoke();
            rtn.BigCode = OutStoke.BigCode;
            rtn.AntiCode = OutStoke.AntiCode;
            rtn.ReturnTime = CommonFunc.GetNowTimestamp();
            rtn.OutTime = OutStoke.CreateTime;
            rtn.SmallCode = OutStoke.SmallCode;
            rtn.ProducctNo = OutStoke.ProductNo;
            rtn.Shipper = OutStoke.Shipper;
            rtn.Shipper = OutStoke.Consignee;
            RtnStockScale.GetUpdateC_UserScale(C_user, Scale);
            int rrtn = rtn.InsertAndReturnIdentity();
            return rrtn > 0 ? true : false;

        }
        public static string GetBoolC_UserOutScale(string C_user, string Scale)
        {
            if (!ScaleOutStoke.GetC_UserRtnScales(C_user, Scale))
            {
                return "没有库存";
            }
            RtnStockScale.GetUpdateC_UserOutScale(C_user, Scale);
            RtnStockScale.GetUpdateC_UserScale(Scale);
            if (RtnStockScale.GetInsertC_UserRtnScale(C_user, Scale))
            {
                return "ok";
            }
            return "出错了!!";
        }

        public static int ToRtnStockCode(string UserName, string IDSet, string RtnOrderNo)
        {
            string strSQL = "insert into ScaleRtnStoke(OrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProducctNo,OperaUser,Shipper,Consignee,OutOrderNo,IsPDAOpera,OutTime,ReturnTime,OutWay,RtnWay) "+
                            "select @RtnOrderNo,BigCode,MiddleCode,SmallCode,AntiCode,ProductNo,@UserName,Shipper,Consignee,OutOrderNo,0,CreateTime,dbo.GetNowTimestamp(),OutWay,1 " +
                            "from ScaleOutStoke where ID in(" + IDSet + ");"+
                            "update ScaleOutStoke set [State]='启用' where Consignee=@UserName and SmallCode in (select SmallCode from ScaleOutStoke where ID in (" + IDSet + "));"+
                            "delete ScaleOutStoke where ID in(" + IDSet + ");";

            System.Data.SqlClient.SqlParameter[] Parameter ={
                        new System.Data.SqlClient.SqlParameter("@UserName",UserName),
                        new System.Data.SqlClient.SqlParameter("@RtnOrderNo",RtnOrderNo),
             };
            return DAL.SqlHelper.ExecuteNonQuery(strSQL, Parameter);
        }

        /// <summary>
        /// 获取用户退货统计
        /// </summary>
        /// <returns></returns>
        public static ScaleCodeCount GetRtnStockCount(string where)
        {
            string SqlStr = "select COUNT(distinct case when BigCode<>'' then BigCode else null end) as BigCount," +
                "COUNT(distinct case when MiddleCode<>'' then MiddleCode else null end) as MiddleCount," +
                "COUNT(SmallCode) as SmallCount from ScaleRtnStoke where Shipper='总部' " + where;
            System.Data.SqlClient.SqlParameter[] Parameter = null;
            return DAL.EntityDataHelper.LoadData2Entity<ScaleCodeCount>(SqlStr, Parameter);
        }
    }
}
