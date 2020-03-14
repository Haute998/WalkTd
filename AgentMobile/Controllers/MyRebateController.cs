using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels.Models.C_UserModel;
using WeModels;

namespace AgentMobile.Controllers
{
    public class MyRebateController : BaseController
    {
        //
        // GET: /MyRebate/
        /// <summary>
        /// 我的返利
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);

            RebateStatistics reba = new RebateStatistics();

            reba.thisMonMoneyTJ = C_UserRebate.GetRebateMoney_tj(CurrentUser.UserName, DateTime.Now, "未发放","");//本月推荐返利
            reba.thisMonMoneyNH = C_UserRebate.GetRebateMoney_nh(CurrentUser.UserName, DateTime.Now, "未发放","");//本月拿货返利
            reba.thisMonMoney = reba.thisMonMoneyTJ + reba.thisMonMoneyNH;

            reba.HistoryMoneyTJ = C_UserRebate.GetRebateMoneyHistory_tj(CurrentUser.UserName, "未发放");//历史推荐返利
            reba.HistoryMoneyNH = C_UserRebate.GetRebateMoneyHistory_nh(CurrentUser.UserName, "未发放");//历史拿货返利
            reba.HistoryMoney = reba.HistoryMoneyTJ + reba.HistoryMoneyNH;//历史返利
            reba.My_tjs_cnt = C_UserRebate.getMy_tjs_cnt(CurrentUser.UserName);

            return View(reba);
        }

        /// <summary>
        /// 返利明细
        /// </summary>
        /// <param name="dattype">本月：thismon/历史：history</param>
        /// <returns></returns>
        public ActionResult RebateDetail_tj(string dattype) 
        {
            ViewData["dattype"] = dattype;
            return View();
        }
        public ActionResult RebateDetail_nh(string dattype)
        {
            ViewData["dattype"] = dattype;

            ViewBag.GetOrderRateMaxPrice = BaseParameters.getPrice("GetOrderRateMaxPrice");//拿货返利门槛金额
            return View();
        }

        public ActionResult LoadRebateDetail(RebateSearch condition)
        {
            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" C_UserRebate left join [Order] left join C_User on [Order].UserName=C_User.UserName on C_UserRebate.OrderNo=[Order].OrderNo ");
            page.strSelect = " C_UserRebate.*,C_User.Name order_CName, [Order].SumPrice ";
            page.strWhere =string.Format(" and R_People='{0}' ",CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.keyword)) 
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and C_UserRebate.OrderNo='{0}' ", condition.keyword);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateMon)) 
            {
                condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

                DateTime DatMon = new DateTime();
                DateTime.TryParse(condition.DatCreateMon, out DatMon);

                page.strWhere += string.Format("  and datepart(mm,C_UserRebate.DatCreat)=datepart(mm,'{0}') ", DatMon.ToString("yyyy/MM/dd"));
            }
            if (!string.IsNullOrWhiteSpace(condition.Cat)) 
            {
                page.strWhere += string.Format("  and C_UserRebate.Cat='{0}' ", condition.Cat);
            }

            page.strOrder = "C_UserRebate.DatCreat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下级的返利 应付返利
        /// </summary>
        /// <returns></returns>
        public ActionResult OutRebate() 
        {
            C_UserVM user = C_UserVM.GetVMByID(CurrentUser.ID);
            if (user.C_UserTypeID > 2) 
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "您还未达到总裁级别，没有应付的返利" });
            }
            ViewData["user"] = user;
            OutRebateStatistics reba = new OutRebateStatistics();
            DateTime lastMonDat = DateTime.Now.AddMonths(-1);
            reba.lastMonMoney = C_UserRebate.GetRebateMoney_tj("",lastMonDat, "未发放",CurrentUser.UserName);
            return View(reba);
        }

        /// <summary>
        /// 我推荐的人
        /// </summary>
        /// <param name="id">查谁推荐的人就填谁的UserName    为空则当前用户推荐的人</param>
        /// <returns></returns>
        public ActionResult My_tjs(string id,string jb="") 
        {
            C_UserVM user = null;
            if (!string.IsNullOrWhiteSpace(id)) 
            {
                user = C_UserVM.GetVMByUserName(id);
            }
            ViewData["user"] = user;
            ViewData["jb"] = jb;
            return View();
        }
        /// <summary>
        /// 读取我推荐的人
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadMy_tjs(C_UserSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Name like '%{0}%' or Phone like '%{0}%')", condition.keyword);
            }
            if (!string.IsNullOrWhiteSpace(condition.username))
            {
                where += string.Format(" and Introducer='{0}' ", condition.username);
            }
            else 
            {
                where += string.Format(" and Introducer='{0}' ", CurrentUser.UserName);
            }


            return GetMy_tjs(condition, where);
        }
        private ActionResult GetMy_tjs(C_UserSearch condition, string where)
        {
            PageJsonModel<C_User> page = new PageJsonModel<C_User>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@" C_User ");
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 应付返利列表
        /// </summary>
        /// <param name="dattype"></param>
        /// <returns></returns>
        public ActionResult OutRebateList_tj(string dattype)
        {
            ViewData["dattype"] = dattype;
            return View();
        }
        /// <summary>
        /// 读取应付返利列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadOutRebateList(RebateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name like '%" + condition.keyword + "%'";
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateMon))
            {
                condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

                DateTime DatMon = new DateTime();
                DateTime.TryParse(condition.DatCreateMon, out DatMon);

                where += string.Format("  and datepart(mm,DatCreat)=datepart(mm,'{0}') ", DatMon.ToString("yyyy/MM/dd"));
            }

            return GetOutRebateListPage(condition, where);
        }



        /// <summary>
        /// 读取应付返利列表
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        private ActionResult GetOutRebateListPage(RebateSearch condition, string where)
        {
            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@"( select  C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat) 年,
                                month(DatCreat) 月,
                                sum(C_UserRebate.Money) 返利合计,C_UserRebate.[State]
                                from C_UserRebate left join C_User on C_UserRebate.R_People=C_User.UserName
                                where C_UserRebate.Cat='推荐返利' and C_UserRebate.Issuer='{0}' {1}
                                group by C_UserRebate.R_People,C_User.Name,C_User.Phone,year(DatCreat),
                                month(DatCreat),C_UserRebate.[State] ) as Show", CurrentUser.UserName, where);
            page.strSelect = " * ";
            page.strWhere = "";
            page.strOrder = "年,月 desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 应付返利明细
        /// </summary>
        /// <param name="dattype"></param>
        /// <returns></returns>
        public ActionResult OutRebateDetail_tj(string ID, string mon)
        {
            ViewData["mon"] = mon;

            C_UserVM user = C_UserVM.GetVMByUserName(ID);
            if (user == null) 
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "找不到人了" });
            }
            ViewData["user"] = user;
            return View();
        }
        /// <summary>
        /// 读取应付返利明细
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadOutRebateDetail(RebateSearch condition)
        {
            PageJsonModel<C_UserRebate> page = new PageJsonModel<C_UserRebate>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" C_UserRebate left join [Order] on C_UserRebate.OrderNo=[Order].OrderNo left join C_User on [Order].UserName=C_User.UserName ");
            page.strSelect = " C_UserRebate.*,C_User.Name order_CName ";
            page.strWhere = string.Format(" and [Issuer]='{0}' and R_People='{1}' ", CurrentUser.UserName,condition.UserName);

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                page.strWhere += string.Format(" and C_UserRebate.OrderNo='{0}' ", condition.keyword);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateMon))
            {
                condition.DatCreateMon = Common.Filter(condition.DatCreateMon);

                DateTime DatMon = new DateTime();
                DateTime.TryParse(condition.DatCreateMon, out DatMon);

                page.strWhere += string.Format("  and datepart(mm,C_UserRebate.DatCreat)=datepart(mm,'{0}') ", DatMon.ToString("yyyy/MM/dd"));
            }
            if (!string.IsNullOrWhiteSpace(condition.Cat))
            {
                page.strWhere += string.Format("  and C_UserRebate.Cat='{0}' ", condition.Cat);
            }

            page.strOrder = "C_UserRebate.DatCreat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发放
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mon"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
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
            string rtn = C_UserRebate.UpdateRebateState(ID, DatMon, cat,CurrentUser.UserName);
            return Content(rtn);
        }
    }
}
