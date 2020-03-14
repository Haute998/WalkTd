using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class articlegetController : BaseController
    {
        //
        // GET: /articleget/

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult getarticle(string articleurl) 
        {
            articleurl = HttpUtility.UrlDecode(articleurl);
            if (articleurl == null || articleurl.Trim() == "")
            {
                return Content("fail|请输入文章链接");
            }
            string rtn = C_UserArticle.getwxarticleByUrl(articleurl,CurrentUser.UserName, Server.MapPath("~"));

            return Content(rtn);
        }
        /// <summary>
        /// 编辑文章新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult articleEdit2(int id)
        {
            C_UserArticle article = C_UserArticle.GetEntityByID(id);
            if (article == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "文章找不到了哦" });
            }
            ViewData["article"] = article;
            return View();
        }

        /// <summary>
        /// 编辑文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult articleEdit(int id) 
        {
            C_UserArticle article = C_UserArticle.GetEntityByID(id);
            if (article == null) 
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "文章找不到了哦" });
            }
            ViewData["article"] = article;
            return View();
        }

        public JsonResult save(int id,string title,string content) 
        {
            C_UserArticle art = C_UserArticle.GetEntityByID(id);
            art.title = title;
            art.contents = content;
            art.UpdateByID();
            return Json(new { status = 1, message="保存成功" }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult upimg(int id) 
        {
            C_UserArticle art = C_UserArticle.GetEntityByID(id);
            //根据前台html的name获取文件  

            string upfile = Request.Params["imgbase"];
            if (string.IsNullOrWhiteSpace(upfile))
            {
                return Json(new { code = "fail", msg = "您没有选择图片" }, JsonRequestBehavior.AllowGet);

            }


            string imgName = DateTime.Now.ToString("yyyyMMddHHss") + DateTime.Now.Ticks;
            string ext = ".jpg";//获得文件扩展名

            string pfile = string.Format("/File/mater/{0}/{1}/", CurrentUser.UserName, DateTime.Now.ToString("yyyyMMdd"));
            string dbUrl = string.Format("/{0}/{1}", pfile, (DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString() + ext));

            string savename = Server.MapPath("~") +dbUrl;


            if (!Directory.Exists(Server.MapPath("~" + pfile)))
            {
                Directory.CreateDirectory(Server.MapPath("~" + pfile));
            }
            imghelper.SaveBase64Image(upfile, dbUrl, savename);

            C_UserMater mater = new C_UserMater();
            mater.C_UserName = CurrentUser.UserName;
            mater.Dat = DateTime.Now;
            mater.MaterType = "img";
            mater.MaterUrl = dbUrl;
            mater.ID = mater.InsertAndReturnIdentity();

            C_UserArticleMater artMater = new C_UserArticleMater();
            artMater.ArticleID = art.ID;
            artMater.Dat = DateTime.Now;
            artMater.MaterID = mater.ID;
            artMater.ID = artMater.InsertAndReturnIdentity();
            return Json(new { code = "ok", msg = WeConfig.wx_domain + dbUrl }, JsonRequestBehavior.AllowGet);
        }

     

    }
}
