using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class ProblemController : Controller
    {
        //
        // GET: /Problem/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logs(string phone)
        {
            ViewData["dh"]=phone;
            return View();
        }
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult Index3()
        {
            return View();
        }
        public ActionResult Logs1(string phone)
        {
            ViewData["dh"] = phone;
            return View();
        }
        public ActionResult toAdd1(C_UserAdvice para)
        {
            if (string.IsNullOrWhiteSpace(para.Contents))
            {
                return Content("Please select the fault problem");
            }

            if (string.IsNullOrWhiteSpace(para.Phone))
            {
                return Content("Please fill in the SN code or your phone number");
            }
            B_bd cnt = B_bd.login(para.Phone);
            if (cnt == null)
            {
                return Content("The phone number doesn't exist");
            }
            para.DatCreate = DateTime.Now;
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("Add an error");
        }
        public ActionResult toAdd(C_UserAdvice para)
        {
            if (string.IsNullOrWhiteSpace(para.Contents))
            {
                return Content("请选择故障问题");
            }

            if (string.IsNullOrWhiteSpace(para.Phone))
            {
                return Content("请填写SN码或者您的电话号码");
            }
           
            B_bd cnt = B_bd.login(para.Phone);
            if(cnt==null)
            {
                return Content("手机号不存在");
            }
            para.Name = para.Name;
            para.DatCreate = DateTime.Now;
            para.State = "退货";
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        public ActionResult toAdd2(C_UserAdvice para)
        {
            if (string.IsNullOrWhiteSpace(para.Contents))
            {
                return Content("请选择故障问题");
            }

            if (string.IsNullOrWhiteSpace(para.Phone))
            {
                return Content("请填写SN码或者您的电话号码");
            }
            B_bd cnt = B_bd.login(para.Phone);
            if (cnt == null)
            {
                return Content("手机号不存在");
            }
            para.Name = para.Name;
            para.DatCreate = DateTime.Now;
            para.State = "换货";
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        //public ActionResult cha(string SN)
        //{

        //    B_bd cnt = B_bd.cha(SN);
        //    if (cnt == null)
        //    {
        //        return Content("SN码不存在");
        //    }
        //     return Content("OK|"+cnt.phone);
            
        //}
        //产品维修列表
        public ActionResult xiuList(C_UserAdvice condition,string dh)
        {

            PageJsonModel<C_UserAdvice> page = new PageJsonModel<C_UserAdvice>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "C_UserAdvice";
            page.strSelect = "*";
            page.strWhere = "  and Phone=" + dh + "";
            page.strOrder = " ID desc";
            page.LoadListNoCnt();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
