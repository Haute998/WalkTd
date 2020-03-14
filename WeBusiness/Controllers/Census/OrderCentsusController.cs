using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class OrderCentsusController : BaseController
    {
        //
        // GET: /OrderCentsus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult GetSaleYearCensus(int Year)
        //{
        //    string SelVal = SaleCensus.GetSalesOrder(Year);
        //    if (string.IsNullOrWhiteSpace(SelVal))
        //    {
        //        return Content("");
        //    }
        //    return Content(SelVal);
        //}

    }
}
