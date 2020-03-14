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
    public class LotteryRecordController : BaseController
    {
        //
        // GET: /LotteryRecord/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object PMoney0obj = DAL.SqlHelper.ExecuteScalar(string.Format("SELECT sum(redMoney)as PMoney0 FROM  LotteryRecord "), paramters);
            decimal PMoney0 = 0;
            try
            {
                PMoney0 = PMoney0obj == null ? 0 : Convert.ToDecimal(PMoney0obj);
            }
            catch (Exception)
            {                
                //throw;
            }
           // decimal PMoney0 = PMoney0obj == null ? 0 : Convert.ToDecimal(PMoney0obj);
            ViewData["PMoney0"] = PMoney0;
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel(LotteryRecordSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select [SerialNumber],[Dat],[IntegralCode],redMoney from [LotteryRecord] where 1=1 ");
            where.Append(LotteryRecordSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "流水号", "中奖时间", "防伪码", "红包金额" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "中奖纪录" + DateTime.Now.ToShortTimeString() + ".xls");
        }



        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetRecords(LotteryRecordSearch condition)
        {
            string where = LotteryRecordSearch.StrWhere(condition);

            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.pageIndex = condition.pageIndex;
            page.strForm = " LotteryRecord ";
            page.strSelect = "* ";
            page.pageSize = condition.pageSize;
            page.strWhere = where;
            page.strOrder = " ID Desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("订单详情", "Index")]
        public ActionResult RecordDetail(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "流水号有误" });
            }
            LotteryRecordVM rec = new LotteryRecordVM();
            rec.LoadRecord(serialNumber);
            return View(rec);
        }

    }
}
