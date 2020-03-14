using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_UserRoles
    {
        /// <summary>
        /// 根据用户名获取用户角色关系表对象
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<B_UserRoles> GetEntitysByUserName(string UserName)
        {
            string strSql = "SELECT * FROM [B_UserRoles] where UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            return DAL.EntityDataHelper.FillData2Entities<B_UserRoles>(strSql, paramters);
        }

        public static B_UserRoles GetByUserName(string UserName)
        {
            string strSql = "SELECT * FROM [B_UserRoles] where UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            return DAL.EntityDataHelper.LoadData2Entity<B_UserRoles>(strSql, paramters);
        }
    }
}
