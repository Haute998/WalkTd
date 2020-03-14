using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models.OrderModel;

namespace WeBusiness.Controllers
{
    public class RebateController : BaseController
    {
        //
        // GET: /Rebate/
        /// <summary>
        /// 未发放推荐返利
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 未发放推荐返利
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetNoRebatePage(RebateSearch condition)
        {
            condition.State = "未发放";
            string where = RebateSearch.StrWhere_tj(condition);
            PageJsonModel<C_UserRebate> page = GetPage(condition, where);
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 未发放推荐返利
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private PageJsonModel<C_UserRebate> GetPage(RebateSearch condition, string where)
        {
            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@"( select  C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat) 年,
                                month(DatCreat) 月,
                                sum(C_UserRebate.Money) 返利合计,C_UserRebate.[State]
                                from C_UserRebate left join C_User on C_UserRebate.R_People=C_User.UserName
                                where C_UserRebate.Cat='推荐返利' and C_UserRebate.Issuer='总部' {0}
                                group by C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat),
                                month(DatCreat),C_UserRebate.[State] ) as Show", where);
            page.strSelect = " * ";
            page.strWhere = "";
            page.strOrder = "年,月 desc";
            return page;

        }
        /// <summary>
        /// 未发放推荐返利导出
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel_NoRebate_tj(RebateSearch condition)
        {
            condition.State = "未发放";
            string where = RebateSearch.StrWhere_tj(condition);
            PageJsonModel<C_UserRebate> page = GetPage(condition, where);

            string sql = @" select convert(varchar(20),年)+'年'+convert(varchar(20),月)+'月',Name,Phone,返利合计,[State] from " + page.strForm + " where 1=1 " + page.strWhere;

            DataTable dt = ExportWay.ExcelDataTable(sql);
            string[] list = { "月份", "返利人", "返利人手机号", "返利金额", "状态" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "未发放推荐返利" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        /// <summary>
        /// 已发放推荐返利
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult RebateIndex()
        {
            return View();
        }
        /// <summary>
        /// 已发放推荐返利
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "RebateIndex")]
        public ActionResult GetYesRebatePage(RebateSearch condition)
        {
            condition.State = "已发放";
            string where = RebateSearch.StrWhere_tj(condition);
            PageJsonModel<C_UserRebate> page = GetPage(condition, where);
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 已发放推荐返利导出  【待续】
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "RebateIndex")]
        public ActionResult ExportExcel_YesRebate_tj(RebateSearch condition)
        {
            condition.State = "已发放";
            string where = RebateSearch.StrWhere_tj(condition);
            PageJsonModel<C_UserRebate> page = GetPage(condition, where);

            string sql = @" select convert(varchar(20),年)+'年'+convert(varchar(20),月)+'月',Name,Phone,返利合计,[State] from " + page.strForm + " where 1=1 " + page.strWhere;

            DataTable dt = ExportWay.ExcelDataTable(sql);
            string[] list = { "月份", "返利人", "返利人手机号", "返利金额", "状态" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "已发放推荐返利" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        /// <summary>
        /// 已发放拿货返利
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult RebateIndex_nh()
        {
            return View();
        }
        /// <summary>
        /// 已发放拿货返利
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "RebateIndex_nh")]
        public ActionResult GetYesRebatePage_nh(RebateSearch condition)
        {
            condition.State = "已发放";
            string where = RebateSearch.StrWhere_nh(condition);
            PageJsonModel<C_UserRebate> page = Rebate_nh_GetPage(condition, where);
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 已发放拿货返利导出 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "RebateIndex_nh")]
        public ActionResult ExportExcel_YesRebate_nh(RebateSearch condition)
        {
            condition.State = "已发放";
            string where = RebateSearch.StrWhere_nh(condition);
            PageJsonModel<C_UserRebate> page = Rebate_nh_GetPage(condition, where);

            string sql = @" select convert(varchar(20),年)+'年'+convert(varchar(20),月)+'月',Name,Phone,订单总额,返利合计,[State] from " + page.strForm + " where 1=1 " + page.strWhere;

            DataTable dt = ExportWay.ExcelDataTable(sql);
            string[] list = { "月份", "返利人", "返利人手机号", "订单总额", "返利金额", "状态" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "已发放拿货返利" + DateTime.Now.ToShortTimeString() + ".xls");
        }



        /// <summary>
        /// 确定发放
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mon"></param>
        /// <returns></returns>
        [B_MenuRightsTag("发放", "Index")]
        public ActionResult GetUpdateNoRebate(string ID, string mon, string cat)
        {
            if (cat == "nh")
            {
                cat = "拿货返利";
            }
            else if (cat == "tj")
            {
                cat = "推荐返利";
            }
            else
            {
                return Content("该返利类型不存在");
            }
            DateTime DatMon = new DateTime();
            DateTime.TryParse(mon, out DatMon);
            string rtn = C_UserRebate.UpdateRebateState(ID, DatMon, cat, "总部");
            return Content(rtn);
        }
        /// <summary>
        /// 取消发放
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("取消发放", "Index")]
        public ActionResult GetUpdateCancelRebate(string ID, string mon, string cat)
        {
            if (cat == "nh")
            {
                cat = "拿货返利";
            }
            else if (cat == "tj")
            {
                cat = "推荐返利";
            }
            else
            {
                return Content("该返利类型不存在");
            }
            DateTime DatMon = new DateTime();
            DateTime.TryParse(mon, out DatMon);
            string rtn = C_UserRebate.UpdateRebateStateCancel(ID, DatMon, cat, "总部");
            return Content(rtn);
        }
        /// <summary>
        /// 未发放推荐返利详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult NoRebateDetailIndex(string ID, string mon)
        {
            ViewData["UserName"] = ID;
            ViewData["mon"] = mon;
            return View();
        }
        [B_MenuRightsTag("订单详情", "Index")]
        //public ActionResult OrderDetail(string orderNo)
        //{
        //    if (string.IsNullOrWhiteSpace(orderNo))
        //    {
        //        return View(ErrorPage.ViewName, new ErrorPage { Message = "订单编号有误" });
        //    }
        //    OrderVM order = new OrderVM();
        //    order.LoadOrder(orderNo);
        //    return View(order);
        //}
        /// <summary>
        /// 未发放推荐返利详情
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        //[B_MenuRightsTag("查看", "Index")]
        public ActionResult GetNoRebateDetailPage(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Show.Name like '%" + condition.keyword + "%'";
            }
            condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

            DateTime DatMon = new DateTime();
            DateTime.TryParse(condition.DatCreateMon, out DatMon);
            where += string.Format(" and Show.State='未发放' and Show.Issuer='总部' and Show.Cat='推荐返利'  and  Show.R_People='" + condition.UserName + "' and datepart(mm,Show.DatCreat)=datepart(mm,'{0}')  ", DatMon.ToString("yyyy/MM/dd"));
            return GetDetailPage(condition, where);

        }
        /// <summary>
        /// 已发放推荐返利详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult YesRebateDetailIndex(string ID, string mon)
        {
            ViewData["UserName"] = ID;
            ViewData["mon"] = mon;
            return View();
        }
        /// <summary>
        /// 已发放推荐返利详情
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "YesRebateDetailIndex")]
        public ActionResult GetYesRebateDetailPage(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and c.Name like '%" + condition.keyword + "%'";
            }
            condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

            DateTime DatMon = new DateTime();
            DateTime.TryParse(condition.DatCreateMon, out DatMon);
            where += string.Format(" and Show.State='已发放' and Show.Cat='推荐返利'  and  Show.R_People='" + condition.UserName + "' and datepart(mm,Show.DatCreat)=datepart(mm,'{0}')  ", DatMon.ToString("yyyy/MM/dd"));
            return GetDetailPage(condition, where);

        }
        private ActionResult GetDetailPage(RebateSearch condition, string where)
        {
            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"( Select t.Name GLever, cn.Name GName, o.SumPrice SumPrice,r.*,c.Name Name,c.Phone Phone from C_UserRebate r left join C_User c on r.R_People=c.UserName  left join [Order] o on r.OrderNo=o.OrderNo  left join  C_User cn on cn.UserName=o.UserName left join C_UserType t on cn.C_UserTypeID=t.Lever  where r.Issuer='总部' ) as Show";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "Money desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取直属代理
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult GetIntroceIndex()
        {
            return View();
        }
        public ActionResult GetInstroceDetail(C_UserSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name like '%" + condition.keyword + "%'";
            }
            PageJsonModel<C_UserInduce> page = NewMethod(condition, where);
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }

        private static PageJsonModel<C_UserInduce> NewMethod(C_UserSearch condition, string where)
        {
            PageJsonModel<C_UserInduce> page = new PageJsonModel<C_UserInduce>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"( select c.*,t.Name ILeverName from  (select c.*,t.Name LeverName from (SELECT c.*,b.C_UserTypeID IC_UserTypeID ,b.Name IName,b.Phone IPhone FROM C_User c left join C_User b on c.Introducer=b.UserName where c.Introducer!='' and c.Chief=0) c left join C_UserType t on  c.C_UserTypeID=t.Lever) c left join C_UserType t on c.IC_UserTypeID=t.Lever ) as Show";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return page;
        }
        [B_MenuRightsTag("查看")]
        public ActionResult ZtreeIndex()
        {
            return View();
        }
        public ActionResult GetOptionTree(string ID)
        {
            string SelVal = C_User.GetOptionDisTreeMenu(ID);
            if (!string.IsNullOrWhiteSpace(SelVal))
            {
                return Json(SelVal, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserMsg(string ID)
        {
            SearchZtree data = C_User.GetOptionDisTreeUserMsg(ID);
            if (data != null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// 未发放拿货返利
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult Rebate_nh()
        {
            return View();
        }
        [B_MenuRightsTag("查看", "Rebate_nh")]
        public ActionResult Rebate_nh_GetNoRebatePage(RebateSearch condition)
        {
            condition.State = "未发放";
            string where = RebateSearch.StrWhere_nh(condition);
            PageJsonModel<C_UserRebate> page = Rebate_nh_GetPage(condition, where);
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 未发放拿货返利导出 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Rebate_nh")]
        public ActionResult ExportExcel_NoRebate_nh(RebateSearch condition)
        {
            condition.State = "未发放";
            string where = RebateSearch.StrWhere_nh(condition);
            PageJsonModel<C_UserRebate> page = Rebate_nh_GetPage(condition, where);

            string sql = @" select convert(varchar(20),年)+'年'+convert(varchar(20),月)+'月',Name,Phone,订单总额,返利合计,[State] from " + page.strForm + " where 1=1 " + page.strWhere;

            DataTable dt = ExportWay.ExcelDataTable(sql);
            string[] list = { "月份", "返利人", "返利人手机号", "订单总额", "返利金额", "状态" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "应付拿货返利" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        private PageJsonModel<C_UserRebate> Rebate_nh_GetPage(RebateSearch condition, string where)
        {
            decimal GetOrderRateMaxPrice = BaseParameters.getPrice("GetOrderRateMaxPrice");//拿货返利门槛金额
            decimal GetOrderRate = BaseParameters.getRate("GetOrderRate");//拿货返利

            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@"( select C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat) 年,
                                                month(DatCreat) 月,
                                                case when sum(C_UserRebate.[OrderMoney])>={0} 
                                                then sum(C_UserRebate.[OrderMoney])*{1}
                                                else 0 end
                                                返利合计,sum(C_UserRebate.[OrderMoney]) 订单总额,C_UserRebate.[State]
                                                from C_UserRebate left join C_User on C_UserRebate.R_People=C_User.UserName
                                                where C_UserRebate.Cat='拿货返利' and C_UserRebate.Issuer='总部' {2}
                                                group by C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat),
                                                month(DatCreat),C_UserRebate.[State] ) as Show", GetOrderRateMaxPrice, GetOrderRate, where);
            page.strSelect = " * ";
            page.strWhere = "";
            page.strOrder = "年,月 desc";
            return page;
        }

        /// <summary>
        /// 未发放拿货返利详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "Rebate_nh")]
        public ActionResult NoRebate_nh_DetailIndex(string ID, string mon)
        {
            ViewData["UserName"] = ID;
            ViewData["mon"] = mon;
            return View();
        }
        [B_MenuRightsTag("查看", "Rebate_nh")]
        public ActionResult GetNoRebate_nh_DetailPage(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and c.Name like '%" + condition.keyword + "%'";
            }
            condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

            DateTime DatMon = new DateTime();
            DateTime.TryParse(condition.DatCreateMon, out DatMon);
            where += string.Format(" and r.State='未发放' and r.Cat='拿货返利'  and  r.R_People='" + condition.UserName + "' and datepart(mm,r.DatCreat)=datepart(mm,'{0}')  ", DatMon.ToString("yyyy/MM/dd"));


            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"  C_UserRebate r left join C_User c on r.R_People=c.UserName
                               left join C_UserType on C_UserType.Lever=c.C_UserTypeID left join [Order] on r.OrderNo=[Order].OrderNo";
            page.strSelect = " r.*,c.Name Name,c.Phone Phone,C_UserType.Name userTypeName,[Order].ProductCnt ";
            page.strWhere = " and Issuer='总部' " + where;
            page.strOrder = "r.DatCreat desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// 已发放拿货返利详情
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "Rebate_nh")]
        public ActionResult Rebate_nh_DetailIndex(string ID, string mon)
        {
            ViewData["UserName"] = ID;
            ViewData["mon"] = mon;
            return View();
        }
        [B_MenuRightsTag("查看", "Rebate_nh")]
        public ActionResult GetRebate_nh_DetailPage(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and c.Name like '%" + condition.keyword + "%'";
            }
            condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

            DateTime DatMon = new DateTime();
            DateTime.TryParse(condition.DatCreateMon, out DatMon);
            where += string.Format(" and r.State='已发放' and r.Cat='拿货返利'  and  r.R_People='" + condition.UserName + "' and datepart(mm,r.DatCreat)=datepart(mm,'{0}')  ", DatMon.ToString("yyyy/MM/dd"));


            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"  C_UserRebate r left join C_User c on r.R_People=c.UserName
                               left join C_UserType on C_UserType.Lever=c.C_UserTypeID left join [Order] on r.OrderNo=[Order].OrderNo ";
            page.strSelect = " r.*,c.Name Name,c.Phone Phone,C_UserType.Name userTypeName,[Order].ProductCnt ";
            page.strWhere = " and Issuer='总部' " + where;
            page.strOrder = "r.DatCreat desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);

        }

    }
}
