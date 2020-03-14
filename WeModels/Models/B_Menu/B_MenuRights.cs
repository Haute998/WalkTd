using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_MenuRights
    {
        public int ParentID { get; set; }
        private string[] _MethodCodeInheritsArray = null;
        public bool HasInheritMethodCode(string methodCode)
        {
            if (_MethodCodeInheritsArray == null)
            {
                _MethodCodeInheritsArray = MethodCodeMain.Split(',');
            }
            return _MethodCodeInheritsArray.Any(code => code.ToLower() == methodCode.ToLower());
        }
        /// <summary>
        /// 所有字段
        /// </summary>
        private const string AllFields = "ID,MenuID,CodeOn,MethodCode,MethodName,MethodCodeMain";
        /// <summary>
        /// 菜单权限
        /// </summary>
        private static ConcurrentDictionary<string, List<B_MenuRights>> UserMenuRights { get; set; }

        static B_MenuRights()
        {
            UserMenuRights = new ConcurrentDictionary<string, List<B_MenuRights>>();
        }

        /// <summary>
        /// 清除菜单权限(内存)
        /// </summary>
        public static void ClearHashMenuRights()
        {
            UserMenuRights.Clear();
        }
        /// <summary>
        /// 根据控制器和方法获取菜单名称
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static string GetMenuName(string controllerName, string actionName)
        {
            string strSql = "SELECT TOP 1 b.MenuName FROM [B_MenuRights] as a inner join [B_Menu] as b on a.MenuID=b.ID WHERE a.CodeOn=@CodeOn and a.MethodCode=@MethodCode";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@CodeOn",controllerName),
                new System.Data.SqlClient.SqlParameter("@MethodCode",actionName)
            };
            object obj = DAL.SqlHelper.ExecuteScalar(strSql, paramters);
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToString();
            }
        }
        /// <summary>
        /// 根据控制器和方法获取菜单对象
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static B_Menu GetMenu(string controllerName, string actionName)
        {
            string strSql = "SELECT TOP 1 b.* FROM [B_MenuRights] as a inner join [B_Menu] as b on a.MenuID=b.ID WHERE a.CodeOn=@CodeOn and a.MethodCode=@MethodCode";
            System.Data.SqlClient.SqlParameter[] paramters ={
                new System.Data.SqlClient.SqlParameter("@CodeOn",controllerName),
                new System.Data.SqlClient.SqlParameter("@MethodCode",actionName)
            };
            return DAL.EntityDataHelper.LoadData2Entity<B_Menu>(strSql, paramters);
        }
        /// <summary>
        /// 获取排序列表
        /// </summary>
        public static List<B_MenuRights> GetSortEntitysAll()
        {
            string strSql = "SELECT TOP 100000 " + AllFields + " FROM [B_MenuRights] WHERE MenuID>0 ORDER BY MenuID";
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return DAL.EntityDataHelper.FillData2Entities<B_MenuRights>(strSql, paramters);
        }

        /// <summary>
        /// 获取指定用户下的菜单权限(内存)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<B_MenuRights> GetHashMenuRights(string userName)
        {
            return UserMenuRights.GetOrAdd(userName, funcUserName =>
            {
                string strSql = "SELECT " + AllFields + " FROM [B_MenuRights] WHERE ID IN (SELECT RightID FROM [B_RoleRights] WHERE  RoleID IN (SELECT a.RoleID FROM [B_UserRoles] as a inner join B_Role as b on a.RoleID=b.ID WHERE a.UserName=@UserName and b.IsValid =1))";
                System.Data.SqlClient.SqlParameter paramter = new System.Data.SqlClient.SqlParameter("@UserName", funcUserName);
                return DAL.EntityDataHelper.FillData2Entities<B_MenuRights>(strSql, paramter);
            });
        }
        /// <summary>
        /// 读取菜单权限列表
        /// </summary>
        public static Dictionary<int, List<B_MenuRights>> GetEntitysDictionary()
        {
            Dictionary<int, List<B_MenuRights>> dicRights = new Dictionary<int, List<B_MenuRights>>();
            List<B_MenuRights> list = B_MenuRights.GetSortEntitysAll();
            List<B_MenuRights> tmpList = null;
            int tmpMenuID = 0;
            foreach (B_MenuRights r in list)
            {
                if (r.MenuID != tmpMenuID)
                {
                    if (tmpMenuID > 0)
                    {
                        dicRights.Add(tmpMenuID, tmpList);
                    }
                    tmpMenuID = r.MenuID;
                    tmpList = new List<B_MenuRights>();
                    tmpList.Add(r);
                }
                else
                {
                    tmpList.Add(r);
                }
            }
            if (tmpList != null)
            {
                dicRights.Add(tmpMenuID, tmpList);
            }
            return dicRights;
        }
        /// <summary>
        /// 移除指定用户下的菜单权限(内存)
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool RemoveHashMenuRights(string userName)
        {
            List<B_MenuRights> menuRights = null;
            return UserMenuRights.TryRemove(userName, out menuRights);
        }

        /// <summary>
        /// 关联权限列表
        /// </summary>
        /// <param name="menuid"></param>
        /// <param name="muList"></param>
        /// <param name="IsDeleteRights"></param>
        /// <returns></returns>
        public static bool RelevanceList(int menuid, List<B_MenuRights> newRights)
        {
            if (menuid <= 0)
            {
                return false;
            }
            StringBuilder DeleteRightIds = new StringBuilder();//需要删除的权限
            List<B_MenuRights> oldRights = B_MenuRights.GetEntitysByMenuID(menuid);//原来所有的权限
            List<B_MenuRights> AddNewRights = new List<B_MenuRights>();//新加的权限
            foreach (B_MenuRights newRight in newRights)
            {
                //新加的权限
                if (!oldRights.Exists(m => m.MethodName == newRight.MethodName))
                {
                    AddNewRights.Add(newRight);
                }
            }

            foreach (B_MenuRights oldRight in oldRights)
            {
                if (!newRights.Exists(m => m.MethodName == oldRight.MethodName))
                {
                    //删掉的权限
                    DeleteRightIds.Append(oldRight.ID + ",");
                }
            }



            int result = 0;
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();
                string ids = DeleteRightIds.ToString().TrimEnd(',');

                try
                {
                    if (ids != string.Empty)
                    {
                        string strSql = "DELETE FROM [B_RoleRights] WHERE RightID IN (" + ids + ")";
                        DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);
                        strSql = "DELETE FROM [B_MenuRights] WHERE ID IN (" + ids + ")";
                        DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);
                    }

                    foreach (B_MenuRights r in AddNewRights)
                    {
                        r.MenuID = menuid;
                        result += r.InsertAndReturnIdentity(tran);

                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(ex.ToString(), "B_MenuRights_RelevanceList_Ex");
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }
            }
            return result > 0;

        }
        /// <summary>
        ///获得权限管理
        /// </summary>
        /// <returns></returns>
        public static List<B_MenuRights> GetRoleUser(int RoleID)
        {
            string SqlStr = "select B_MenuRights.*,B_Menu.ParentID ParentID from B_RoleRights left join B_MenuRights on  B_RoleRights.RightID=B_MenuRights.ID left join B_Menu on B_MenuRights.MenuID=B_Menu.ID where B_RoleRights.RoleID=@RoleID";
                System.Data.SqlClient.SqlParameter[] SqlParameter={
                                                                  new System.Data.SqlClient.SqlParameter("@RoleID",RoleID) };
           return  DAL.EntityDataHelper.FillData2Entities<B_MenuRights>(SqlStr,SqlParameter);
        }
    }
}
