using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Mvc;
using DAL;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class LuckyDrawController : BaseController
    {
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnpaidPrize()
        {
            return View();
        }

        public ActionResult AwardedPrize()
        {
            return View();
        }

        public ActionResult DrawPrizeCount()
        {
            return View();
        }

        public ActionResult PrizeSettings()
        {
            LotteryActivitys activity = LotteryActivitys.GetEntityByID(2);
            ViewData["activity"] = activity;
            ViewData["IsNot"] = LotteryPrizes.IsNotCanActive();

            List<LotteryPrizes> Prizes = LotteryPrizes.GetEntitysByActivityID(2);
            ViewData["Prizes"] = Prizes;
            MainShow UserScale = Scale.GetScaleCount();
            ViewData["BarCodeQty"] = UserScale.CanLotteryCount;
            return View();
        }

        public ContentResult AddPrize(LotteryPrizes prize)
        {
            int cnt = LotteryPrizes.GetPrizesCnt(prize.ActivityID);
            if (cnt >= 10)
            {
                return Content("fail|奖项不能超过10个啊");
            }

            prize.IsFinish = false;
            int id = prize.InsertAndReturnIdentity();
            if (id > 0)
            {
                return Content("ok|" + id);
            }
            else
            {
                return Content("fail|添加奖项失败");
            }
        }

        public ContentResult AddNotPrize(LotteryPrizes prize)
        {
            int cnt = LotteryPrizes.GetPrizesCnt(prize.ActivityID);
            if (cnt >= 10)
            {
                return Content("fail|奖项不能超过10个啊");
            }

            prize.IsFinish = false;
            prize.IsNot = true;
            int id = prize.InsertAndReturnIdentity();
            if (id > 0)
            {
                return Content("ok|" + id);
            }
            else
            {
                return Content("fail|添加奖项失败");
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        public ActionResult SavePrizes(LotteryPrizes prize)
        {
            prize.IsFinish = true;
            HttpPostedFileBase upfile = Request.Files["goodsimg"];

            LotteryPrizes oldPrizes = LotteryPrizes.GetEntityByID(prize.ID);

            if (upfile != null)
            {
                if (upfile.ContentLength >= (5242880))
                {
                    return Content("请上传5M以内的图片！");
                }
                string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
                string saveUrl = "/images/LotteryPrizes/" + "prize_" + prize.ID + ext;

                try
                {
                    if (!Directory.Exists(Server.MapPath("~/images/LotteryPrizes/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/LotteryPrizes/"));
                    }
                    upfile.SaveAs(Server.MapPath(saveUrl));
                    prize.PrizeImgUrl = saveUrl + "?" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                }
                catch (Exception ex)
                {
                    DAL.Log.Instance.Write(ex.ToString(), "LotteryActivitysManage_SavePrizes_error");
                    return Content("保存失败");
                }
            }
            else
            {
                prize.PrizeImgUrl = oldPrizes.PrizeImgUrl;
            }

            prize.WinningRate = prize.WinningRate / 100;

            int rtn = prize.EditByID();
            if (rtn > 0)
            {
                return Content("保存成功");
            }
            else
            {
                return Content("保存失败");
            }
        }

        public ContentResult delPrize(int id)
        {
            LotteryPrizes oldPrizes = LotteryPrizes.GetEntityByID(id);

            string delFile = "";
            try
            {
                if (oldPrizes.PrizeImgUrl.Contains("?"))
                {
                    oldPrizes.PrizeImgUrl = oldPrizes.PrizeImgUrl.SubStringSafe(0, oldPrizes.PrizeImgUrl.IndexOf("?"));
                }

                delFile = Server.MapPath("~") + oldPrizes.PrizeImgUrl;
                System.IO.File.Delete(delFile);

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write("删除文件失败：" + delFile + ex.ToString(), "LotteryActivitysManage_delPrize_error");
            }
            var rtn = LotteryPrizes.DeleteByID(id);
            return rtn > 0 ? Content("ok") : Content("删除失败");
        }

        public ActionResult GetRecords(LotteryRecordSearch condition)
        {
            string where = LotteryRecordSearch.StrWhere(condition);

            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.pageIndex = condition.pageIndex;
            page.strForm = " LotteryRecord ";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = where;
            page.strOrder = " ID Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVerify(int id)
        {
            LotteryRecord r = LotteryRecord.GetEntityByID(id);
            if (r != null)
            {
                r.States = "已发放";
                r.ID = r.UpdateByID();
            }
            else
            {
                return Content("数据错误！！！");
            }

            return Content("ok");
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel(LotteryRecordSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select [Dat],[IntegralCode],[ActivityTitle],[PrizeLevel],[PrizeName],Name,Phone,RecAddress,[States] from [LotteryRecord] where 1=1 ");
            where.Append(LotteryRecordSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "中奖时间", "防伪码", "活动名称", "奖品等级", "奖品名称", "中奖人姓名", "电话", "地址", "发放状态" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "中奖纪录" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        public ActionResult GetStatistics(LotteryRecordSearch condition)
        {
            string where = LotteryRecordSearch.StrWhere(condition);

            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.pageIndex = condition.pageIndex;
            page.strForm = " (select  ActivityTitle,PrizeID,PrizeLevel,PrizeImgUrl,PrizeName,COUNT(*) num from LotteryRecord where 1=1 " + where + " group by    ActivityTitle,PrizeID,PrizeLevel,PrizeImgUrl,PrizeName ) Lotteryshow ";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strOrder = " PrizeID Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportStatistics(LotteryRecordSearch condition)
        {
            string where = LotteryRecordSearch.StrWhere(condition);

            DataTable dt = ExportWay.ExcelDataTable("select  ActivityTitle,PrizeLevel,PrizeImgUrl,PrizeName,COUNT(*) num from LotteryRecord where 1=1 " + where + " group by    ActivityTitle,PrizeID,PrizeLevel,PrizeImgUrl,PrizeName ");
            string[] list = { "活动名称", "奖品等级", "奖品名称", "中奖数量" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "中奖纪录统计" + DateTime.Now.ToShortTimeString() + ".xls");
        }
    }
}
