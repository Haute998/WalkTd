using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class TableViewConfig_rtn 
    {
        public string ShowName { get; set; }
        public int Sort { get; set; }
    }
    public partial class TableViewConfig
    {
        public static List<TableViewConfig> GetEntitys(string TableUrl, string UserName)
        {
            string strSql = "SELECT TOP 100000 * FROM [TableViewConfig] where TableUrl=@TableUrl and UserName=@UserName ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@TableUrl", TableUrl),
                                                              new System.Data.SqlClient.SqlParameter("@UserName", UserName)};

            return DAL.EntityDataHelper.FillData2Entities<TableViewConfig>(strSql, paramters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableUrl"></param>
        /// <param name="UserName"></param>
        /// <param name="ShowName"></param>
        /// <returns></returns>
        public static TableViewConfig GetEntity(string TableUrl, string UserName, string ShowName)
        {
            string strSql = "SELECT top 1 * FROM [TableViewConfig] WHERE TableUrl=@TableUrl and UserName=@UserName and ShowName=@ShowName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@TableUrl", TableUrl),
                                                              new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                              new System.Data.SqlClient.SqlParameter("@ShowName", ShowName)};

            return DAL.EntityDataHelper.LoadData2Entity<TableViewConfig>(strSql, paramters);
        }

        public static int Delete(string TableUrl, string UserName)
        {

            string strSql = string.Format("DELETE FROM [TableViewConfig] WHERE TableUrl=@TableUrl and UserName=@UserName");
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@TableUrl", TableUrl),
                                                              new System.Data.SqlClient.SqlParameter("@UserName", UserName)};

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
