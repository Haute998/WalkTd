using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.C_UserModel
{
    public class C_UserVMSearch : BaseSearch
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>

        public string wxNo { get; set; }
        /// <summary>
        /// 代理级别
        /// </summary>
        public string userTypeName { get; set; }
    }
    public class C_UserVM : C_User
    {
        /// <summary>
        /// 级别名称
        /// </summary>
        public string userTypeName { get; set; }
        /// <summary>
        /// 级别等级
        /// </summary>
        public int userTypeLever { get; set; }
        /// <summary>
        /// 上级名称
        /// </summary>
        public string ChiefName { get; set; }
        /// <summary>
        /// 推荐人 姓名
        /// </summary>
        public string IName { get; set; }

        public static C_UserVM GetVMByID(int id)
        {
            string strSql = @"SELECT C_User.*,isnull(C_UserType.Name,'') userTypeName,a.Name ChiefName,C_UserType.Lever userTypeLever,b.Name IName
                                     FROM [C_User]
                                     left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever 
                                     left join [C_User] a on [C_User].Chief=a.ID
                                     left join [C_User] b on [C_User].Introducer=b.UserName
                                     WHERE C_User.ID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserVM>(strSql, paramters);
        }
        /// <summary>
        /// 获取未审核数量
        /// </summary>
        /// <param name="id">上级ID</param>
        /// <returns></returns>
        public static int GetNoAuditCnt(int id)
        {
            string StrSq = "select COUNT(1) from C_User where state='未审核' and CHIEF=@ID";
            System.Data.SqlClient.SqlParameter[] parameter =
            {
                new System.Data.SqlClient.SqlParameter("@ID",id)
            };
            object rtn = DAL.SqlHelper.ExecuteScalar(StrSq, parameter);
            return rtn == null ? 0 : Convert.ToInt32(rtn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static C_UserVM GetVMByUserName(string UserName)
        {
            string strSql = @"SELECT C_User.*,isnull(C_UserType.Name,'') userTypeName,a.Name ChiefName,C_UserType.Lever userTypeLever,b.Name IName
                                     FROM [C_User]
                                     left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever 
                                     left join [C_User] a on [C_User].Chief=a.ID
                                     left join [C_User] b on [C_User].Introducer=b.UserName
                                     WHERE C_User.UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserVM>(strSql, paramters);
        }


        public static C_UserVM GetVMByUserNameOrPhone(string UserName)
        {
            string strSql = @"SELECT top 1 C_User.*,isnull(C_UserType.Name,'') userTypeName,a.Name ChiefName,C_UserType.Lever userTypeLever
                                     FROM [C_User]
                                     left join C_UserType on C_User.C_UserTypeID=C_UserType.Lever 
                                     left join [C_User] a on [C_User].Chief=a.ID
                                     WHERE C_User.UserName=@UserName or C_User.Phone=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<C_UserVM>(strSql, paramters);
        }

    }
}
