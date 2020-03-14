using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.ModelService.Common;

namespace WeModels.WxModel
{
    public class WxMenuJsonModel
    {
        /// <summary>
        /// 一级菜单数组，个数应为1~3个
        /// </summary>
        public List<Button> button { get; set; }
    }
    public class Button
    {
        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        public List<Button> sub_button { get; set; }
        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过1024字节
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        public string media_id { get; set; }
    }


    public class WxMenuConvert
    {
        /// <summary>
        /// 获取微信自定义菜单到本地数据库
        /// </summary>
        /// <returns></returns>
        public static bool GetPublishMenuToDB()
        {
            try
            {
                WXMenu.DeleteAll();//先清空
                JsonData menudatas = new JsonData();
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                menudatas = VariousApi.GetMenus();

                if (JsonDataHelper.IsContains(menudatas, "errcode") == false)
                {
                    foreach (JsonData button in menudatas["menu"]["button"])
                    {
                        WXMenu menuParent = new WXMenu();
                        menuParent.MenuName = button["name"].ToString();
                        menuParent.Media_id = JsonDataHelper.GetValue(button, "media_id");
                        menuParent.Menukey = JsonDataHelper.GetValue(button, "key");
                        menuParent.MenuType = JsonDataHelper.GetValue(button, "type");
                        menuParent.MenuUrl = JsonDataHelper.GetValue(button, "url");
                        menuParent.ParentID = 0;
                        int parentID = menuParent.InsertAndReturnIdentity();

                        try
                        {
                            foreach (JsonData sub_button in button["sub_button"])
                            {
                                WXMenu menu = new WXMenu();
                                menu.MenuName = sub_button["name"].ToString();
                                menu.Media_id = JsonDataHelper.GetValue(sub_button, "media_id");
                                menu.Menukey = JsonDataHelper.GetValue(sub_button, "key");
                                menu.MenuType = JsonDataHelper.GetValue(sub_button, "type");
                                menu.MenuUrl = JsonDataHelper.GetValue(sub_button, "url");
                                menu.ParentID = parentID;
                                menu.InsertAndReturnIdentity();
                            }
                        }
                        catch { }
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "GetPublishMenuToDB_error");
                return false;
            }
        }
    }
}
