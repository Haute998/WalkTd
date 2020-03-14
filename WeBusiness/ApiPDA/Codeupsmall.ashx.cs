using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WeModels;

namespace WeBusiness.Api
{
    /// <summary>
    /// Codeupsmall 的摘要说明
    /// </summary>
    public class Codeupsmall : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            ResponseInStock response = new ResponseInStock();
            List<string> Suc = new List<string>();
            List<string> Fail = new List<string>();
            List<string> Repeat = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Middlecode");
            dt.Columns.Add("SmallCode");
            dt.Columns.Add("AntiCode");
            dt.Columns.Add("DatCreate");
            dt.Columns.Add("OrderID");
            dt.Columns.Add("Product");
            string time = DateTime.Now.ToString();
            string aa = context.Request["Code"];
            PDA pda = PDA.GetEntityByKeys(context.Request["key"]);
            if (pda == null)
            {
                response.code = "Success";
                response.errormsg = "请检查PDA是否启用";
                response.Msg = "请检查PDA是否启用";
                response.SuccessCode = JsonConvert.SerializeObject(Suc);
                response.FailCode = JsonConvert.SerializeObject(Fail);
                context.Response.Write(JsonConvert.SerializeObject(response));
                return;
            }
            string code1 = "";
            //aa = "1230,12301,12313213213,A001.1230,12302,12313213213,A001.";
            //aa = "21976549783801,m2001,12313213213.219755896672,m2001,12313213213.219741937066,m2001,12313213213.219764669041,m2001,12313213213.219762275483,m2001,12313213213.219768974181,m2001,12313213213.219750543847,m2001,12313213213.219758288196,m2001,12313213213.219723808384,m2001,12313213213.219728760211,m2001,12313213213.219761003252,m2001,12313213213.219713852620,m2001,12313213213.219791381833,m2001,12313213213.219750118413,m2001,12313213213.219704563953,m2001,12313213213.219700909699,m2001,12313213213.219764083019,m2001,12313213213.219703134869,m2001,12313213213.219706329825,m2001,12313213213.219719828532,m2001,12313213213.219768233760,m2001,12313213213.219787710980,m2001,12313213213.219713136134,m2001,12313213213.219743762803,m2001,12313213213.219704165354,m2001,12313213213.219777464027,m2001,12313213213.219778156414,m2001,12313213213.219798800629,m2001,12313213213.219786453906,m2001,12313213213.219751885585,m2001,12313213213.219723471686,m2001,12313213213.219705279240,m2001,12313213213.219748648830,m2001,12313213213.219709919778,m2001,12313213213.219730296995,m2001,12313213213.219729131110,m2001,12313213213.219784513524,m2001,12313213213.219726259043,m2001,12313213213.219795205875,m2001,12313213213.219725546456,m2001,12313213213.219747598345,m2001,12313213213.219765319893,m2001,12313213213.219798486931,m2001,12313213213.219700577501,m2001,12313213213.219758662794,m2001,12313213213.219738376777,m2001,12313213213.219712341237,m2001,12313213213.219796742659,m2001,12313213213.219705933626,m2001,12313213213.219778500712,m2001,12313213213.219757591909,m2001,12313213213.219702740071,m2001,12313213213.219790900435,m2001,12313213213.219797138257,m2001,12313213213.219759764979,m2001,12313213213.219799552316,m2001,12313213213.219732342165,m2001,12313213213.219700107950,m2001,12313213213.219713611758,m2001,12313213213.219788766102,m2001,12313213213.219781694005,m2001,12313213213.219733547176,m2001,12313213213.219772068739,m2001,12313213213.219719630770,m2001,12313213213.219706856149,m2001,12313213213.219738296515,m2001,12313213213.219742535855,m2001,12313213213.219730364821,m2001,12313213213.19720087768,m2001,12313213213.219734356073,m2001,12313213213.219711172589,m2001,12313213213.219706249862,m2001,12313213213.219777719363,m2001,12313213213.219743181342,m2001,12313213213.219720476367,m2001,12313213213.219717528200,m2001,12313213213.219702543408,m2001,12313213213.19753614304,m2001,12313213213.219789702887,m2001,12313213213.219757062348,m2001,12313213213.219749975051,m2001,12313213213.219732573891,m2001,12313213213.219739780198,m2001,12313213213.219733947274,m2001,12313213213.219773402923,m2001,12313213213.219714326944,m2001,12313213213.219747401782,m2001,12313213213.219768109297,m2001,12313213213.219711583988,m2001,12313213213.219780910418,m2001,12313213213.219734650961,m2001,12313213213.219753344717,m2001,12313213213.219767544622,m2001,12313213213.219773320646,m2001,12313213213.219768659807,m2001,12313213213.219771423164,m2001,12313213213.";
            //aa = aa + "219765497838,m2001,12313213213.219755896672,m2001,12313213213.219741937066,m2001,12313213213.219764669041,m2001,12313213213.219762275483,m2001,12313213213.219768974181,m2001,12313213213.219750543847,m2001,12313213213.219758288196,m2001,12313213213.219723808384,m2001,12313213213.219728760211,m2001,12313213213.219761003252,m2001,12313213213.219713852620,m2001,12313213213.219791381833,m2001,12313213213.219750118413,m2001,12313213213.219704563953,m2001,12313213213.219700909699,m2001,12313213213.219764083019,m2001,12313213213.219703134869,m2001,12313213213.219706329825,m2001,12313213213.219719828532,m2001,12313213213.219768233760,m2001,12313213213.219787710980,m2001,12313213213.219713136134,m2001,12313213213.219743762803,m2001,12313213213.219704165354,m2001,12313213213.219777464027,m2001,12313213213.219778156414,m2001,12313213213.219798800629,m2001,12313213213.219786453906,m2001,12313213213.219751885585,m2001,12313213213.219723471686,m2001,12313213213.219705279240,m2001,12313213213.219748648830,m2001,12313213213.219709919778,m2001,12313213213.219730296995,m2001,12313213213.219729131110,m2001,12313213213.219784513524,m2001,12313213213.219726259043,m2001,12313213213.219795205875,m2001,12313213213.219725546456,m2001,12313213213.219747598345,m2001,12313213213.219765319893,m2001,12313213213.219798486931,m2001,12313213213.219700577501,m2001,12313213213.219758662794,m2001,12313213213.219738376777,m2001,12313213213.219712341237,m2001,12313213213.219796742659,m2001,12313213213.219705933626,m2001,12313213213.219778500712,m2001,12313213213.219757591909,m2001,12313213213.219702740071,m2001,12313213213.219790900435,m2001,12313213213.219797138257,m2001,12313213213.219759764979,m2001,12313213213.219799552316,m2001,12313213213.219732342165,m2001,12313213213.219700107950,m2001,12313213213.219713611758,m2001,12313213213.219788766102,m2001,12313213213.219781694005,m2001,12313213213.219733547176,m2001,12313213213.219772068739,m2001,12313213213.219719630770,m2001,12313213213.219706856149,m2001,12313213213.219738296515,m2001,12313213213.219742535855,m2001,12313213213.219730364821,m2001,12313213213.19720087768,m2001,12313213213.219734356073,m2001,12313213213.219711172589,m2001,12313213213.219706249862,m2001,12313213213.219777719363,m2001,12313213213.219743181342,m2001,12313213213.219720476367,m2001,12313213213.219717528200,m2001,12313213213.219702543408,m2001,12313213213.19753614304,m2001,12313213213.219789702887,m2001,12313213213.219757062348,m2001,12313213213.219749975051,m2001,12313213213.219732573891,m2001,12313213213.219739780198,m2001,12313213213.219733947274,m2001,12313213213.219773402923,m2001,12313213213.219714326944,m2001,12313213213.219747401782,m2001,12313213213.219768109297,m2001,12313213213.219711583988,m2001,12313213213.219780910418,m2001,12313213213.219734650961,m2001,12313213213.219753344717,m2001,12313213213.219767544622,m2001,12313213213.219773320646,m2001,12313213213.219768659807,m2001,12313213213.219771423164,m2001,12313213213.";
            string[] list = aa.Split('.');
            string code = "";

            for (int i = 0; i <= list.Length - 1; i++)
            {
                string[] list1 = list[i].Split(',');
                if (list1.Length > 2)
                {
                    dt.Rows.Add(list1[0].ToString(), list1[1].ToString(), "", time, list1[2].ToString(), list1[3].ToString());
                }
            }
            SqlBulkCopyByDatatable(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, "Scale2", dt);
            DataSet ds = Query("SELECT distinct SmallCode FROM Scale where SmallCode in (SELECT SmallCode FROM Scale2)");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                {
                    code = code + "'" + ds.Tables[0].Rows[m]["SmallCode"] + "',";
                    code1 = code1 + ds.Tables[0].Rows[m]["SmallCode"] + "AA";
                    Suc.Add(ds.Tables[0].Rows[m]["SmallCode"].ToString());
                }
                Query("update  Scale set Scale.Middlecode=Scale2.Middlecode,Scale.OrderID=Scale2.OrderID,Scale.Product=Scale2.Product from Scale2 where Scale.SmallCode=Scale2.SmallCode");
                Query("Delete from Scale2 where SmallCode in( SELECT SmallCode FROM Scale where SmallCode in (SELECT SmallCode FROM Scale2))");
            }
            ds = Query("SELECT  distinct SmallCode FROM Scale2  group by SmallCode");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                {
                    Fail.Add(ds.Tables[0].Rows[m]["SmallCode"].ToString());
                }
            }
            Query("delete from  Scale2 where DatCreate='" + time + "'");
            if (code1.Length > 2)
            {
                code1 = code1.Substring(0, code1.Length - 2);
            }
            time = time + DateTime.Now.ToString();
            response.code = "Success";
            response.errormsg = "成功" + Suc.Count + ",失败" + Fail.Count + ",重复" + Repeat.Count + ".";
            response.Msg = code1;
            response.SuccessCode = JsonConvert.SerializeObject(Suc);
            response.FailCode = JsonConvert.SerializeObject(Fail);
            context.Response.Write(JsonConvert.SerializeObject(response));
        }
        private void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");


                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    //throw new Exception(ex.Message);
                    return null;
                }
                return ds;

            }
        }


        public class ResponseInStock
        {
            /// <summary>
            /// 调用结果标识  success/fail
            /// </summary>
            public string code { get; set; }

            /// <summary>
            /// 返回错误信息
            /// </summary>
            public string errormsg { get; set; }
            /// <summary>
            /// Success
            /// </summary>
            public string Msg { get; set; }
            /// <summary>
            /// 成功条码
            /// </summary>
            public string SuccessCode { get; set; }
            /// <summary>
            /// 失败条码
            /// </summary>
            public string FailCode { get; set; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}