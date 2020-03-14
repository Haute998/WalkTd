using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_UserDesktopAuth
    {
        public static List<B_UserDesktopAuth> GetEntitysMainRoleID(int ID, string UserName)
        {
            string strSql = "SELECT * FROM [B_UserDesktopAuth] WHERE RoleID=@ID and IsShow='True' and UserName=@UserName ";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", ID),
                                                               new System.Data.SqlClient.SqlParameter("@UserName", UserName)};

            return DAL.EntityDataHelper.FillData2Entities<B_UserDesktopAuth>(strSql, paramters);
        }



        public static B_UserDesktopAuth GetEntitysMain(int RoleID, int MainID, string UserName)
        {
            string strSql = "SELECT * FROM [B_UserDesktopAuth] WHERE RoleID=@RoleID and MainID=@MainID and UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@RoleID", RoleID),
                                                               new System.Data.SqlClient.SqlParameter("@MainID", MainID),
                                                               new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.LoadData2Entity<B_UserDesktopAuth>(strSql, paramters);
        }


        public static List<BaseMain> GetUserDesktopAuth(int RoleID, string UserName)
        {
            string strSql = @"select distinct a.*,cast(isnull((select top 1 MainID from B_UserDesktopAuth where UserName=@UserName and MainID=b.MainID),0)as bit) as IsCheck
                                from BaseMain as a left join B_RolesDesktopAuth as b on a.ID=b.MainID
                                where b.RoleID=@RoleID";
            System.Data.SqlClient.SqlParameter[] paramters = {
                                                                 new System.Data.SqlClient.SqlParameter("@RoleID", RoleID),
                                                                 new System.Data.SqlClient.SqlParameter("@UserName", UserName)
                                                             };
            return DAL.EntityDataHelper.FillData2Entities<BaseMain>(strSql, paramters);
        }

        public static List<BaseMain> GetRolesDesktopAuth(int RoleID)
        {
            string strSql = @"select distinct a.*,cast((case when b.MainID IS NULL then 0 else 1 end)as bit) as IsCheck
                            from BaseMain as a left join B_RolesDesktopAuth as b on a.ID=b.MainID and b.RoleID=@RoleID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@RoleID", RoleID) };
            return DAL.EntityDataHelper.FillData2Entities<BaseMain>(strSql, paramters);
        }
        public static List<BaseMain> GetEntitysMainUserNames(string UserName)
        {

            string strSql = "select * from BaseMain where  ID in(select MainID from B_UserDesktopAuth where UserName=@UserName and  IsShow='True' and RoleID=(select RoleID from  B_UserRoles  where  UserName=@UserName))";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

            return DAL.EntityDataHelper.FillData2Entities<BaseMain>(strSql, paramters);
        }

        public static BaseMain GetEntitysByStringMainID(string MainID)
        {

            string strSql = "SELECT ID,MainID,ShowName,Sort FROM [BaseMain] WHERE MainID=@MainID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@MainID", MainID) };

            return DAL.EntityDataHelper.LoadData2Entity<BaseMain>(strSql, paramters);
        }

        public static bool RemoveUserDesktopAuth(int MainID, string UserName)
        {
            string strSql = "delete B_UserDesktopAuth where UserName=@UserName and MainID=@MainID";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@MainID",MainID),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0 ? true : false;
        }
    }
}
