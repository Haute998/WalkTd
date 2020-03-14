using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Scale_Middle
    {
        public static int DeleteScaleMiddleCode(int ScaleId)
        {
            string strSql = "delete Scale_Middle where ScaleId=@ScaleId";
            System.Data.SqlClient.SqlParameter[] paramters ={
                 new System.Data.SqlClient.SqlParameter("@ScaleId",ScaleId)
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
