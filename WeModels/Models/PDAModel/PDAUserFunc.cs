using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class PDAUserFunc
    {
        public static bool UpdateUserFunc(int UserId, string FuncCodeSet)
        {
            string strSql = string.Empty;

            strSql = "delete PDAUserFunc where UserId=@UserId;";
            if (!string.IsNullOrWhiteSpace(FuncCodeSet))
            {
                strSql += "insert into PDAUserFunc(UserId,FunCode)select @UserId,FunCode from PDAFuntion where FunCode in (" + FuncCodeSet + ");";
            }  

            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserId", UserId) };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0 ? true : false;
        }

        public static List<PDAUserFuncEasy> GetEntitysUserId(int Id)
        {
            string strSql = "select b.FunCode,b.FunName from PDAUserFunc as a left join PDAFuntion as b on a.FunCode=b.FunCode where a.IsEnable=1 and UserId=@UserId";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserId", Id) };

            return DAL.EntityDataHelper.FillData2Entities<PDAUserFuncEasy>(strSql, paramters);
        }
    }
}
