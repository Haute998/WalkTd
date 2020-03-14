using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDAAgentApp
    {
        public static int UpdateIsOK()
        {
            string strSql = "UPDATE [PDAAgentApp] SET IsOK=0 ";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int SetIDIsOK(int ID)
        {
            string strSql = "UPDATE [PDAAgentApp] SET IsOK=1 where ID=@ID ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID) };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static PDAAgentApp GetDefultScanApp()
        {
            string strSql = "SELECT top 1 ID,IsOK,Ver,AppName,Size,AppPath,CreatTime FROM [PDAAgentApp] WHERE IsOK=1";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            return DAL.EntityDataHelper.LoadData2Entity<PDAAgentApp>(strSql, paramters);
        }
    }
}
