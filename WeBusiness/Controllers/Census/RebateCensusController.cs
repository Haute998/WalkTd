using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class RebateCensusController : BaseController
    {
        //
        // GET: /RebateCensus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetRebateYearCensus(int Year)
        {
            string SelVal = SaleCensus.GetSaleRebate(Year);
            if (string.IsNullOrWhiteSpace(SelVal))
            {
                return Content("");
            }
            return Content(SelVal);
        }
    }
}
