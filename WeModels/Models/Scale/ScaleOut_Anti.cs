using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class ScaleOut_Anti
    {
        public static bool IsAntiCodeOut(string AntiCode)
        {
            string SqlStr = "select count(*) From ScaleOut_Anti where AntiCode=@AntiCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@AntiCode",AntiCode)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }

    }
}
