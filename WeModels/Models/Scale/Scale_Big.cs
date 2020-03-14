using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class Scale_Big
    {
        public static int InsertScaleBigCode(string MiddleCode)
        {
            string strSql = "insert into Scale_Big(BigCode,ScaleId) select BigCode,ID from Scale where ID in(select ScaleId from Scale_Middle where MiddleCode=@MiddleCode)";
            System.Data.SqlClient.SqlParameter[] paramters ={
                 new System.Data.SqlClient.SqlParameter("@MiddleCode",MiddleCode)
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        public static int DeleteScaleBigCode(string MiddleCode)
        {
            string strSql = "delete Scale_Middle where ScaleId in (select ScaleId from Scale_Middle where MiddleCode=@MiddleCode)";
            System.Data.SqlClient.SqlParameter[] paramters ={
                 new System.Data.SqlClient.SqlParameter("@MiddleCode",MiddleCode)
            };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
    }
}
