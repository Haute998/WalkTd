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
    public class RedEnvelopeController : BaseController
    {
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            LotteryActivitys activity = LotteryActivitys.GetEntityByID(1);
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

            ViewData["areas"] = areas;

            //List<LotteryActivitysAreaRedPack> LAARPs = LotteryActivitysAreaRedPack.GetEntitysByActivityID(activity.ID);
            //foreach (var item in LAARPs)
            //{
            //    if (areas.Count(m => m.ID == item.AreaID) <= 0)
            //    {
            //        LotteryActivitysAreaRedPack.DeleteByID(item.ID);
            //    }
            //}
            //LAARPs = LAARPs.FindAll(m => areas.Count(n => n.ID == m.AreaID) > 0);
            //ViewData["LAARPs"] = LAARPs;
            

            return View();
        }

        public ActionResult GetRedPackList()
        {
            List<LotteryActivitysRedPack> RedPackList = LotteryActivitysRedPack.GetEntitysByActivityID(1);
            return Json(RedPackList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRadParam()
        {
            List<LotteryActivitysRedPack> RedPackList = LotteryActivitysRedPack.GetAllRadParam();
            return Json(RedPackList,JsonRequestBehavior.AllowGet);
        }

        public ContentResult ToAddParam(LotteryActivitysRedPack para)
        {
            para.ActivityID = 1;
            if (para.MaxPrice <= 0)
            {
                return Content("最大金额不能小于0");
            }

            if (para.MaxPrice < para.MinPrice)
            {
                return Content("最大金额不能小于最小金额");
            }

            if (RepeatHelper.NoRepeatThreeAnd("LotteryActivitysRedPack", "MaxPrice", para.MaxPrice.ToString(), "MinPrice", para.MinPrice.ToString(), "ActivityID", para.ActivityID.ToString(), para.ID) > 0)
            {
                return Content("该设置已存在");
            }
           
            para.InsertAndReturnIdentity();

            return Content("ok");
        }

        public ContentResult ToEditParam(LotteryActivitysRedPack para)
        {
            para.ActivityID = 1;
            if (para.MaxPrice <= 0)
            {
                return Content("最大金额不能小于0");
            }

            if (para.MaxPrice < para.MinPrice)
            {
                return Content("最大金额不能小于最小金额");
            }

            if (RepeatHelper.NoRepeatThreeAnd("LotteryActivitysRedPack", "MaxPrice", para.MaxPrice.ToString(), "MinPrice", para.MinPrice.ToString(), "ActivityID", para.ActivityID.ToString(), para.ID) > 0)
            {
                return Content("该设置已存在");
            }

            para.UpdateByID();

            return Content("ok");
        }

        public ContentResult ToDelParam(int ID)
        {
            if (LotteryActivitysRedPack.DeleteByID(ID) > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败！");
            }
        }
    }
}
