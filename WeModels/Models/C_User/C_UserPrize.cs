using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class C_UserPrize : BaseSearch
    {
        public string keyword { get; set; }

        public static int GetCuserPrize(string code)
        {
            string SqlStr = "Select count(*) from C_UserPrize where AntiCode=@code";
            System.Data.SqlClient.SqlParameter[] SqlParameter ={
                                                                  new System.Data.SqlClient.SqlParameter("@code",code)
                                                              };
           return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));

        }
    }
}
