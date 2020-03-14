using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public partial class B_Menu
    {

        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<B_Menu> SubMenuList { get; set; }
        private static void RecursiveEntitys(B_Menu org, List<B_Menu> list)
        {
            List<B_Menu> tmpList = list.FindAll(m => m.ParentID == org.ID);
            if (tmpList.Count > 0)
            {
                org.SubMenuList = tmpList;
                foreach (B_Menu tmp in tmpList)
                {
                    RecursiveEntitys(tmp, list);
                }
            }
        }

        /// <summary>
        /// 获取栏目
        /// </summary>
        /// <param name="isLoadSubMenuLst"></param>
        /// <returns></returns>
        public static List<B_Menu> GetMainMenus()
        {
            string strSql = string.Format("SELECT * FROM [B_Menu] WHERE IsValid=1 and ParentID=0 ORDER BY Sort Desc");
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<B_Menu> menus = DAL.EntityDataHelper.FillData2Entities<B_Menu>(strSql, paramters);
            return menus;
        }

        /// <summary>
        /// 获取下级菜单
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public static List<B_Menu> GetChildMenus(int ParentID = 0)
        {
            string strSql = "SELECT * FROM [B_Menu] WHERE ParentID=@ParentID and IsValid=1 ORDER BY Sort Desc";
            System.Data.SqlClient.SqlParameter[] paramters = {
                  new System.Data.SqlClient.SqlParameter("@ParentID",ParentID)                               
            };
            return DAL.EntityDataHelper.FillData2Entities<B_Menu>(strSql, paramters);
        }
        /// <summary>
        /// 获取显示的菜单
        /// </summary>
        /// <param name="isLoadSubMenuLst"></param>
        /// <returns></returns>
        public static List<B_Menu> GetShowMenus(bool isLoadSubMenuLst = false)
        {
            string strSql = string.Format("SELECT * FROM [B_Menu] WHERE IsValid=1 and IsShow=1 ORDER BY Sort Desc");
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<B_Menu> menus = DAL.EntityDataHelper.FillData2Entities<B_Menu>(strSql, paramters);
            if (isLoadSubMenuLst == false)
            {
                return menus;
            }
            List<B_Menu> listResult = new List<B_Menu>();
            foreach (B_Menu menu in menus.FindAll(m => m.ParentID == 0))
            {
                RecursiveEntitys(menu, menus);
                listResult.Add(menu);
            }
            return listResult;
        }

        /// <summary>
        /// 获取有效菜单
        /// </summary>
        /// <param name="haveSub">是否要下级菜单</param>
        /// <returns></returns>
        public static List<B_Menu> GetValidMenus(bool isLoadSubMenuLst=false)
        {
            string strSql = string.Format("SELECT * FROM [B_Menu] WHERE IsValid=1 ORDER BY Sort Desc");
            System.Data.SqlClient.SqlParameter[] paramters = null;
            List<B_Menu> menus= DAL.EntityDataHelper.FillData2Entities<B_Menu>(strSql, paramters);
            if (isLoadSubMenuLst == false) 
            {
                return menus;
            }
            List<B_Menu> listResult = new List<B_Menu>();
            foreach (B_Menu menu in menus.FindAll(m => m.ParentID == 0))
            {
                RecursiveEntitys(menu, menus);
                listResult.Add(menu);
            }
            return listResult;
        }

        /// <summary>
        /// 删除菜单和所有菜单权限
        /// </summary>
        /// <param name="id"></param>
        public static bool DeleteAll(int id)
        {
            if (id == 0)
            {
                return false;
            }
            B_Menu dbBOMenu = B_Menu.GetEntityByID(id);
            if (dbBOMenu == null)
            {
                return false;
            }
            using (System.Data.SqlClient.SqlConnection conn = DAL.SqlHelper.DefaultConnection)
            {
                conn.Open();
                System.Data.SqlClient.SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    List<B_Menu> muList = B_Menu.GetChildMenus(id);
                    muList.Insert(0, dbBOMenu);
                    List<B_MenuRights> tpList = B_MenuRights.GetEntitysAll();
                    foreach (B_Menu menu in muList)
                    {
                        if (DeleteByID(tran, menu.ID) > 0)
                        {
                            StringBuilder sbDeleteIds = new StringBuilder();
                            foreach (B_MenuRights tRights in tpList.FindAll(m => m.MenuID == menu.ID))
                            {
                                sbDeleteIds.Append(tRights.ID + ",");
                            }
                            string ids = sbDeleteIds.ToString().TrimEnd(',');
                            if (ids != string.Empty)
                            {
                                string strSql = "DELETE FROM [B_RoleRights] WHERE RightID IN (" + ids + ")";
                                DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);
                                strSql = "DELETE FROM [B_MenuRights] WHERE ID IN (" + ids + ")";
                                DAL.SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, strSql);
                            }
                        }
                    }
                    tran.Commit();
                    return true;
                }
                catch (System.Data.SqlClient.SqlException exSql)
                {
                    tran.Rollback();
                    DAL.Log.Instance.Write(exSql.ToString(), "B_MenuRights_RelevanceList");
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
            return false;
        }
    }
}
