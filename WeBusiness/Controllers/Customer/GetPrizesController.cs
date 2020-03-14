using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class GetPrizesController : BaseController
    {
        //
        // GET: /GetPrizes/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetPage(BaseInSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( Code like '%{0}%' or State like '%{0}%' or PrizeName like '%{0}%' or PrizeID like '%{0}%')", condition.keyword);
            }
            PageJsonModel<GetPrize> page = new PageJsonModel<GetPrize>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"GetPrize  ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("添加", "Index")]
        public ActionResult AddIndex()
        {
            return View();
        }

        [B_MenuRightsTag("添加", "AddIndex")]
        public ActionResult GetAddPrize(GetPrize condition)
        {
            if (string.IsNullOrWhiteSpace(condition.Code))
            {
                return Content("防伪码不能为空！！");
            }
            if (!Scale.GetAntiFake(condition.Code))
            {
                return Content("没有此防伪码！！");
            }
            if (GetPrize.GetCodeRepeat(condition.Code) > 0)
            {
                return Content("防伪码已设置奖品");
            }
            Prize prize = Prize.GetEntitysAll().Single(m => m.Lever == condition.PrizeID);
            condition.PrizeName = prize.Name;
            condition.DatCreate = DateTime.Now;
            condition.State = "未抽到";
            int rtn = condition.InsertAndReturnIdentity();
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }
        public ActionResult GetDel(int ID)
        {

            int rtn = GetPrize.DeleteByID(ID);
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }
      

    }
}
