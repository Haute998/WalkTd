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
    public class C_UserNoDirectlyController : BaseController
    {
        //
        // GET: /C_UserNoDirectly/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "DirectlyIndex")]
        public ActionResult ExportExcel(C_UserSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select UserName,Name,wxNo,Phone,Province,City,Area,WxQRCode from [C_User] where 1=1 ");
            where.Append("and state='已审核'  and Chief=0  ");
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where.Append(string.Format(" and (Name like '%{0}%' or Phone like '%{0}%' or Identifier like '%{0}%' or LevelName like '%{0}%')", condition.keyword));
            }
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "经销商编号", "经销商名称", "联系人", "联系方式", "省份", "市区", "城镇", "详细地址" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "非直属客户信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        private string StrWhere(C_UserSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Name like '%{0}%' or Phone like '%{0}%' or Identifier like '%{0}%' or LevelName like '%{0}%' or (Province+City+Area) like '%{0}%' )", condition.keyword);
            }
            return where;
        }
        public ActionResult GetNoVerityPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='已审核'  and Chief!=0 ";
            return GetPages(condition, where);
        }

        public ActionResult GetVerityPage(C_UserSearch condition)
        {
            string where = StrWhere(condition);
            where += " and state='未审核'  and Chief!=0 ";
            return GetPages(condition, where);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult VerifyIndex()
        {
            return View();
        }
        private ActionResult GetPages(C_UserSearch condition, string where)
        {
            PageJsonModel<C_UserShow> page = new PageJsonModel<C_UserShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"(select c.*,o.Name ChiefName,o.Phone ChiefPhone ,t.Name LevelName,I.Name IName  from  C_User as c left join C_User as o on c.Chief=o.ID  left join C_UserType t on c.C_UserTypeID=t.Lever left join C_User I on c.Introducer=I.UserName ) as C_UserShow";
            page.strSelect = " * ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ID desc";
            }
            
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


    }
}
