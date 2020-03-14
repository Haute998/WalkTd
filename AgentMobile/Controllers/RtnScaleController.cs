using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class RtnScaleController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }
            return View();
        }

        public ActionResult ScanRtnStock()
        {
            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }
            string orderno = DateTime.Now.ToString("yyMMddHHmmssf");
            ViewData["OrderNo"] = orderno;
            return View();
        }
        public ActionResult GetBoolRtnScale(string ID)
        {
            RequestResult result = new RequestResult();

            List<BarCode> CodeList = ScaleOutStoke.GetCanRtnCode(CurrentUser.UserName, ID);

            if (CodeList != null && CodeList.Count > 0)
            {
                result.data = CodeList;
                result.message = "正确";
                result.success = true;
            }
            else
            {
                result.message = "未找到条码,或者此条码你无权退货";
                result.success = false;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetUpdateRtnScale(string Scale,string RtnOrderNo)
        {
            RequestResult result = new RequestResult();
            bool IsOK = true;

            if (string.IsNullOrWhiteSpace(Scale))
            {
                IsOK = false;
                result.message = "正确条码不能为空";
                result.success = true;
            }

            if (IsOK)
            {
                List<BarCode> SmallCodeList = ScaleOutStoke.GetRtnStockID(CurrentUser.UserName, Scale);

                string IDSet = "";
                foreach (BarCode b in SmallCodeList)
                {
                    if (IDSet != "") IDSet += ",";
                    IDSet += b.ID.ToString();
                }

                if (IDSet != "") RtnStockScale.ToRtnStockCode(CurrentUser.UserName, IDSet, RtnOrderNo);

                result.data = SmallCodeList;
                result.message = "成功";
                result.success = true;
            }

            //string[] Scales = Scale.Split(',');
            //string msg = string.Empty;
            //for (int i = 0; i < Scales.Length - 1; i++)
            //{
            //    if (RtnStockScale.GetBoolC_UserOutScale(CurrentUser.UserName, Scales[i].Trim()) != "ok")
            //    {
            //        msg += Scales[i].Trim() + "添加失败!";
            //    }
            //    else
            //    {
            //        msg += "ok";
            //    }
            //}
            //if (string.IsNullOrWhiteSpace(msg) || msg.Contains("ok"))
            //{
            //    msg = "ok";
            //}
            //return Content(msg);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RtnScaleDetail()
        {
            return View(CurrentUser);
        }
        /// <summary>
        /// 退货记录列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult GetC_UserOutScaleDetail(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name+OrderNo+ProductName like '%" + condition.keyword + "%'";
            }
            if (!string.IsNullOrEmpty(condition.DatCreateB))
            {
                where += string.Format(" and ReturnTime>={0}", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrEmpty(condition.DatCreateE))
            {
                where += string.Format(" and ReturnTime<={0}", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            PageJsonModel<RtnStockCensus> page = new PageJsonModel<RtnStockCensus>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select COUNT(*) SmallCount,s.Shipper, s.OrderNo,c.Name,s.ReturnTime,p.ProductName,p.ProductImg,p.ProductNumber,Consignee " +
                           " from ScaleRtnStoke s left join C_User c on s.Consignee=c.UserName left join Product as p on s.ProducctNo=p.ProductNumber " +
                           " where s.Shipper='" + CurrentUser.UserName + "'" +
                           " group by s.OrderNo ,c.Name,s.ReturnTime,s.Shipper,p.ProductName,p.ProductImg,p.ProductNumber,Consignee )as Detail";
            page.strSelect = "* ";
            page.strWhere =  where;
            page.strOrder = "ReturnTime desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 退货祥情页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOutDetailIndex(string OrderNo, string ProductNo, string Consignee, int IDateTime)
        {
            ViewData["OrderNo"] = OrderNo;
            ViewData["ProductNo"] = ProductNo;
            ViewData["Consignee"] = Consignee;
            ViewData["IDateTime"] = IDateTime;
            return View();
        }
        public ActionResult GetOuttjIndex(string ID)
        {
            ViewData["OrderNo"] = ID;
            return View();
        }
        public ActionResult GetC_UserScaleDetail(ScaleRtnSeacher condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and isnull(BigCode,'')+isnull(MiddleCode,'')+SmallCode like '%" + condition.keyword + "%'";
            }

            PageJsonModel<OutScaleDetail> page = new PageJsonModel<OutScaleDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;

            page.strForm = "ScaleRtnStoke";
            page.strSelect = "BigCode,MiddleCode,SmallCode";
            page.strWhere = string.Format(" and Shipper='{0}' and OrderNo='{1}' and ProducctNo='{2}' and ReturnTime={3} and Consignee='{4}'", CurrentUser.UserName, condition.OrderNo, condition.ProducctNo, condition.ReturnTime, condition.Consignee) + where;
            page.strOrder = "SmallCode asc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetC_UserScaletj(ScaleOutStokeShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and ProductNumber+ProductName like '%" + condition.keyword + "%'";
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and ReturnTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and ReturnTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            PageJsonModel<RtnStockCensus> page = new PageJsonModel<RtnStockCensus>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;

            page.strForm = "(select count(SmallCode) as SmallCount,s.Shipper,p.ProductNumber,p.ProductName,p.ProductImg " +
                            "from ScaleRtnStoke s left join Product p on s.ProducctNo=p.ProductNumber " +
                            "where s.Shipper='" + CurrentUser.UserName + "'" + where +
                            "group by s.Shipper,p.ProductName,p.ProductNumber,p.ProductImg) as Detail";
       
            page.strSelect = "* ";
            page.strOrder = "SmallCount desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
