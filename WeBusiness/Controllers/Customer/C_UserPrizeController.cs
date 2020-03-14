using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class C_UserPrizeController : BaseController
    {
        //
        // GET: /C_UserPrize/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetNoPrizePage(C_UserPrize condition)
        {

            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( AntiCode like '%{0}%' or Phone like '%{0}%' or Name like '%{0}%')", condition.keyword);
            }
            where += " and State='未审核' and Prize!='谢谢惠顾'";
            PageJsonModel<C_UserPrize> page = new PageJsonModel<C_UserPrize>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"C_UserPrize  ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("审核", "Index")]
        public ActionResult GetVerify(int ID)
        {
            C_UserPrize getprize = C_UserPrize.GetEntityByID(ID);
            getprize.State = "已审核";
            int rtn = getprize.UpdateByID();
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult PrizeIndex()
        {
            return View();
        }

        public ActionResult GetPrizePage(C_UserPrize condition)
        {

            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( AntiCode like '%{0}%' or Phone like '%{0}%' or Name like '%{0}%')", condition.keyword);
            }
            where += " and State='已审核' and Prize!='谢谢惠顾'";
            PageJsonModel<C_UserPrize> page = new PageJsonModel<C_UserPrize>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"C_UserPrize  ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult AllIndex()
        {
            return View();
        }

        public ActionResult GetPage(C_UserPrize condition)
        {

            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ( AntiCode like '%{0}%' or Phone like '%{0}%' or Name like '%{0}%')", condition.keyword);
            }
            PageJsonModel<C_UserPrize> page = new PageJsonModel<C_UserPrize>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"C_UserPrize  ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult PrizeTypeIndex()
        {
            List<Prize> prize = Prize.GetEntitysAll();
            return View(prize);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult PrizeAddIndex()
        {
            return View();
        }
        [B_MenuRightsTag("删除", "PrizeTypeIndex")]
        public ActionResult GetDelPrize(int ID)
        {
            int rtn = Prize.DeleteByID(ID);
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }

        [B_MenuRightsTag("添加产品", "PrizeAddIndex")]
        public ActionResult GetPrizeAdd(Prize condition)
        {
            if (string.IsNullOrWhiteSpace(Common.Filter(condition.Lever)))
            {
                return Content("级别不能为空！");
            }
            if (string.IsNullOrWhiteSpace(Common.Filter(condition.Name)))
            {
                return Content("名称不能为空！");
            }
            if (Prize.GetLeverRepeat(Common.Filter(condition.Lever)) > 0)
            {
                return Content("级别不能重复！");
            }
            int rtn = condition.InsertAndReturnIdentity();
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult PrizeEditIndex(int ID)
        {
            Prize Prize = Prize.GetEntityByID(ID);
            return View(Prize);
        }
        [B_MenuRightsTag("修改产品", "PrizeEditIndex")]
        public ActionResult GetPrizeEdit(Prize condition)
        {
            if (string.IsNullOrWhiteSpace(Common.Filter(condition.Lever)))
            {
                return Content("级别不能为空！");
            }
            if (string.IsNullOrWhiteSpace(Common.Filter(condition.Name)))
            {
                return Content("名称不能为空！");
            }
            if (Prize.GetEditLeverRepeat(Common.Filter(condition.Lever), condition.ID) > 0)
            {
                return Content("级别不能重复！");
            }
            int rtn = condition.UpdateByID();
            string msg = rtn > 0 ? "ok" : "网络出错了！！";
            return Content(msg);
        }
    }
}
