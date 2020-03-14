using AgentMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class myarticleController : ShardBaseController
    {
        //
        // GET: /myarticle/

        public ActionResult myart()
        {
            ViewData["user"] = CurrentUser;
            return View();
        }

        public ActionResult LoadArts(C_UserArticleSearch condition)
        {
            PageJsonModel<C_UserArticle> page = new PageJsonModel<C_UserArticle>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" C_UserArticle ");
            page.strSelect = " * ";
            page.strWhere = " ";


            page.strOrder = "DatCreate desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int id, string fromuser)
        {
            fromuser = (fromuser == null ? "" : fromuser);
            C_UserArticle article = C_UserArticle.GetEntityByID(id);
            if (article == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "文章不存在哦" });
            }
            ViewData["fromuser"] = fromuser;

            ViewData["user"] = CurrentUser;
            return View(article);
        }

    }
}
