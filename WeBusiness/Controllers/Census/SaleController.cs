using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SaleController : BaseController
    {
        //
        // GET: /Sale/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetSaleAgent(SaleSearch condition)
        {
            string where = string.Empty;
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and Name like '%{0}%'  ", condition.keyword);
            }
            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and DatAudit >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and DatAudit <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }


            if (condition.C_UserTypeID != 0)
            {
                where += string.Format(" and C_UserTypeID ='{0}' ", condition.C_UserTypeID);
            }

            PageJsonModel<Sale> page = new PageJsonModel<Sale>();
            page.pageIndex = condition.pageIndex;
            page.strForm = @" (select c.Name Name ,SUM(o.SumPrice) SumPrice,c.Identifier Identifier from [C_User] c left join [Order] o on c.UserName=o.UserName   where OrderState='已发货'  " + where + " group by Name,Identifier) as show";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = "";
            page.strOrder = " SumPrice Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }



        public ActionResult InCreaseIndex()
        {
            return View();
        }
        /// <summary>
        /// 会员分析
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetC_ConsumercxyYearCensus(int Year)
        {
            string SelVal = SaleCensus.GetCountC_Consumer(Year,"促销员");
            if (string.IsNullOrWhiteSpace(SelVal))
            {
                return Content("");
            }
            return Content(SelVal);
        }
















        //[B_MenuRightsTag("查看")]
        //public ActionResult InCreaseIndex()
        //{
        //    return View();
        //}

        public ActionResult GetAgentIncrease(SaleSearch condition)
        {
            string where = string.Empty;
            string wheres = string.Empty;
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                wheres += string.Format(@" and Name like '%{0}%'  ", condition.keyword);
            }
            //订单创建时间
            where += string.Format(" and DatAudit >='{0}/01 00:00:00' and DatAudit<'{1}/01 00:00:00'", Common.Filter(condition.DatCreateB), DateTime.Parse(Common.Filter(condition.DatCreateB) + "/01").AddMonths(1).ToString("yyyy-MM"));

            if (condition.C_UserTypeID != 0)
            {
                wheres += string.Format(" and C_UserTypeID ='{0}' ", condition.C_UserTypeID);
            }
            string OldSumPrice = "(select Sum(o.SumPrice)  from [C_User] c left join [Order] o on c.UserName=o.UserName  where c.UserName=oc.UserName and OrderState='已发货' and DatAudit<'" + Common.Filter(condition.DatCreateB) + "/01 00:00:00')";
            string TheMonthSumPrice = "(select Sum(o.SumPrice)  from [C_User] c left join [Order] o on c.UserName=o.UserName  where c.UserName=oc.UserName and OrderState='已发货'  " + where + ")";
            PageJsonModel<SaleInCrease> page = new PageJsonModel<SaleInCrease>();
            page.pageIndex = condition.pageIndex;
            page.strForm = @"(select " + OldSumPrice + " OldSumPrice," + TheMonthSumPrice + " TheMonthSumPrice ," + TheMonthSumPrice + "/" + OldSumPrice + " Increase,  SUM(od.SumPrice) SumPrice,oc.Name Name,oc.Identifier Identifier,oc.C_UserTypeID C_UserTypeID from [C_User] oc left join [Order] od on od.UserName=oc.UserName   where od.OrderState='已发货'  group by oc.UserName, Name,Identifier,oc.C_UserTypeID  ) as show";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = wheres;
            page.strOrder = " Increase,TheMonthSumPrice desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
