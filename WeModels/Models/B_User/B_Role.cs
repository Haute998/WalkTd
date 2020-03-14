using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_Role
    {
        public int RoleID { get; set; }
        /// <summary>
        /// 获得用户的角色
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<B_Role> GetUserRoles(string UserName)
        {
            string strSql = "select B_Role.* ,B_UserRoles.RoleID RoleID from B_UserRoles left join B_Role on  B_UserRoles.RoleID=B_Role.ID where B_UserRoles.UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            return DAL.EntityDataHelper.FillData2Entities<B_Role>(strSql, paramters);
        }
        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static int DelUserRoleByUserName(string UserName)
        {
            string strSql = string.Format("delete from B_UserRoles where UserName=@UserName");
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return obj == null ? 0 : Convert.ToInt32(obj);

        }

        /// <summary>
        /// 更新用户角色【就每个用户只有一个角色】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public static bool EditUserRole(string UserName, int roleID)
        {
            List<B_UserRoles> userRoles = B_UserRoles.GetEntitysByUserName(UserName);
            if (userRoles.Count(m => m.RoleID == roleID) > 0)
            {
                return true;
            }
            else
            {
                string strSql = "DELETE FROM [B_UserRoles] WHERE UserName=@UserName";
                System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@UserName", UserName) };

                DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);//删除原来的角色

                if (roleID != 0)
                {
                    B_UserRoles newRole = new B_UserRoles();
                    newRole.UserName = UserName;
                    newRole.RoleID = roleID;
                    if (newRole.InsertAndReturnIdentity() <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 关联角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idRightsList"></param>
        /// <returns></returns>
        public static bool RelevanceRightsList(int id, List<int> idRightsList)
        {

            if (idRightsList.Count == 0)
            {
                return false;
            }
            StringBuilder sbIdList = new StringBuilder();
            foreach (int tid in idRightsList)
            {
                sbIdList.Append(tid + ",");
            }
            string idlist = sbIdList.ToString().TrimEnd(',');

            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    string strSql = "DELETE FROM [B_RoleRights] WHERE RoleID=" + id;
                    DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);

                    foreach (int rid in idRightsList)
                    {
                        new B_RoleRights
                        {
                            RoleID = id,
                            RightID = rid
                        }.InsertAndReturnIdentity(tran);
                    }
                    tran.Commit();
                    return true;
                }
                catch (System.Data.SqlClient.SqlException exSql)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(exSql.ToString(), "BORoles_RelevanceRightsList");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "BORoles_RelevanceRightsList_Ex");
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return false;
        }

        /// <summary>
        /// 角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idRightsList"></param>
        /// <returns></returns>
        public static bool RelevanceRightsLists(string UserName)
        {
            string strSql = @"delete from B_UserRoles where UserName=@UserName";
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        /// <summary>
        /// 设置角色桌面权限
        /// </summary>
        /// <param name="AuthIDSet"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public static bool SetDesktopAuth(string AuthIDSet, int RoleID)
        {
            string strSql = "delete B_RolesDesktopAuth where RoleID=@RoleID;" +
                            "delete B_UserDesktopAuth where RoleID=@RoleID;";
            if (!string.IsNullOrWhiteSpace(AuthIDSet))
            {
                strSql += "insert into B_RolesDesktopAuth(RoleID,MainID) select @RoleID,ID from BaseMain where ID in(" + AuthIDSet + ");" +
                        "insert into B_UserDesktopAuth(MainID,RoleID,UserName,IsShow)" +
                        "select b.MainID,b.RoleID,a.UserName,1 from B_User as a left join B_RolesDesktopAuth as b on a.RoleID=b.RoleID where a.RoleID=@RoleID and b.RoleID is not null;";
            }

            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@RoleID",RoleID)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0 ? true : false;
        }

        /// <summary>
        /// 设置用户桌面权限
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="UserName"></param>
        /// <param name="IDSet"></param>
        /// <returns></returns>
        public static bool SetUserDesktopAuth(int RoleID, string UserName, string IDSet)
        {
            string strSql = @"delete B_UserDesktopAuth where RoleID=@RoleID and UserName=@UserName;";
            if (!string.IsNullOrWhiteSpace(IDSet))
            {
                strSql+=@"insert into B_UserDesktopAuth(MainID,RoleID,UserName,IsShow)
                select MainID,@RoleID,@UserName,1 from B_RolesDesktopAuth where RoleID=@RoleID and MainID in(" + IDSet + ")";
            }
            System.Data.SqlClient.SqlParameter[] paramters = {
                new System.Data.SqlClient.SqlParameter("@RoleID",RoleID),
                new System.Data.SqlClient.SqlParameter("@UserName",UserName)
            };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt > 0 ? true : false;
        }
    }
}
