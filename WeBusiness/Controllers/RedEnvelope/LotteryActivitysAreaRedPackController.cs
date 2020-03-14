using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;


namespace WeBusiness.Controllers
{
    public class LotteryActivitysAreaRedPackController : BaseController
    {
        //
        // GET: /LotteryActivitysAreaRedPack/
        public ActionResult Index(int activityID)
        {
            LotteryActivitys activity = LotteryActivitys.GetEntityByID(activityID);
            ViewData["activity"] = activity;
            List<SYSIntegralCodeArea> areas = SYSIntegralCodeArea.GetEntitysAll();
            foreach (var item in areas)
            {
                LotteryActivitysAreaRedPack old = LotteryActivitysAreaRedPack.GetEntityActArea(activity.ID, item.ID);
                if (old == null)
                {
                    LotteryActivitysAreaRedPack newmodel = new LotteryActivitysAreaRedPack();
                    newmodel.ActivityID = activity.ID;
                    newmodel.AreaID = item.ID;
                    newmodel.InsertAndReturnIdentity();
                }
            }
            List<LotteryActivitysAreaRedPack> LAARPs = LotteryActivitysAreaRedPack.GetEntitysByActivityID(activity.ID);
            foreach (var item in LAARPs)
            {
                if (areas.Count(m => m.ID == item.AreaID) <= 0)
                {
                    LotteryActivitysAreaRedPack.DeleteByID(item.ID);
                }
            }
            LAARPs = LAARPs.FindAll(m => areas.Count(n => n.ID == m.AreaID) > 0);
            ViewData["LAARPs"] = LAARPs;
            ViewData["areas"] = areas;

            return View();
        }

        public ActionResult editRedCnt(int id,int redcnt) 
        {
           int cnt= LotteryActivitysAreaRedPack.EditRedCntByID(id,redcnt);

           return Content(cnt>0?"ok":"修改失败");
        }



        public ActionResult IndexOld(int activityID)
        {
            LotteryActivitys activity = LotteryActivitys.GetEntityByID(activityID);
            ViewData["activity"] = activity;


            List<SYSIntegralCodeArea> areas = SYSIntegralCodeArea.GetEntitysAll();

            foreach (var item in areas)
            {
                LotteryActivitysAreaRedPack old = LotteryActivitysAreaRedPack.GetEntityActArea(activity.ID, item.ID);
                if (old == null)
                {
                    LotteryActivitysAreaRedPack newmodel = new LotteryActivitysAreaRedPack();
                    newmodel.ActivityID = activity.ID;
                    newmodel.AreaID = item.ID;
                    newmodel.InsertAndReturnIdentity();
                }
            }


            List<LotteryActivitysAreaRedPack> LAARPs = LotteryActivitysAreaRedPack.GetEntitysByActivityID(activity.ID);


            foreach (var item in LAARPs)
            {
                if (areas.Count(m => m.ID == item.AreaID) <= 0)
                {
                    LotteryActivitysAreaRedPack.DeleteByID(item.ID);
                }
            }


            LAARPs = LAARPs.FindAll(m => areas.Count(n => n.ID == m.AreaID) > 0);
            ViewData["LAARPs"] = LAARPs;
            ViewData["areas"] = areas;

            return View();
        }

        public ActionResult toEdit(LotteryActivitysAreaRedPack model)
        {
            int rtn = model.EditByID();
            return Content(rtn > 0 ? "ok" : "修改失败");
        }



        public JsonResult getRedPackPrice(int id)
        {
            LotteryActivitysAreaRedPackPrice p = LotteryActivitysAreaRedPackPrice.GetEntityByID(id);
            return Json(p, JsonRequestBehavior.AllowGet);
        }

        public ActionResult todel(int id)
        {
            int rtn = LotteryActivitysAreaRedPackPrice.DeleteByID(id);
            return Content(rtn > 0 ? "ok" : "删除失败");
        }

    }
}
