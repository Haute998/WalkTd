using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class StockController : BaseController
    {
          /// <summary>
        /// 我的库存
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }
        /// <summary>
        /// 我的库存
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult MyStockPage(StockSearch condition)
        {
            string where = string.Empty;
            PageJsonModel<Stock> page = new PageJsonModel<Stock>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select p.ProductName,p.ProductImg as ProductImgUrl,s.Consignee,p.ProductNumber,Count(SmallCode) count " +
                           " from Product as p left join ScaleOutStoke as s on s.ProductNo=p.ProductNumber and s.State<>'禁用' and s.Consignee='" + CurrentUser.UserName + "' " +
                           " group by s.Consignee,p.ProductNumber,p.ProductName,p.ProductImg)def";
            page.strSelect = "* ";
            page.strOrder = "count desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AgentStockIndex()
        {
            return View();
        }
        public ActionResult GetAgentStockPage(StockSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Consignee+Name+ProductNumber+ProductName like '%" + Common.Filter(condition.keyword) + "%'";
            }
            return NewMethod(condition, where);
        }
        public ActionResult AgentStockByUserNameIndex(string ID)
        {
            ViewData["user"] = C_UserVM.GetVMByID(Convert.ToInt32(ID));
            return View(CurrentUser);
        }
        public ActionResult GetAgentStockByUserNamePage(StockSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Consignee='" + condition.keyword + "'";
            }
            return NewMethod(condition, where);
        }

        private ActionResult NewMethod(StockSearch condition, string where)
        {
            PageJsonModel<Stock> page = new PageJsonModel<Stock>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " (select COUNT(*) count,s.Consignee Consignee, p.ProductName ProductName,p.ProductImg ProductImgUrl,c.Name Name,ProductNumber " +
                           " from ScaleOutStoke  s left join Product p on s.ProductNo=p.ProductNumber left join C_User c on s.Consignee=c.UserName "+
                           " where s.State='启用' and s.Shipper='" + CurrentUser.UserName + "' group by s.Consignee,s.ProductNo,p.ProductName,p.ProductImg,c.Name,ProductNumber) as Detail";
            page.strSelect = "* ";
            page.strWhere = where;
            page.strOrder = "Consignee asc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 库存条码明细
        /// </summary>
        /// <returns></returns>
        public ActionResult MyScaleDetail(string ID)
        {
            ViewData["P_ID"] = ID;
            return View();
        }

        public ActionResult GetMyScalePage(BaseInSearch condition)
        {
            PageJsonModel<ScaleOutStoke> page = new PageJsonModel<ScaleOutStoke>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " ScaleOutStoke ";
            page.strSelect = "* ";
            page.strWhere = string.Format(" and  Consignee='{0}' and State='启用' and  P_ID='{1}'", CurrentUser.UserName, condition.keyword);
            page.strOrder = "DatCreate desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

    }
}
