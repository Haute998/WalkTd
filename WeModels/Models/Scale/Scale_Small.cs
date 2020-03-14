using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Scale_Small
    {
        // 判断小标是否存在
        public static bool GetBoolSmallCode(string SmallCode)
        {
            string SqlStr = "SELECT COUNT(*) FROM SCALE WHERE SmallCode=@SmallCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@SmallCode",SmallCode)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? false : true;
        }


    }
}
