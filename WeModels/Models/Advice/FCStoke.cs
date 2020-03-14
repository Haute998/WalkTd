using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class FCStoke : BaseSearch
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

            string strSql = string.Format("UPDATE [C_UserAdvice] SET state2='已审核' ,DatCreate=@DatCreate WHERE ID in ({0});", userIDs);
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

            string strSql = string.Format("UPDATE [C_UserAdvice] SET state2='已删除' WHERE ID in ({0});", userIDs);
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
            string strSql = "SELECT count(*) FROM [C_UserAdvice] WHERE  state2='已审核'";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(strSql, paramters));
        }
        public static string GetPieC_UserCountCensus()
        {
            string strSql = "select count(Contents) Count,Contents name from C_UserAdvice  where state2='已审核' group by Contents order by Contents";
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
    public partial class FCStoke : BaseSearch
    {


        ///// <summary>
        /// 多条件
        /// </summary>
        public static FCStoke GetEntityByIDname(string NName,string pname)
        {
         
                string strSql = "SELECT sl from FCStoke where name=@name and pname=@pname";
                System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@name", NName), new System.Data.SqlClient.SqlParameter("@pname", pname) };
                
                return DAL.EntityDataHelper.LoadData2Entity<FCStoke>(strSql, paramters);
         
        }
        public static C_UserAdvice UpdateByIDSN(string NName)
        {
            string strSql = "UPDATE [C_UserAdvice] SET OrderNo='2018' WHERE SN in (select smallcode  from ScaleOutStoke where Consignee='" + NName + "' ) ;";

         

            return DAL.EntityDataHelper.LoadData2Entity<C_UserAdvice>(strSql, null);

        }
        public static int GetUpdateScaleInState(int sl,string name)
        {
            string SqlStr = "Update FCStoke set sl=@sl WHERE name=@name";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                      new System.Data.SqlClient.SqlParameter("@sl",sl),
                       new System.Data.SqlClient.SqlParameter("@name",name)
             };
            return DAL.SqlHelper.ExecuteNonQuery(SqlStr, Parameter);
        }

    }
}
