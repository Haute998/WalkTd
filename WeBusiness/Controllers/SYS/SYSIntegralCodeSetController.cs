using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSIntegralCodeSetController : BaseController
    {
        //
        // GET: /SYSIntegralCodeSet/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(SYSIntegralCodePrizesVMSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (WaterCode like '%{0}%' or IntegralCode like '%{0}%')", Common.Filter(condition.keyword));
            }
            if (!string.IsNullOrWhiteSpace(condition.State))
            {
                where += string.Format(" and [State]='{0}'", Common.Filter(condition.State));
            }
            if (condition.PrizesID != 0)
            {
                where += string.Format(" and [PrizesID]={0}", condition.PrizesID);
            }
            if (condition.ActivityID != 0 && condition.PrizesID == 0)
            {
                where += string.Format(" and LotteryActivitys.ID={0}", condition.ActivityID);
            }

            if (condition.AreaID != 0)
            {
                where += string.Format(" and SYSIntegralCodeArea.ID={0}", condition.AreaID);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(SYSIntegralCodePrizesVMSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSIntegralCodePrizesVM> page = new PageJsonModel<SYSIntegralCodePrizesVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @" [SYSIntegralCode] 
                                   left join SYSIntegralCodeArea on [SYSIntegralCode].AreaID=SYSIntegralCodeArea.ID
                                   left join LotteryPrizes on [SYSIntegralCode].PrizesID=LotteryPrizes.ID 
                                   left join LotteryActivitys on LotteryPrizes.ActivityID=LotteryActivitys.ID ";
            page.strSelect = " [SYSIntegralCode].*,SYSIntegralCodeArea.AreaName,isnull(LotteryPrizes.PrizeLevel,'') PrizeLevel,isnull(LotteryPrizes.PrizeName,'') PrizeName,isnull(LotteryActivitys.Title,'') activityName ";
            page.strWhere = where;
            page.strOrder = "[SYSIntegralCode].ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("修改", "Index")]
        public ActionResult SetPrize(int id)
        {
            SYSIntegralCode para = SYSIntegralCode.GetEntityByID(id);
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToSetPrize(int id, int prizeID)
        {
            //if (prizeID==0)
            //{
            //    return Content("奖品不存在");
            //}
            int rtn = SYSIntegralCode.SetPrizeByID(id, prizeID);
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("设置出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult getPrizesByActivityID(int ActivityID)
        {
            List<LotteryPrizes> Prizes = LotteryPrizes.GetEntitysByActivityID(ActivityID);
            return Json(Prizes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量设置区域
        /// </summary>
        /// <returns></returns>
        public ActionResult toSetAreas(string WaterCodeBStr, string WaterCodeEStr, int AreaID)
        {

            string rtn = SYSIntegralCode.AreaIDSets(WaterCodeBStr, WaterCodeEStr, AreaID);
            return Content(rtn);
        }

    }
}
