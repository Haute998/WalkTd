using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class SaleCensusController : BaseController
    {
        //
        // GET: /SaleCensus/
        //采购统计
        public ActionResult AgentUserNameIndex(int ID)
        {
            C_UserVM cuser = C_UserVM.GetVMByID(ID);
            ViewData["user"] = cuser;
            DateTime dt = DateTime.Now;
            ViewData["TheMonthSale"] = OrderCensus.GetLastMonthVal(string.Format("and UserName='" + cuser.UserName + "' and DatAudit >='{0}-{1}-01 00:00:00' and DatAudit <='{2}-{3}-01 00:00:00'", dt.Year, dt.Month, dt.AddMonths(1).Year, dt.AddMonths(1).Month))[0].SumPrice;
            ViewData["Sale"] = OrderCensus.GetLastMonthVal(" and UserName='" + cuser.UserName + "'")[0].SumPrice;
            return View();
        }
        //采购统计
        public ActionResult GetLastMonthValue(string ID)
        {
            string where = string.Empty;
            int month = 0;
            int year = 0;
            int Nextmonth = 0;
            int Nextyear = 0;
            List<decimal> SumVal = new List<decimal>();
            DateTime dt = DateTime.Now;
            for (int i = 5; i >= 1; i--)
            {

                year = dt.AddMonths(-i).Year;
                month = dt.AddMonths(-i).Month;
                Nextyear = dt.AddMonths(-i + 1).Year;
                Nextmonth = dt.AddMonths(-i + 1).Month;
                where = " and UserName='" + ID + "'";
                where += string.Format(" and DatAudit >='{0}-{1}-01 00:00:00' ", year, month);
                where += string.Format(" and DatAudit <'{0}-{1}-01 00:00:00' ", Nextyear, Nextmonth);
                List<OrderCensusShow> list = OrderCensus.GetLastMonthVal(where);
                SumVal.Add(list[0].SumPrice);
            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(SumVal));
        }
        //销售统计
        public ActionResult AgentUserNameIndexSale(int ID)
        {
            C_UserVM cuser = C_UserVM.GetVMByID(ID);
            ViewData["user"] = cuser;
            DateTime dt = DateTime.Now;
            ViewData["TheMonthSale"] = OrderCensus.GetLastMonthVal(string.Format("and ParentUser='" + cuser.UserName + "' and DatAudit >='{0}-{1}-01 00:00:00' and DatAudit <='{2}-{3}-01 00:00:00'", dt.Year, dt.Month, dt.AddMonths(1).Year, dt.AddMonths(1).Month))[0].SumPrice;
            ViewData["Sale"] = OrderCensus.GetLastMonthVal(" and ParentUser='" + cuser.UserName + "'")[0].SumPrice;
            return View();
        }
        //销售统计
        public ActionResult GetLastMonthValueSale(string ID)
        {
            string where = string.Empty;
            int month = 0;
            int year = 0;
            int Nextmonth = 0;
            int Nextyear = 0;
            List<decimal> SumVal = new List<decimal>();
            DateTime dt = DateTime.Now;
            for (int i = 5; i >= 1; i--)
            {

                year = dt.AddMonths(-i).Year;
                month = dt.AddMonths(-i).Month;
                Nextyear = dt.AddMonths(-i + 1).Year;
                Nextmonth = dt.AddMonths(-i + 1).Month;
                where = " and ParentUser='" + ID + "'";
                where += string.Format(" and DatAudit >='{0}-{1}-01 00:00:00' ", year, month);
                where += string.Format(" and DatAudit <'{0}-{1}-01 00:00:00' ", Nextyear, Nextmonth);
                List<OrderCensusShow> list = OrderCensus.GetLastMonthVal(where);
                SumVal.Add(list[0].SumPrice);
            }
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(SumVal));
        }

    }
}
