using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models;

namespace WeBusiness.Controllers
{
    public class RtnStockController : BaseController
    {   
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

         [B_MenuRightsTag("退货", "Index")]
        public ActionResult GetStockRtn(string SmallCodeSet, string RtnOrderNo)
        {
            List<Scale> CodeScale = new List<Scale>();
            string msg = string.Empty;

            int iRow = Scale.UpdateScaleRtnState(SmallCodeSet, RtnOrderNo, CurrentUser.UserName);

            if (iRow > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "退货失败！";
            }
            
            return Content(msg);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult RtnStockIndex()
        {
            return View();
        }

        public ActionResult GetBigCodePage(BigScaleSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.KeyWord))
            {
                where += string.Format(" and ProductNumber+ProductName+ProducctNo+BarCode like '%{0}%'", Common.Filter(condition.KeyWord));
            }
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += string.Format(" and OrderNo like '{0}%'", Common.Filter(condition.OrderNo));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and ReturnTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and ReturnTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            PageJsonModel<ScaleRtnStokeDetail> page = new PageJsonModel<ScaleRtnStokeDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "View_RtnRecord";
            page.strSelect = " OrderNo, RtnWay, BarCode, ProducctNo, ReturnTime, OperaUser, BarCount, ProductImg, ProductNumber, ProductName, PUserName, PRealName, Name ";
            page.strWhere = where;
            page.strOrder = " ReturnTime desc";
            page.LoadList();
            
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("小标明细", "RtnStockIndex")]
        public ActionResult InSmallStockDetailIndex(string Time, string OrderNo, string Bigcode, string Middlecode, string PNo)
        {
            ViewData["Time"] = Time;
            ViewData["OrderNo"] = OrderNo;
            ViewData["BigCode"] = Bigcode;
            ViewData["Middlecode"] = Middlecode;
            ViewData["PNo"] = PNo;
            return View();
        }

        public ActionResult RtnRecordDetail(string OrderNo, int RtnWay, string BarCode)
        {
            ViewData["OrderNo"] = OrderNo;
            ViewData["RtnWay"] = RtnWay;
            ViewData["BarCode"] = BarCode;
            return View();
        }

        public ActionResult GetRecordPageDetail(RtnStockSeacher condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += string.Format(" and OrderNo='{0}'", Common.Filter(condition.OrderNo));
            }

            if (condition.RtnWay != 0)
            {
                where += string.Format(" and RtnWay={0}", condition.RtnWay);

                string codeStr = "SmallCode";
                if (condition.RtnWay == 2) codeStr = "MiddleCode";
                if (condition.RtnWay == 3) codeStr = "BigCode";

                if (!string.IsNullOrWhiteSpace(condition.BarCode))
                {
                    where += string.Format(" and {0}='{1}'", codeStr, Common.Filter(condition.BarCode));
                }
            }

            return GetPages(condition, where);
        }

        public ActionResult GetSmallCodePage(RtnStockSeacher condition)
        {
            string where = string.Empty;

            if (condition.ReturnTime != 0)
            {
                where += string.Format(" and ReturnTime>={0} and ReturnTime<{1}", condition.ReturnTime, condition.ReturnTime + 86400);
            }
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += string.Format(" and OrderNo='{0}'", Common.Filter(condition.OrderNo));
            }
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += string.Format(" and BigCode='{0}'", Common.Filter(condition.BigCode));
            }
            if (!string.IsNullOrWhiteSpace(condition.MiddleCode))
            {
                where += string.Format(" and MiddleCode='{0}'", Common.Filter(condition.MiddleCode));
            }
            if (!string.IsNullOrWhiteSpace(condition.ProductNo))
            {
                where += string.Format(" and ProducctNo='{0}'", Common.Filter(condition.ProductNo));
            }

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and SmallCode like '%{0}%'", Common.Filter(condition.keyword));
            }

            return GetPages(condition, where);
        }

        private ActionResult GetPages(RtnStockSeacher condition, string where)
        {
            PageJsonModel<ScaleRtnStokeDetail> page = new PageJsonModel<ScaleRtnStokeDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "ScaleRtnStoke s left join C_User as c on c.UserName=s.Consignee";
            page.strSelect = " s.ReturnTime,s.BigCode,s.MiddleCode,s.SmallCode,s.OrderNo,s.OutTime,s.OutOrderNo,c.Name ";
            page.strWhere = where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ReturnTime desc";
            }
            
            page.LoadList();
            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult GetRtnStockCencusIndex()
        {
            return View();
        }
        public ActionResult GetRtnStockCencusPage(RtnStockCensusShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( BigCode='{0}' or MiddleCode='{0}' or SmallCode='{0}')", Common.Filter(condition.keyword));
            }
            if (!string.IsNullOrWhiteSpace(condition.ProductNumber))
            {
                where += string.Format(" and ProducctNo='{0}'", Common.Filter(condition.ProductNumber));
            }
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += string.Format(" and OrderNo='{0}'", condition.OrderNo);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and ReturnTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and ReturnTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            return GetRtnStockCencusPages(condition, where);
        }
        private ActionResult GetRtnStockCencusPages(RtnStockCensusShow condition, string where)
        {
            string strForm = @"(select ReturnTime,BigCode,MiddleCode,ProductNumber,ProductName,ProductImg,COUNT(SmallCode) as SmallCount,OperaUser,PRealName,OrderNo from ( " +
                                "select (ReturnTime/86400*86400-28800) as ReturnTime,BigCode,MiddleCode,SmallCode,ProductNumber,ProductName,ProductImg,OperaUser,PRealName,OrderNo " +
                                "from ScaleRtnStoke as a left join Product as b on a.ProducctNo=b.ProductNumber left join PDAUser as d on a.OperaUser=d.PUserName " +
                                "where a.Shipper='总部' " + where + ") as def " +
                                "group by ReturnTime,BigCode,MiddleCode,ProductNumber,ProductName,ProductImg,OperaUser,PRealName,OrderNo) as RtnCountTB";

            PageJsonModel<RtnStockCensus> page = new PageJsonModel<RtnStockCensus>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strSelect = " ReturnTime,BigCode,MiddleCode,ProductNumber,ProductName,ProductImg,OperaUser,PRealName,SmallCount,OrderNo ";
            page.strForm = strForm;
            
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ReturnTime desc";
            }
            
            page.LoadList();
            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                ScaleCodeCount OutCodeCount = RtnStockScale.GetRtnStockCount(where);
                page.pageResponse.BigCount = OutCodeCount.BigCount;
                page.pageResponse.MiddleCount = OutCodeCount.MiddleCount;
                page.pageResponse.SmallCount = OutCodeCount.SmallCount;

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}
