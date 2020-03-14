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
    public class SYSOpenVideoController : BaseController
    {
        //
        // GET: /SYSOpenVideo/


        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        [B_MenuRightsTag("上传", "Index")]
        public ActionResult Process(string goodspk, string id, string name, string type, string lastModifiedDate, string size)
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
                if (!Directory.Exists(Server.MapPath("~/Video/SYSOpenVideo/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Video/SYSOpenVideo/"));
                }
                var file = Request.Files[0];
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                string savename = "G_" + goodspk + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                filePathName = "/Video/SYSOpenVideo/" + savename;//自行处理保存

                //记录到数据库
                SYSOpenVideo article = new SYSOpenVideo();
                article.Dat = DateTime.Now;
                article.VideoUrl = filePathName;
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
        /// 删除
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
            SYSOpenVideo imgs = SYSOpenVideo.GetEntityByID(imgID);
            if (SYSOpenVideo.DeleteByID(imgID) > 0)
            {
                result = "OK";

                string delFile = Server.MapPath("~") + imgs.VideoUrl;
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
            PageJsonModel<SYSOpenVideo> page = new PageJsonModel<SYSOpenVideo>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSOpenVideo ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }



        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            SYSOpenVideo oldimgs = SYSOpenVideo.GetEntityByID(id);
            if (SYSOpenVideo.DeleteByID(id) > 0)
            {
                try
                {
                    string delFile = Server.MapPath("~") + oldimgs.VideoUrl;
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
            bool rtn = SYSOpenVideo.ToDels(ids, Server.MapPath("~"));
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
