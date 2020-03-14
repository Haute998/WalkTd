using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class ScaleCensusController : BaseController
    {
        //
        // GET: /ScaleCensus/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 会员分析
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetC_ConsumerYearCensus(int Year)
        {
            string SelVal = SaleCensus.GetCountC_Consumer(Year,"消费者");
            if (string.IsNullOrWhiteSpace(SelVal))
            {
                return Content("");
            }
            return Content(SelVal);
        }











        /// <summary>
        /// 出入库统计
        /// </summary>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetScaleYearCensus(int Year)
        {
            string SelVal = SaleCensus.GetCountSclae(Year);
            if (string.IsNullOrWhiteSpace(SelVal))
            {
                return Content("");
            }
            return Content(SelVal);
        }

    }
}
