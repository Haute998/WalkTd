using AgentMobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class SourceShowController : ShardBaseController
    {
        //
        // GET: /SourceShow/

        public ActionResult SourceLst()
        {

            ViewData["user"] = CurrentUser;
            return View();
        }

        public ActionResult LoadSources(SYSMArticleSearch condition)
        {
            PageJsonModel<SYSMArticle> page = new PageJsonModel<SYSMArticle>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" SYSMArticle ");
            page.strSelect = " * ";
            page.strWhere = " and Title<>'' ";


            if (condition.dayType == "today") 
            {
                page.strWhere += string.Format(" and DatEdit>='{0}' ",DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (condition.dayType == "history") 
            {
                page.strWhere += string.Format(" and DatEdit<'{0}' ", DateTime.Now.ToString("yyyy-MM-dd 00:00:00")); 
            }


            page.strOrder = "DatEdit desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Detail(int id, string fromuser)
        {
            fromuser=(fromuser == null ? "" : fromuser);
            SYSMArticle article = SYSMArticle.GetEntityByID(id);
            if (article == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "素材不存在哦" });
            }
            ViewData["fromuser"] = fromuser;
            return View(article);
        }

        /// <summary>
        /// 个人文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult article(int id)
        {
            C_UserArticle article = C_UserArticle.GetEntityByID(id);
            if (article == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "文章找不到了哦" });
            }
            ViewData["article"] = article;
            return View();
        }

        public ActionResult articlenew(int id)
        {
            C_UserArticle article = C_UserArticle.GetEntityByID(id);
            if (article == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "文章找不到了哦" });
            }
            ViewData["article"] = article;
            return View();
        }


    }
}
