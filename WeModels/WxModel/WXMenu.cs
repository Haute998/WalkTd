using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.WxModel;

namespace WeModels
{
    public partial class WXMenu
    {
        /// <summary>
        /// 清空所有
        /// </summary>
        /// <returns></returns>
        public static int DeleteAll()
        {
            string strSql = "DELETE FROM [WXMenu]";
            System.Data.SqlClient.SqlParameter[] paramters = null;

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 删除并删除子菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int DeleteAllByID(int id)
        {
            string strSql = "DELETE FROM [WXMenu] WHERE ID=@ID or ParentID=@ID";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@ID", id) };

            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }

        /// <summary>
        /// 根据菜单key获取菜单实例
        /// </summary>
        /// <param name="Menukey"></param>
        /// <returns></returns>
        public static WXMenu GetMenuByMenukey(string Menukey)
        {
            string strSql = "SELECT top 1 * FROM [WXMenu] WHERE Menukey=@Menukey";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@Menukey", Menukey) };

            return DAL.EntityDataHelper.LoadData2Entity<WXMenu>(strSql, paramters);
        }
        /// <summary>
        /// 修改内容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Edit(int id, string name, string value)
        {
            string strSql = string.Format("UPDATE [WXMenu] SET {0}='{1}' WHERE ID={2};", Common.Filter(name), Common.Filter(value), id);
            System.Data.SqlClient.SqlParameter[] paramters = null;
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 修改Media_id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Media_id"></param>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        public static int EditMedia(int id, string Media_id, string MediaType)
        {
            string strSql = string.Format("UPDATE [WXMenu] SET Media_id=@Media_id,MediaType=@MediaType WHERE ID=@ID;");
            System.Data.SqlClient.SqlParameter[] paramters = { 
                                                                 new System.Data.SqlClient.SqlParameter("@Media_id", Media_id),
                                                                 new System.Data.SqlClient.SqlParameter("@MediaType", MediaType),
                                                                 new System.Data.SqlClient.SqlParameter("@ID", id) 
                                                             };
            int cnt = DAL.SqlHelper.ExecuteNonQuery(strSql, paramters);
            return cnt;
        }
        /// <summary>
        /// 获取菜单个数
        /// </summary>
        /// <param name="ParentID">默认-1为全部</param>
        /// <returns></returns>
        public static int GetMenuCount(int ParentID = -1)
        {
            string strSql = string.Format(" select count(1) from [WXMenu] ");
            if (ParentID >= 0)
            {
                strSql += " where ParentID=" + ParentID;
            }
            System.Data.SqlClient.SqlParameter[] paramters = null;
            return (int)DAL.SqlHelper.ExecuteScalar(strSql, paramters);
        }


        /// <summary>
        /// 发布菜单到公众号  ok为成功
        /// </summary>
        /// <returns></returns>
        public static string ToPublish()
        {
            List<WXMenu> MenuList = WXMenu.GetEntitysAll();

            WxMenuJsonModel WxMenuJsonM = new WxMenuJsonModel();

            List<Button> buttonLst = new List<Button>();

            foreach (var ParentItem in MenuList.Where(m => m.ParentID == 0))
            {
                List<Button> buttonSonLst = new List<Button>();
                foreach (var SonItem in MenuList.Where(m => m.ParentID == ParentItem.ID))
                {
                    Button btnSon = new Button();
                    btnSon.name = SonItem.MenuName;
                    btnSon.key = SonItem.Menukey;
                    btnSon.media_id = SonItem.Media_id;
                    btnSon.type = SonItem.MenuType;
                    btnSon.url = SonItem.MenuUrl;
                    buttonSonLst.Add(btnSon);
                }
                Button btn = new Button();
                btn.name = ParentItem.MenuName;
                btn.key = ParentItem.Menukey;
                btn.media_id = ParentItem.Media_id;
                btn.type = ParentItem.MenuType;
                btn.url = ParentItem.MenuUrl;
                btn.sub_button = buttonSonLst;
                buttonLst.Add(btn);

            }
            WxMenuJsonM.button = buttonLst;

            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();

            string menuRtnStr = VariousApi.CreatMenu(WxMenuJsonM);
            return menuRtnStr;
        }
    }
}
