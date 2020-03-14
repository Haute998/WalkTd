using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_UserAdvice1 : BaseSearch
    {
        public string DatCreateB { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string DatCreateE { get; set; }
        public string keyword { get; set; }

        public static string AuditByIDs(int[] cklst)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');

            string strSql = string.Format("UPDATE [C_UserAdvice] SET state='已审核' ,DatCreate=@DatCreate WHERE ID in ({0});", userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@DatCreate", DateTime.Now.ToString()) };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            if (cnt > 0)
            {
                return "ok";
            }
            else
            {
                return "审核失败";
            }
        }
        public static string DelByIDs(int[] cklst)
        {
            string userIDs = "";
            for (int i = 0; i < cklst.Length; i++)
            {
                userIDs += cklst[i] + ",";
            }
            userIDs = userIDs.TrimEnd(',');

            string strSql = string.Format("UPDATE [C_UserAdvice] SET state='已删除' WHERE ID in ({0});", userIDs);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            if (cnt > 0)
            {
                return "ok";
            }
            else
            {
                return "删除失败";
            }
        }
        public static int GetUserCount()
        {
            string strSql = "SELECT count(*) FROM [C_UserAdvice] WHERE  state='已审核'";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
        public static string GetPieC_UserCountCensus()
        {
            string strSql = "select count(Contents) Count,Contents name from C_UserAdvice  where state='已审核' group by Contents order by Contents";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<C_CountCensus> list = DAL.EntityDataHelper.FillData2Entities<C_CountCensus>(strSql, paramters);
            List<PieCensus> censusList = new List<PieCensus>();
            int count = C_UserAdvice1.GetUserCount();
            foreach (C_CountCensus item in list)
            {
                censusList.Add(new PieCensus
                {
                    name = item.name,
                    y = item.Count % count
                });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(censusList);
        }
    }
    public partial class C_UserAdvice : BaseSearch
    {
        public string DatCreateB { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string DatCreateE { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public int sl { get; set; }
        public string ProductName { get; set; }
        public string XName { get; set; }
        public string CName { get; set; }
        public string TCName { get; set; }
        public string keyword { get; set; }

        public static int GetNoAuditCount()
        {
            string strSql = "select COUNT(*) From C_UserAdvice where State='未审核'";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }
    }
}
