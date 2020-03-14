using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class GetPrize
    {
        public static int GetCodeRepeat(string Code)
        {
            string SqlStr = "Select count(*) from GetPrize where Code=@Code";
            System.Data.SqlClient.SqlParameter[] SqlParameter ={
new  System.Data.SqlClient.SqlParameter("@Code",Code)
                                                              };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));

        }
        public static GetPrize GetGetPrize(string Code)
        {
            string SqlStr = "Select * from GetPrize where Code=@Code";
            System.Data.SqlClient.SqlParameter[] SqlParameter ={
new  System.Data.SqlClient.SqlParameter("@Code",Code)
                                                              };
            return DAL.EntityDataHelper.LoadData2Entity<GetPrize>(SqlStr, SqlParameter);

        }
       
    }
}
