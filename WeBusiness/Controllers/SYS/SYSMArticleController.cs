using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSMArticleController : BaseController
    {
        //
        // GET: /SYSMArticle/
         [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

         private string StrWhere(SYSMArticleSearch condition)
         {
             string where = string.Empty;
             if (!string.IsNullOrWhiteSpace(condition.Title))
             {
                 where += string.Format(" and Title ='{0}'", condition.Title);
             }
             return where;
         }
         [B_MenuRightsTag("查看", "Index")]
         public ActionResult GetPage(SYSMArticleSearch condition)
         {
             string where = StrWhere(condition);
             PageJsonModel<SYSMArticle> page = new PageJsonModel<SYSMArticle>();
             page.pageIndex = condition.pageIndex;
             page.pageSize = condition.pageSize;
             page.strForm = " SYSMArticle ";
             page.strSelect = " * ";
             page.strWhere = where;
             page.strOrder = "DatEdit desc";
             page.LoadList();

             return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
         }


         [B_MenuRightsTag("添加", "Index")]
         public ActionResult Add()
         {
             SYSMArticle article = new SYSMArticle();
             article.DatCreate = DateTime.Now;
             article.DatEdit = DateTime.Now;
             article.ID=article.InsertAndReturnIdentity();
             return View("Edit",article);
         }
         [B_MenuRightsTag("修改", "Index")]
         public ActionResult Edit(int id)
         {
             SYSMArticle para = SYSMArticle.GetEntityByID(id);
             return View(para);
         }
         [B_MenuRightsTag("修改", "Index")]
         public ContentResult ToSave(SYSMArticle para)
         {
             if (string.IsNullOrWhiteSpace(para.Title))
             {
                 return Content("素材标题不能为空");
             }
             SYSMArticle oldPara = SYSMArticle.GetEntityByID(para.ID);
             HttpPostedFileBase upfile = Request.Files["Img"];
             if (upfile != null)
             {

                 //判断文件大小是否符合要求  
                 if (upfile.ContentLength >= (5242880))
                 {
                     return Content("fail|请上传5M以内的文件！");
                 }
                 string imgName = DateTime.Now.ToString("yyyy-MM-dd-HH-ss") + DateTime.Now.Ticks;
                 string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
                 para.Img = "/File/SYSMArticle/" + CurrentUser.UserName + "_" + imgName + ext;

                 try
                 {
                     if (string.IsNullOrWhiteSpace(oldPara.Img) == false)
                     {
                         string delFile = Server.MapPath("~") + oldPara.Img;
                         System.IO.File.Delete(delFile);
                     }
                 }
                 catch (Exception ex)
                 {
                     DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
                 }


                 try
                 {
                     if (!Directory.Exists(Server.MapPath("~/File/SYSMArticle/")))
                     {
                         Directory.CreateDirectory(Server.MapPath("~/File/SYSMArticle/"));
                     }
                     upfile.SaveAs(Server.MapPath(para.Img));
                 }
                 catch (Exception ex)
                 {
                     DAL.Log.Instance.Write(ex.ToString(), "SYSMArticle_ToSave_error");
                 }
             }
             else
             {
                 para.Img = oldPara.Img;
             }

             SYSMArticle oldArticle = SYSMArticle.GetEntityByID(para.ID);
             para.DatCreate = oldArticle.DatCreate;
             para.DatEdit = DateTime.Now;
             para.Tmp = "";
             int rtn = para.UpdateByID();
             if (rtn > 0)
             {
                 return Content("ok");
             }
             return Content("保存出错");
         }
        [B_MenuRightsTag("修改", "Index")]
         public ContentResult TempSave(int id, string tmp)
         {
             try
             {

                 SYSMArticle article = SYSMArticle.GetEntityByID(id);
                 if (article == null)
                 {
                     return Content("素材不存在");
                 }
                 int rtn = SYSMArticle.SaveTmpByID(id, tmp);
                 if (rtn > 0)
                 {
                     return Content("ok");

                 }
                 return Content("网络异常");
             }
             catch (Exception ex)
             {
                 Log.Instance.Write(ex.ToString(), "SYSMArticle_TempSave_error");
                 return Content("网络异常");
             }
         }
        [B_MenuRightsTag("修改", "Index")]
         public ContentResult GetTemp(int id)
         {
             SYSMArticle article = SYSMArticle.GetEntityByID(id);
             if (article != null)
             {
                 return Content(article.Tmp);
             }
             return Content("");
         }


         [B_MenuRightsTag("上传图片", "Index")]
         public ActionResult ImgsUp(int id)
         {
             try
             {
                 var file = Request.Files[0];
                 string path = Request.MapPath("~/");
                 string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                 if (!Directory.Exists(Server.MapPath("~/images/SYSMArticle/")))
                 {
                     Directory.CreateDirectory(Server.MapPath("~/images/SYSMArticle/"));
                 }
                 if (file.ContentLength == 0 || file == null)
                 {
                     return Content("上传的图片没有内容");
                 }
                 if (file.ContentLength > 5242880)
                 {
                     return Content("上传图片不能超过5MB！");
                 }
                 SYSMArticle article = SYSMArticle.GetEntityByID(id);

                 SYSMArticleImg imgs = new SYSMArticleImg();
                 imgs.ImgUrl = "/images/SYSMArticle/" + article.ID + "_article" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                 file.SaveAs(path + imgs.ImgUrl);
                 imgs.UserName = CurrentUser.UserName;
                 imgs.ArticleID = id;
                 imgs.Dat = DateTime.Now;
                 imgs.ImgUrl = WeConfig.b_domain + imgs.ImgUrl;
                 imgs.InsertAndReturnIdentity();
                 return Content(imgs.ImgUrl);
             }
             catch (Exception ex)
             {
                 Log.Instance.Write(ex.ToString(), "DetailImgsUp_error");
                 return Content("error|服务器繁忙");
             }

         }

         [B_MenuRightsTag("删除", "Index")]
         public ContentResult ToDel(int id)
         {
             if (SYSMArticle.DeleteByID(id) > 0)
             {
                 return Content("ok");
             }
             return Content("删除出错");
         }
         [B_MenuRightsTag("批量删除", "Index")]
         public ContentResult ToDels(int[] ids)
         {
             bool rtn = SYSMArticle.ToDels(ids);
             if (rtn)
             {
                 return Content("ok");
             }
             else
             {
                 return Content("操作失败,网络异常");
             }
         }

    }
}
