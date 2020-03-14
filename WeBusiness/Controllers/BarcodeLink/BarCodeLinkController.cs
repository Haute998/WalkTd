using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class BarCodeLinkController : BaseController
    {
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllBarCodeLinkData(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += string.Format(" and (a.ID in(select ScaleId from Scale_Big where BigCode='{0}') ", condition.BigCode);
                where += string.Format(" or a.ID in(select ScaleId from Scale_Middle where MiddleCode='{0}') ", condition.BigCode);
                where += string.Format(" or a.ID in(select ScaleId from Scale_Small where SmallCode='{0}')) ", condition.BigCode);
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and a.LinkMidTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and (a.LinkMidTime<>0 and a.LinkMidTime <={0}) ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }

            if (!string.IsNullOrWhiteSpace(condition.LinkMidOrderNo))
            {
                where += string.Format(" and a.LinkMidOrderNo='{0}'", Common.FilteSQLStr(condition.LinkMidOrderNo));
            }

            if (CurrentUser.DeptID != 0)
            {
                where += " and c.ID=" + CurrentUser.DeptID.ToString();
            }

            PageJsonModel<Scale> page = new PageJsonModel<Scale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "Scale as a left join PDAUser as b on a.LinkMidPDAUser=b.PUserName left join Supplier as c on b.SupplierID=c.id ";
            page.strSelect = " a.ID,StateID,MiddleCode,SmallCode,LinkMidTime,a.LinkMidPDAUser,a.LinkMidOrderNo,b.PRealName as Name,c.Name as SupplierName ";
            page.strWhere = " and a.IsLinkMid=1 " + where;
            page.strOrder = " a.LinkMidTime desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ContentResult RelieveBarCodeLink(string IDSet)
        {
            if (!string.IsNullOrWhiteSpace(IDSet))
            {
                int iRet = Scale.BatchUnboxingSmall(IDSet);

                SYSLog.add("电脑端用户拆除小标装箱", "用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")拆除了ID为(" + IDSet + ")，ip为" + Request.UserHostAddress, "/BarCodeLink/RelieveBarCodeLink", "拆除小标装箱", "电脑端后台");

                if (iRet > 0) return Content("ok");
                else return Content("解除失败！");
            }
            else
            {
                return Content("没有可解除的项");
            }
        }

        public ContentResult RelieveBigCodeLink(string MiddleCodeSet)
        {
            if (!string.IsNullOrWhiteSpace(MiddleCodeSet))
            {
                int iRet = Scale.BatchUnboxingMiddle(MiddleCodeSet);
                SYSLog.add("电脑端用户拆除中标装箱", "用户" + CurrentUser.Name + "(" + CurrentUser.UserName + ")拆除了条码为(" + MiddleCodeSet + ")，ip为" + Request.UserHostAddress, "/BarCodeLink/RelieveBigCodeLink", "拆除中标装箱", "电脑端后台");
                if (iRet > 0) return Content("ok");
                else return Content("解除失败！");
            }
            else
            {
                return Content("没有可解除的项");
            }
        }

        public ActionResult BigCodeLink()
        {
            return View();
        }

        public ActionResult GetBigCodeLinkData(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += string.Format(" and (a.ID in(select ScaleId from Scale_Big where BigCode='{0}') ", condition.BigCode);
                where += string.Format(" or a.ID in(select ScaleId from Scale_Middle where MiddleCode='{0}') ", condition.BigCode);
                where += string.Format(" or a.ID in(select ScaleId from Scale_Small where SmallCode='{0}')) ", condition.BigCode);
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and a.LinkBigTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and (a.LinkBigTime<>0 and a.LinkBigTime <={0}) ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }
            if (!string.IsNullOrWhiteSpace(condition.LinkBigOrderNo))
            {
                where += string.Format(" and a.LinkBigOrderNo='{0}'", Common.FilteSQLStr(condition.LinkBigOrderNo));
            }
            if (CurrentUser.DeptID != 0)
            {
                where += " and c.ID=" + CurrentUser.DeptID.ToString();
            }

            PageJsonModel<Scale> page = new PageJsonModel<Scale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select distinct StateID,BigCode,MiddleCode,LinkBigTime,a.LinkBigPDAUser,a.LinkBigOrderNo,b.PRealName as Name,c.Name as SupplierName " +
                           " from Scale as a left join PDAUser as b on a.LinkBigPDAUser=b.PUserName left join Supplier as c on b.SupplierID=c.id "+
                           " where a.IsLinkBig=1 " + where + ")as TempTB ";
            page.strSelect = " StateID,BigCode,MiddleCode,LinkBigTime,LinkBigPDAUser,LinkBigOrderNo,Name,SupplierName ";
            page.strOrder = " LinkBigTime desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}