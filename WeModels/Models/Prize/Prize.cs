using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Prize
    {
        public static int GetLeverRepeat(string Lever)
        {
            string SqlStr = "Select count(*) from Prize where Lever=@Lever";
            System.Data.SqlClient.SqlParameter[] SqlParameter ={
new  System.Data.SqlClient.SqlParameter("@Lever",Lever)
                                                              };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));

        }
        public static int GetEditLeverRepeat(string Lever, int ID)
        {
            string SqlStr = "Select count(*) from Prize where Lever=@Lever and ID!=@ID";
            System.Data.SqlClient.SqlParameter[] SqlParameter ={
new  System.Data.SqlClient.SqlParameter("@Lever",Lever),
new  System.Data.SqlClient.SqlParameter("@ID",ID)
                                                              };
            return Convert.ToInt32(DAL.SqlHelper.ExecuteScalar(SqlStr, SqlParameter));

        }
    }
}
