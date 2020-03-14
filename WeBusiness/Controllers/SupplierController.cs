using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SupplierController : BaseController
    {
        // GET: /Supplier/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPage(SupplierSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Name like '%{0}%' or Address like '%{0}%' or Phone like '%{0}%' or Card like '%{0}%' )", condition.keyword);
            }
            return GetPages(condition, where);
        }

        private ActionResult GetPages(SupplierSearch condition, string where)
        {
            PageJsonModel<Supplier> page = new PageJsonModel<Supplier>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "Supplier";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }

        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {

            return View();
        }

        public ActionResult ToAdd(Supplier sup)
        {

            if (string.IsNullOrWhiteSpace(sup.Name))
            {
                return Content("供应商名称不能为空！！！");
            }
            if (string.IsNullOrWhiteSpace(sup.Address))
            {
                return Content("地区名称不能为空！！！");
            }
            if (string.IsNullOrWhiteSpace(sup.Phone))
            {
                return Content("联系方式不能为空！！！");
            }
           
            sup.B_Name = CurrentUser.Name;
            sup.AddTime = DateTime.Now;
            int rtn = sup.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("网络错误！！！");
        }


        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int ID)
        {
            Supplier sup = Supplier.GetEntityByID(ID);

            return View(sup);
        }

         public ActionResult ToEdit(Supplier sup)
        {
            if (string.IsNullOrWhiteSpace(sup.Name))
            {
                return Content("供应商名称不能为空！！！");
            }
            if (string.IsNullOrWhiteSpace(sup.Address))
            {
                return Content("地区名称不能为空！！！");
            }
            if (string.IsNullOrWhiteSpace(sup.Phone))
            {
                return Content("联系方式不能为空！！！");
            }
            Supplier NewSup = Supplier.GetEntityByID(sup.ID);
            NewSup.Name = sup.Name;
            NewSup.Address = sup.Address;
            NewSup.B_Name = CurrentUser.Name;
            NewSup.AddTime = DateTime.Now;
            NewSup.Phone = sup.Phone; ;
            NewSup.Card = sup.Card;
            NewSup.Type = sup.Type;
            int rtn = NewSup.UpdateByID();
            if (rtn>0)
            {
                return Content("ok");
            }
            return Content("修改失败！！！");
         }

         [B_MenuRightsTag("删除", "Index")]
         public ActionResult ToDel(int ID)
         {
             if (Supplier.DeleteByID(ID) > 0)
             {
                 return Content("ok");
             
             }
             return Content("删除失败！！！");
         }

         [B_MenuRightsTag("导出", "Index")]
         public ActionResult ExportExcel(SupplierSearch condition)
         {
             StringBuilder where = new StringBuilder();
             where.Append("select Name,Address,Phone,Card,B_Name,AddTime from [Supplier] where 1=1 ");
             if (!string.IsNullOrWhiteSpace(condition.keyword))
             {
                 where.Append(string.Format(" and (Name like '%{0}%' or Address like '%{0}%' or Phone like '%{0}%' or Card like '%{0}%')", condition.keyword));
             }
             DataTable dt = ExportWay.ExcelDataTable(where.ToString());
             string[] list = { "供应商", " 所属地区","手机号","身份证号", "添加人", "添加时间" };
             return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "供应商信息" + DateTime.Now.ToShortTimeString() + ".xls");
         }


    }
}
