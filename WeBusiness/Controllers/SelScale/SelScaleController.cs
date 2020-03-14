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
    public class SelScaleController : BaseController
    {
        //
        // GET: /SelScale/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult ExportExcel(SelScale condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select SelScale.*,Scale.BigCode,Scale.SmallCode,Scale.SelectDate from SelScale left join Scale on SelScale.AntiCode=Scale.AntiCode where 1=1");
            //where.Append("and state='已审核'  and Chief=0  ");
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where.Append(string.Format(" and (BigCode like '%{0}%' or SmallCode like '%{0}%' or SelScale.AntiCode like '%{0}%' or Address like '%{0}%')", condition.keyword));
            }
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "ID", "防伪码", "IP", "消费者查询省份", "消费者查询城市", "查询状态", "销售地区", "经销商编号", "外箱条码", "产品条码", "查询时间" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "防伪查询明细信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        public ActionResult GetCodePage2(SelScale condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.province))
            {
                where += string.Format(" and province+city like '%{0}%'", condition.province);
            }
            if (!string.IsNullOrWhiteSpace(condition.Address))
            {
                where += string.Format(" and Address like '%{0}%'", condition.Address);
            }

            return GetPages2(condition, where);
        }
        private ActionResult GetPages2(SelScale condition, string where)
        {
            PageJsonModel<SelScale> page = new PageJsonModel<SelScale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SelScale";
            page.strSelect = " province,city,Address,warning,count(ID) cnt";
            page.strWhere = where + " and warning='窜货' group by province,city,Address,warning";
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "count(ID) desc";
            }
           
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCodePage(SelScale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and (isnull(BigCode,'')+isnull(MiddleCode,'')+isnull(SmallCode,'')+AntiCode like '%" + Common.FilteSQLStr(condition.keyword) + "%' )";
            }
            if (!string.IsNullOrWhiteSpace(condition.province))
            {
                where += " and (province+citye like '%" + Common.FilteSQLStr(condition.keyword) + "%' )";
            }
            if (!string.IsNullOrWhiteSpace(condition.Address))
            {
                where += " and (Address like '%" + Common.FilteSQLStr(condition.Address) + "%' )";
            }

            return GetPages(condition, where);
        }

        private ActionResult GetPages(SelScale condition, string where)
        {
            PageJsonModel<View_AntiQuery> page = new PageJsonModel<View_AntiQuery>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "View_AntiQuery";
            page.strSelect = " ID, AntiCode, IP, province, city, warning, Address, UserName, CreateTime, BigCode, MiddleCode, SmallCode,SalesAddress ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "CreateTime desc";
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
