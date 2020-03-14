using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeModels.ModelService.Common;

namespace WeModels
{
    public partial class WXMaterial_Media
    {
        /// <summary>
        /// 更新图片素材从微信获取图文素材列表接口
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mapPath">服务器根路径</param>
        /// <param name="savePath">文件夹路径</param>
        /// <returns></returns>
        public static JsonData UpdateImgFromWxNewsLst(JsonData data, string mapPath, string savePath)
        {
            try
            {
                foreach (JsonData itemdata in data["item"])
                {
                    string media_id = itemdata["media_id"].ToString();

                    foreach (JsonData news_item in itemdata["content"]["news_item"])
                    {
                        WXMaterial_Media media = new WXMaterial_Media();
                        media.media_id = news_item["thumb_media_id"].ToString();
                        media.name = media.media_id + ".jpg";
                        media.type = "image";
                        media.update_time = Common.ConvertToDateTen(itemdata["content"]["update_time"].ToString());
                        media.wx_url = news_item["thumb_url"].ToString();
                        media.url = savePath + media.media_id + ".jpg";
                        news_item["bo_url"] = media.url;//赋值我们后台url

                        List<WXMaterial_Media> dbMedias = WXMaterial_Media.GetEntitysBymedia_id(media.media_id);
                        if (dbMedias == null || dbMedias.Count <= 0)
                        {
                            bool rtnsave = LoadImage.SavePhotoFromUrl(mapPath + media.url, media.wx_url);
                            media.InsertAndReturnIdentity();
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "UpdateImgFromWxLst_error");
                return data;

            }
            return data;
        }


        /// <summary>
        /// 更新图片素材从微信获取图文素材接口
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mapPath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public static JsonData UpdateImgFromWxNews(JsonData data, string mapPath, string savePath)
        {
            try
            {
                foreach (JsonData news_item in data["news_item"])
                {
                    WXMaterial_Media media = new WXMaterial_Media();
                    media.media_id = news_item["thumb_media_id"].ToString();
                    media.name = media.media_id + ".jpg";
                    media.type = "image";
                    media.wx_url = news_item["thumb_url"].ToString();
                    media.url = savePath + media.media_id + ".jpg";
                    news_item["bo_url"] = media.url;//赋值我们后台url

                    List<WXMaterial_Media> dbMedias = WXMaterial_Media.GetEntitysBymedia_id(media.media_id);
                    if (dbMedias == null || dbMedias.Count <= 0)
                    {
                        bool rtnsave = LoadImage.SavePhotoFromUrl(mapPath + media.url, media.wx_url);
                        media.InsertAndReturnIdentity();
                    }

                }

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "UpdateImgFromWxLst_error");
                return data;

            }
            return data;
        }



        /// <summary>
        /// 更新图片素材从微信获取图片素材列表接口
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mapPath">服务器根路径</param>
        /// <param name="savePath">文件夹路径</param>
        /// <returns></returns>
        public static JsonData UpdateImgFromWxImageLst(JsonData data, string mapPath, string savePath)
        {
            try
            {
                foreach (JsonData itemdata in data["item"])
                {
                    string media_id = itemdata["media_id"].ToString();

                    WXMaterial_Media media = new WXMaterial_Media();
                    media.media_id = media_id;
                    media.name = itemdata["name"].ToString();
                    media.type = "image";
                    media.update_time = Common.ConvertToDateTen(itemdata["update_time"].ToString());
                    media.wx_url = itemdata["url"].ToString();
                    media.url = savePath + media.media_id + ".jpg";
                    itemdata["bo_url"] = media.url;//赋值我们后台url

                    List<WXMaterial_Media> dbMedias = WXMaterial_Media.GetEntitysBymedia_id(media.media_id);
                    if (dbMedias == null || dbMedias.Count <= 0)
                    {
                        bool rtnsave = LoadImage.SavePhotoFromUrl(mapPath + media.url, media.wx_url);
                        media.InsertAndReturnIdentity();
                    }

                }

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "UpdateImgFromWxImageLst_error");
                return data;

            }
            return data;
        }








        /// <summary>
        /// 根据media_id获取素材
        /// </summary>
        /// <param name="media_id"></param>
        /// <returns></returns>
        public static List<WXMaterial_Media> GetEntitysBymedia_id(string media_id)
        {
            string strSql = "SELECT ID,media_id,name,update_time,wx_url,url,type FROM [WXMaterial_Media] WHERE media_id=@media_id";
            System.Data.SqlClient.SqlParameter[] paramters = { new System.Data.SqlClient.SqlParameter("@media_id", media_id) };

            return DAL.EntityDataHelper.FillData2Entities<WXMaterial_Media>(strSql, paramters);
        }
    }
}
