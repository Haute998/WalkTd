using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class InviteCode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static InviteCode GetListByUserName(string UserName,string type)
        {
            string strSql = "SELECT top 1 * FROM [InviteCode] WHERE UserName=@UserName and Type=@Type order by id desc";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName),
                                                             new System.Data.SqlClient.SqlParameter("@Type", type)};

            return DAL.EntityDataHelper.LoadData2Entity<InviteCode>(strSql, paramters);
        }

    }
}
