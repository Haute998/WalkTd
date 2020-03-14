using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using LitJson;
using WeModels;

namespace WeBusiness.Controllers
{
    public class wxMaterialController : BaseController
    {
        [B_MenuRightsTag("素材列表")]
        public ActionResult CheckLst(int menuid)
        {
            ViewData["menuid"] = menuid;
            return View();
        }

        /// <summary>
        /// 图文素材
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("图文素材")]
        public ActionResult news()
        {
            return View();
        }
        [B_MenuRightsTag("图文素材列表", "news")]
        public ActionResult getnews(int offset, int count)
        {
            string mapPath = Server.MapPath("~");//服务器根路径
            string savePath = "/Files/WXMaterial_Media/images/";//文件夹路径
            if (!Directory.Exists(mapPath + savePath))
            {
                Directory.CreateDirectory(mapPath + savePath);
            }

            JsonData sendRtn = new JsonData();
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            sendRtn = VariousApi.GetMaterials("news", offset, count);
            WXMaterial_Media.UpdateImgFromWxNewsLst(sendRtn, mapPath, savePath);


            string json = sendRtn.ToJson();
            return Content(json);
        }
        [B_MenuRightsTag("删除图文素材", "news")]
        public ContentResult del(string media_id)
        {
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            bool rtn = VariousApi.DelMedia(media_id);
            return Content(rtn ? "1" : "0");
        }


        [B_MenuRightsTag("图片素材")]
        public ActionResult image()
        {
            return View();
        }

        [B_MenuRightsTag("图片素材列表", "image")]
        public ActionResult getimage(int offset, int count)
        {
            string mapPath = Server.MapPath("~");//服务器根路径
            string savePath = "/Files/WXMaterial_Media/images/";//文件夹路径
            if (!Directory.Exists(mapPath + savePath))
            {
                Directory.CreateDirectory(mapPath + savePath);
            }

            JsonData sendRtn = new JsonData();
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            sendRtn = VariousApi.GetMaterials("image", offset, count);
            WXMaterial_Media.UpdateImgFromWxImageLst(sendRtn, mapPath, savePath);


            string json = sendRtn.ToJson();
            return Content(json);
        }

        [B_MenuRightsTag("获取单个素材")]
        public ActionResult getonemedia(string media_id, string mediatype)
        {



            JsonData sendRtn = new JsonData();
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            sendRtn = VariousApi.GetMaterial(media_id);


            if (mediatype == "news")
            {
                string mapPath = Server.MapPath("~");//服务器根路径
                string savePath = "/Files/WXMaterial_Media/images/";//文件夹路径
                if (!Directory.Exists(mapPath + savePath))
                {
                    Directory.CreateDirectory(mapPath + savePath);
                }
                WXMaterial_Media.UpdateImgFromWxNews(sendRtn, mapPath, savePath);
            }






            string json = sendRtn.ToJson();
            return Content(json);
        }
    }
}
