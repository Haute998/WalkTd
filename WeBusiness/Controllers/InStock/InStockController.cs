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
    public class InStockController : BaseController
    {
        //
        // GET: /InStock/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            ViewBag.ProductList = SelectListHelper.GetProducts();

            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult EditIndex()
        {
            return View();
        }

        [B_MenuRightsTag("导出", "InStockCensusIndex")]
        public ActionResult ExportExcel(C_UserSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append(@"select p.ProductNumber ProductNumber,p.ProductName ProductName,count(*) count from Scale s 
left join Product p on s.ProductNo=p.ProductNumber where s.StateID in (6,7,9) group by ProductName,p.ProductImg,p.ProductNumber ");
            //where.Append("and state='已审核'  and Chief=0  ");
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where.Append(string.Format(" and (ProductName like '%{0}%' )", condition.ProductName));
            }
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "产品编号", "产品名称", "数量" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "入库统计信息" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        [B_MenuRightsTag("入库", "Index")]
        public ActionResult StockInsert(string SmallCodeSet, string IntoOrderNo, string ProductNo)
        {
            List<Scale> CodeScale = new List<Scale>();
            string msg = string.Empty;

            int iRow = Scale.UpdateScaleInState(SmallCodeSet, CurrentUser.UserName,IntoOrderNo, ProductNo);

            if (iRow > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "入库失败！";
            }

            return Content(msg);
        }

        public ActionResult UpdateStockInsert(string code, string P_ID)
        {
            int SmallScale = InStockScale.getcount(code);

            if (SmallScale != 0)
            {
                int UPDATESmall = InStockScale.updatecount(code, P_ID);
            }
            else
            {
                return Content("没有此条码！！");
            }

            return Content("修改成功");
        }

        [B_MenuRightsTag("查看")]
        public ActionResult InStockDetailIndex()
        {
            return View();
        }

        [B_MenuRightsTag("小标明细", "InStockDetailIndex")]
        public ActionResult InSmallStockDetailIndex(string Time, string OrderNo, string Bigcode, string Middlecode, string PNo)
        {   
            ViewData["Time"] = Time;
            ViewData["OrderNo"] = OrderNo;
            ViewData["BigCode"] = Bigcode;
            ViewData["Middlecode"] = Middlecode;
            ViewData["PNo"] = PNo;
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult InoneStockDetailIndex()
        {
          
            return View();
        }
        public ActionResult GetoneCodePage(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and (SmallCode like '%" + condition.keyword + "%' or AntiCode like '%" + condition.keyword + "%' or P_Name like '%" + condition.keyword + "%' or O_ID like '%" + condition.keyword + "%' )";
            }
            where += " and BigCode IS NULL";
            return GetonePages(condition, where);
        }
        private ActionResult GetonePages(Scale condition, string where)
        {
            PageJsonModel<InStockScaleShow> page = new PageJsonModel<InStockScaleShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select s.BigCode BigCode,s.SmallCode SmallCode,s.AntiCode AntiCode,s.B_UserID B_UserID,s.DatCreate DatCreate,s.O_ID O_ID,p.ProductName P_Name,p.ProductImg P_ImgUrl from ScaleInStoke s left join Product p on s.P_ID=p.ProductNumber) as InStockScaleShow";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSmallCodePage(Scale condition)
        {
            string where = string.Empty;

            if(condition.IntoTime!=0)
            {
                where += string.Format(" and IntoTime={0}", condition.IntoTime);
            }
            if(!string.IsNullOrWhiteSpace(condition.IntoOrderNo))
            {
                where += string.Format(" and IntoOrderNo='{0}'", Common.Filter(condition.IntoOrderNo));
            }
            if(!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += string.Format(" and BigCode='{0}'", Common.Filter(condition.BigCode));
            }
            if(!string.IsNullOrWhiteSpace(condition.MiddleCode))
            {
                where += string.Format(" and MiddleCode='{0}'", Common.Filter(condition.MiddleCode));
            }
            if(!string.IsNullOrWhiteSpace(condition.ProductNo))
            {
                where += string.Format(" and ProductNo='{0}'",Common.Filter(condition.ProductNo));
            }

            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and SmallCode like '%{0}%'", Common.Filter(condition.keyword));
            }

            return GetPages(condition, where);
        }
        private ActionResult GetPages(Scale condition, string where)
        {
            PageJsonModel<InStockScaleShow> page = new PageJsonModel<InStockScaleShow>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " Scale as a left join Product as b on a.ProductNo=b.ProductNumber ";
            page.strSelect = " IntoOrderNo,SmallCode,IntoTime,b.ProductImg,b.ProductNumber,b.ProductName ";
            page.strWhere = " and IsInto=1 " + where;
            page.strOrder = "IntoTime desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBigCodePage(BigScaleSearch condition)
        {
            string where = string.Empty;

            if (!string.IsNullOrWhiteSpace(condition.KeyWord))
            {
                where += " and a.BigCode+a.MiddleCode+a.SmallCode+b.ProductName+b.ProductNumber like '%" + Common.FilteSQLStr(condition.KeyWord) + "%'";
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and a.IntoTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and a.IntoTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }
            if (!string.IsNullOrWhiteSpace(condition.PUserName) && condition.PUserName != "All")
            {
                where += string.Format(" and a.IntoPDAUser='{0}'", Common.FilteSQLStr(condition.PUserName));
            }

            if (CurrentUser.DeptID != 0)
            {
                where += " and c.ID=" + CurrentUser.DeptID.ToString();
            }

            PageJsonModel<ScaleInStokeDetail> page = new PageJsonModel<ScaleInStokeDetail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strSelect = " a.ID,a.IntoOrderNo,a.BigCode,a.MiddleCode,a.SmallCode,a.IntoTime,b.ProductImg,b.ProductNumber,b.ProductName,a.IntoPDAUser,c.PRealName ";
            page.strForm = " Scale a left join Product b on a.ProductNo=b.ProductNumber left join PDAUser as c on a.IntoPDAUser=c.PUserName ";
            page.strWhere = " and a.IsInto=1 " + where;
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = " a.IntoTime desc";
            }
            
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {   
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult InStockCensusIndex()
        {
            return View();
        }

        public ActionResult GetInStockCensusPage(InStockCensusShow condition)
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
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and IntoTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and IntoTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            return GetInStockCensusPages(condition, where);
        }
        private ActionResult GetInStockCensusPages(InStockCensusShow condition, string where)
        {
            string StrForm = "(select count(*) count,s.IntoOrderNo,s.BigCode,s.MiddleCode,p.ProductNumber ProductNumber,p.ProductName ProductName,s.IntoTime " +
                            "from Scale s left join Product p on s.ProductNo=p.ProductNumber where s.IsInto=1 " + where +
                            "group by ProductNumber,ProductName,s.IntoOrderNo,s.BigCode,s.MiddleCode,s.IntoTime) as InStockScaleShow ";

            PageJsonModel<InStockCensus> page = new PageJsonModel<InStockCensus>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = StrForm;
            page.strSelect = " * ";
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "IntoTime desc";
            }
            
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                ScaleCodeCount IntoStockCount = Scale.GetIntoStockCount(where);
                page.pageResponse.BigCount = IntoStockCount.BigCount;
                page.pageResponse.MiddleCount = IntoStockCount.MiddleCount;
                page.pageResponse.SmallCount = IntoStockCount.SmallCount;
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ContentResult DelIntoStockCode(string IDSet)
        {
            int iRet = Scale.BatchRemoveIntoSmall(IDSet);

            SYSLog.add("电脑端用户删除入库", "用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")删除了ID为(" + IDSet + ")的条码入库，ip为" + Request.UserHostAddress, "/InStock/DelIntoStockCode", "删除入库", "电脑端后台");

            if (iRet > 0) return Content("ok");
            else return Content("删除失败！");
        }
    }
}
