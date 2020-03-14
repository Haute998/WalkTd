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
    public class SYSOpenPicController : BaseController
    {
        //
        // GET: /SYSOpenPic/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        [B_MenuRightsTag("上传", "Index")]
        public ActionResult Process(string goodspk, string id, string name, string type, string lastModifiedDate, int size)
        {
            int goodsID = 0;
            int.TryParse(goodspk, out goodsID);
            string filePathName = string.Empty;
            string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload");
            if (Request.Files.Count == 0)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" });
            }
            try
            {
                    string path = Request.MapPath("~/");
                    if (!Directory.Exists(Server.MapPath("~/images/SYSOpenPic/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/SYSOpenPic/"));
                    }
                    var file = Request.Files[0];
                    string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                    string savename = "G_" + goodspk + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                    filePathName = "/images/SYSOpenPic/" + savename;//自行处理保存

                    //记录到数据库
                    SYSOpenPic article = new SYSOpenPic();
                    article.Dat = DateTime.Now;
                    article.ImgUrl = filePathName;
                    article.Title = goodsID.ToString();
                    article.ID = article.InsertAndReturnIdentity();


                    file.SaveAs(path + filePathName);
            }
            catch (Exception)
            {
                return Json(new { jsonrpc = 2.0, error = new { code = 103, message = "保存失败" }, id = "id" });
            }
            return Json(new
            {
                jsonrpc = "2.0",
                id = "id",
                filePath = filePathName//返回路径以备后用
            });
        }
        /// <summary>
        /// 海报删除
        /// </summary>
        /// <param name="imgpk"></param>
        /// <returns></returns>
        [B_MenuRightsTag("删除", "Index")]
        [HttpGet]
        public ContentResult imgsDel(string imgpk)
        {
            int imgID = 0;
            int.TryParse(imgpk, out imgID);
            string result = "";
            SYSOpenPic imgs = SYSOpenPic.GetEntityByID(imgID);
            if (SYSOpenPic.DeleteByID(imgID) > 0)
            {
                result = "OK";

                string delFile = Server.MapPath("~") + imgs.ImgUrl;
                System.IO.File.Delete(delFile);

            }
            else
            {
                result = "删除失败！";
            }
            return Content(result);
        }


        private string StrWhere(SYSOpenPicSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.Title))
            {
                where += string.Format(" and Title ='{0}'", condition.Title);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSOpenPicSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSOpenPic> page = new PageJsonModel<SYSOpenPic>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSOpenPic ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }



        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            SYSOpenPic oldimgs = SYSOpenPic.GetEntityByID(id);
            if (SYSOpenPic.DeleteByID(id) > 0)
            {
                try
                {
                    string delFile = Server.MapPath("~") + oldimgs.ImgUrl;
                    System.IO.File.Delete(delFile);
                }
                catch (Exception ex) { }
                return Content("ok");
            }
            return Content("删除出错");
        }
        [B_MenuRightsTag("批量删除", "Index")]
        public ContentResult ToDels(int[] ids)
        {
            bool rtn = SYSOpenPic.ToDels(ids, Server.MapPath("~"));
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
