using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class OpenSourceController : BaseController
    {
        //
        // GET: /OpenSource/

        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }

        /// <summary>
        /// 产品海报
        /// </summary>
        /// <returns></returns>
        public ActionResult openImg() 
        {
            ViewData["user"] = CurrentUser;
            return View();
        }

        public ActionResult LoadopenImg(SYSOpenPicSearch condition)
        {
            PageJsonModel<SYSOpenPic> page = new PageJsonModel<SYSOpenPic>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" SYSOpenPic ");
            page.strSelect = " * ";
            page.strWhere = " and Title<>'' ";


            if (condition.dayType == "today")
            {
                page.strWhere += string.Format(" and Dat>='{0}' ", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (condition.dayType == "history")
            {
                page.strWhere += string.Format(" and Dat<'{0}' ", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            }


            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 产品海报
        /// </summary>
        /// <returns></returns>
        public ActionResult openvideo()
        {
            ViewData["user"] = CurrentUser;
            return View();
        }

        public ActionResult Loadopenvideo(SYSOpenVideoSearch condition)
        {
            PageJsonModel<SYSOpenVideo> page = new PageJsonModel<SYSOpenVideo>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" SYSOpenVideo ");
            page.strSelect = " * ";
            page.strWhere = " and Title<>'' ";


            if (condition.dayType == "today")
            {
                page.strWhere += string.Format(" and Dat>='{0}' ", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            }
            if (condition.dayType == "history")
            {
                page.strWhere += string.Format(" and Dat<'{0}' ", DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            }


            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
