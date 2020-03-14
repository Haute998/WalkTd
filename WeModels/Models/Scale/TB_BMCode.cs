using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class TB_BMCode
    {
        public static bool GetBoolBarCode(string BarCode,int TypeId)
        {
            string SqlStr = "SELECT COUNT(*) FROM TB_BMCode WHERE (TypeId=0 or TypeId=@TypeId) and BarCode=@BarCode";
            System.Data.SqlClient.SqlParameter[] Parameter ={
                 new System.Data.SqlClient.SqlParameter("@BarCode",BarCode),
                 new System.Data.SqlClient.SqlParameter("@TypeId",TypeId),
            };
            object obj = DAL.SqlHelper.ExecuteScalar(SqlStr, Parameter);
            return Convert.ToInt32(obj) > 0 ? true : false;
        }
    }
}
