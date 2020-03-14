using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models;

namespace WeBusiness.Controllers
{
    public class OutStockController : BaseController
    {
        //
        // GET: /OutStock/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 不入库出货
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult OutStokeIndex()
        {
            return View();
        }

        [B_MenuRightsTag("导出", "KucunStockIndex")]
        public ActionResult ExportExcel(C_UserSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append(@"select b.ProductNumber,b.ProductName,COUNT(SmallCode) as SmallCount,sum(b.kw) as [money]
                            from Scale as a left join Product as b on a.ProductNo=b.ProductNumber
                            where StateID in (6,9)");

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where.Append(string.Format(" and (b.ProductName like '%{0}%' )", condition.keyword));
            }
            where.Append(@" group by b.ProductName,b.ProductNumber,b.ProductImg");

            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "产品编号", "产品名称", "库存数量" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "出库统计信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        /// <summary>
        /// 不入库出货
        /// </summary>
        /// <param name="ScaleOutStoke"></param>
        /// <returns></returns>
        public ActionResult NoOutStockInsert(ScaleOutStoke ScaleOutStoke)
        {
            List<Scale> BigScale = Scale.GetBigScaleList(ScaleOutStoke.Code);
            List<Scale> SmallScale = Scale.GetSmallScaleList(ScaleOutStoke.Code);
            List<Scale> CodeScale = new List<Scale>();
            string msg = string.Empty;
            if (BigScale.Count > 0)
            {
                CodeScale = BigScale;
            }
            else if (SmallScale.Count > 0)
            {
                CodeScale = SmallScale;
            }
            else
            {
                return Content("没有此条码！！");
            }
            foreach (Scale item in CodeScale)
            {
                if (Scale.GetBoolInCode(item.SmallCode))
                {
                    msg += item.SmallCode + "条码已出库<br/>";
                }
                else if (!ScaleOutStoke.GetInScale(item.SmallCode))
                {
                    ScaleOutStoke.BigCode = item.BigCode;
                    ScaleOutStoke.SmallCode = item.SmallCode;
                    ScaleOutStoke.AntiCode = item.AntiCode;
                    ScaleOutStoke.Code = item.SmallCode;
                    ScaleOutStoke.Shipper = "总部";
                    ScaleOutStoke.State = "启用";
                    ScaleOutStoke.InsertAndReturnIdentity();
                    Scale.GetUpdateScaleOutState(item.SmallCode);
                    msg += item.SmallCode + "条码出货成功！<br/>";
                }
                else
                {
                    msg += item.SmallCode + "条码已出货！<br/>";
                }
            }
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = "条码已出货";
            }
            return Content(msg);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult KucunStockIndex()
        {
            return View();
        }
        public ActionResult GetBigCodePageKuCun(BigScaleSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += " and ( b.ProductNumber+b.ProductName like '%" + condition.BigCode + "%' )";
            }

            PageJsonModel<BigScale> page = new PageJsonModel<BigScale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select COUNT(SmallCode) as SmallCount,sum(b.kw) as [money],b.ProductName,b.ProductNumber,b.ProductImg" +
                           " from Scale as a left join Product as b on a.ProductNo=b.ProductNumber" +
                           " where StateID in (6,9)" + where +
                           " group by b.ProductName,b.ProductNumber,b.ProductImg) as BigScale";
            page.strSelect = " * ";
            
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = " ProductName ";
            }
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                ScaleCodeCount OutCodeCount = Scale.GetStockCount(where);
                page.pageResponse.SmallCount = OutCodeCount.SmallCount;
                page.pageResponse.SumPrice = OutCodeCount.SumPrice;
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 出货
        /// </summary>
        /// <param name="ScaleOutStoke"></param>
        /// <returns></returns>
        [B_MenuRightsTag("出货", "Index")]
        public ActionResult OutStockInsert(string SmallCodeSet, string OutOrderNo, string CUserName)
        {
            List<Scale> CodeScale = new List<Scale>();
            string msg = string.Empty;

            int iRow = Scale.UpdateScaleOutState(SmallCodeSet, CurrentUser.UserName, OutOrderNo, CUserName);

            if (iRow > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "出货失败！";
            }

            return Content(msg);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult OutStockDetailIndex()
        {
            return View();
        }
        [B_MenuRightsTag("小标明细", "OutStockDetailIndex")]
        public ActionResult OutSmallStockDetailIndex(string Time,string OrderNo,string Bigcode,string Middlecode,string PNo)
        {
            ViewData["Time"] = Time;
            ViewData["OrderNo"] = OrderNo;
            ViewData["Bigcode"] = Bigcode;
            ViewData["Middlecode"] = Middlecode;
            ViewData["PNo"] = PNo;
            
            return View();
        }
        [B_MenuRightsTag("出货明细", "OutSmallStockDetailIndex")]
        public ActionResult OutStocksDetailIndex(string ID)
        {
            ViewData["SmallCode"] = ID;
            return View();
        }
        public ActionResult GetOutSmallCodePage(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and Name like '%" + condition.keyword + "%'";
            }
            where += " and SmallCode='" + condition.SmallCode + "'";
            return GetPages(condition, where);
        }
        public ActionResult GetSmallCodePage(Scale condition)
        {   
            string where = string.Empty;
            if (condition.OutTime != 0)
            {
                where += string.Format(" and CreateTime={0}", condition.OutTime);
            }
            if (!string.IsNullOrWhiteSpace(condition.OutOrderNo))
            {
                where += string.Format(" and OutOrderNo='{0}'", Common.Filter(condition.OutOrderNo));
            }
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += string.Format(" and BigCode='{0}'", Common.Filter(condition.BigCode));
            }
            if (!string.IsNullOrWhiteSpace(condition.MiddleCode))
            {
                where += string.Format(" and Middlecode='{0}'", Common.Filter(condition.MiddleCode));
            }
            if (!string.IsNullOrWhiteSpace(condition.ProductNo))
            {
                where += string.Format(" and ProductNumber='{0}'", Common.Filter(condition.ProductNo));
            }

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and SmallCode='{0}'", Common.Filter(condition.keyword));
            }

            return GetPages(condition, where);
        }
        private ActionResult GetPages(Scale condition, string where)
        {
            PageJsonModel<ScaleOutStokeShow> page = new PageJsonModel<ScaleOutStokeShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select s.Shipper,s.BigCode,s.MiddleCode,s.SmallCode,s.AntiCode,s.CreateTime,s.OutOrderNo,s.ProductName,s.ProductNumber,s.ProductImg,c.Name " +
                            "from (select s.SmallCode,s.Consignee, s.Shipper, s.BigCode,s.MiddleCode,"+
                            "s.AntiCode,s.CreateTime,s.OutOrderNo,p.ProductName,p.ProductNumber,"+
                            "p.ProductImg ProductImg from ScaleOutStoke s left join Product p on s.ProductNo=p.ProductNumber where s.Shipper='总部') s left join C_User c on s.Consignee=c.UserName) as OutStockScaleShow";
            page.strSelect = " * ";
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
        public ActionResult GetBigCodePage(BigScaleSearch condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.KeyWord))
            {
                where += string.Format(" and ( BigCode='{0}' or MiddleCode='{0}' or SmallCode='{0}')", Common.Filter(condition.KeyWord));
            }
            if (!string.IsNullOrWhiteSpace(condition.ProductNumber))
            {
                where += string.Format(" and ProductNo='{0}'", condition.ProductNumber);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and OutTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and OutTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }
            if (!string.IsNullOrWhiteSpace(condition.OutOrderNo))
            {
                where += string.Format(" and OutOrderNo like '%{0}%' ", Common.Filter(condition.OutOrderNo));
            }

            PageJsonModel<ScaleOutStokeDetails> page = new PageJsonModel<ScaleOutStokeDetails>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;

            page.strSelect = " a.OutOrderNo,a.BigCode,a.MiddleCode,a.SmallCode,a.OutTime,b.ProductImg,b.ProductNumber,b.ProductName,a.OutPDAUser,a.UserName,c.Name C_Name  ";
            page.strForm = " Scale as a left join Product b on a.ProductNo=b.ProductNumber left join C_User as c on a.UserName=c.UserName ";
            page.strWhere = " and a.IsOut=1 " + where;
            page.strOrder = " a.OutTime desc";
            
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        [B_MenuRightsTag("查看")]
        public ActionResult GetOutStockCencusIndex()
        {
            return View();
        }
        public ActionResult GetOutStockCencusPage(OutStockCensusShow condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( BigCode='{0}' or MiddleCode='{0}' or SmallCode='{0}')", Common.Filter(condition.keyword));
            }
            if (!string.IsNullOrWhiteSpace(condition.ProductNumber))
            {
                where += string.Format(" and ProductNo='{0}'", condition.ProductNumber);
            }
            if (!string.IsNullOrWhiteSpace(condition.OutOrderNo))
            {
                where += string.Format(" and OutOrderNo='{0}'", condition.OutOrderNo);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and CreateTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and CreateTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            return GetOutStockCencusPages(condition, where);
        }
        private ActionResult GetOutStockCencusPages(OutStockCensusShow condition, string where)
        {
            PageJsonModel<OutStockCensus> page = new PageJsonModel<OutStockCensus>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            string strForm = "";
            strForm = @"(select count(*) count,s.CreateTime,s.OutOrderNo,s.BigCode,s.MiddleCode,s.Consignee Consignee, s.Shipper Shipper,p.ProductName ProductName,p.ProductNumber ProductNumber " +
                        "from ScaleOutStoke s left join Product p on s.ProductNo=p.ProductNumber where Shipper='总部' " + where;
            strForm += " group by s.CreateTime,s.OutOrderNo,s.BigCode,s.MiddleCode,s.Consignee,s.Shipper,p.ProductName,p.ProductNumber) as OutStockScaleShow left join C_User as c on c.UserName=OutStockScaleShow.Consignee";
            page.strForm = strForm;
            page.strSelect = " OutStockScaleShow.*,c.Name C_Name ";
            
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "OutStockScaleShow.CreateTime desc";
            }
            
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                ScaleCodeCount OutCodeCount = ScaleOutStoke.GetC_UserOutCount(where);
                page.pageResponse.BigCount = OutCodeCount.BigCount;
                page.pageResponse.MiddleCount = OutCodeCount.MiddleCount;
                page.pageResponse.SmallCount = OutCodeCount.SmallCount;
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ContentResult DelOutStockCode(string CodeSet)
        {
            if (ScaleOutStoke.BatchIsSubOutStock(CodeSet))
            {
                return Content("有编码已出货下级，无法删除！");
            }
            else
            {
                int iRet = Scale.BatchRemoveOutSmall(CodeSet);

                SYSLog.add("电脑端用户删除出货", "用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")删除了Code为(" + CodeSet + ")的条码入库，ip为" + Request.UserHostAddress, "/OutStock/DelOutStockCode", "删除出货", "电脑端后台");

                if (iRet > 0) return Content("ok");
                else return Content("删除失败！");
            }
        }
    }
}
