using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using System.Drawing;

namespace WeBusiness.Controllers
{
    public class SYSNavAdvController : BaseController
    {
        //
        // GET: /SYSNavAdv/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<SYSAdv> model = SYSAdv.GetNavAdvs();
            return View(model);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(SYSAdv model)
        {
            string base64 = Request["image"].ToString();

            string msg = "";
            //var file = Request.Files[0];
            //string path = Request.MapPath("~/");
            //string ext = Path.GetExtension(file.FileName);//获得文件扩展名
            //if (!Directory.Exists(Server.MapPath("~/images/SYSAdv/")))
            //{
            //    Directory.CreateDirectory(Server.MapPath("~/images/SYSAdv/"));
            //}
            //if (file.ContentLength == 0 || file == null)
            //{
            //    msg = "上传的图片没有内容！";
            //    TempData["ToAddSYSNavAdv_err"] = msg;
            //    return View("Add", model);
            //}
            //if (file.ContentLength > 5242880)
            //{
            //    msg = "上传图片不能超过5MB！";
            //    TempData["ToAddSYSNavAdv_err"] = msg;
            //    return View("Add", model);
            //}


            model.AdvType = 1;
            model.Name = "首页";
            model.ID = model.InsertAndReturnIdentity();
            string filePath = "/images/SYSAdv/";
            string fileName = "Adv_" + model.ID.ToString();

            try
            {
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(Server.MapPath("~/images/SYSAdv/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/SYSAdv/"));
                }

                if (!imghelper.Base64SaveImage(Server.MapPath("~") + filePath, base64, ref fileName))
                {
                    return Content("图片保存失败！");
                }
            }
            catch(Exception ex)
            {
                return Content(ex.Message);
            }

            model.ImgUrl = filePath + fileName;
            model.UpdateByID();
                
            return Content("ok");
        }

        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id)
        {
            SYSAdv model = SYSAdv.GetEntityByID(id);
            return View(model);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult ToEdit(SYSAdv model)
        {
            SYSAdv OldModel = SYSAdv.GetEntityByID(model.ID);
            string oldImgUrl = OldModel.ImgUrl;//原来的图片
            string msg = "";

            var file = Request.Files[0];
            if (file.ContentLength > 0 && file != null)
            {
                if (file.ContentLength > 5242880)
                {
                    msg = "上传图片不能超过5MB！";
                    TempData["ToEditSYSNavAdv_Msg"] = msg;
                    return View("Edit", model);
                }
                if (!Directory.Exists(Server.MapPath("~/images/SYSAdv/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/SYSAdv/"));
                }
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                OldModel.ImgUrl = "/images/SYSAdv/Adv_" + OldModel.ID + ext;
                string saveName = OldModel.ImgUrl;//实际保存文件名

                string delFile = Server.MapPath("~") + oldImgUrl;
                System.IO.File.Delete(delFile);

                file.SaveAs(path + saveName);
            }
            OldModel.AdvType = 1;
            OldModel.Name = "首页";
            OldModel.Url = model.Url;
            OldModel.Sort = model.Sort;
            if (OldModel.UpdateByID() > 0)
            {
                TempData["ToEditGoodsType_Msg"] = "ok";
            }
            else
            {
                TempData["ToEditGoodsType_Msg"] = "修改网站导航图片失败！";
            }
            return View("Edit", model);
        }
        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            int rtn = SYSAdv.DeleteByID(id);
            if (rtn > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("网络错误");
            }
        }
    }
}
