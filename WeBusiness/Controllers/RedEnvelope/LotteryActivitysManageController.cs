using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using DAL;
using System.IO;
using WeModels;



namespace WeBusiness.Controllers
{
    public class LotteryActivitysManageController : BaseController
    {
        //
        // GET: /LotteryActivitysManage/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(LotteryActivitysSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (Title like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(LotteryActivitysSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<LotteryActivitys> page = new PageJsonModel<LotteryActivitys>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " LotteryActivitys ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("查看", "Index")]
        public ActionResult getPrizes(int id)
        {
            List<LotteryPrizes> prizes = LotteryPrizes.GetPrizesByActivityID(id);
            return Json(prizes, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(LotteryActivitys para)
        {
            para.IsValid = true;
            if (string.IsNullOrWhiteSpace(para.Title))
            {
                return Content("fail|活动标题不能为空");
            }
            if (RepeatHelper.NoRepeat("LotteryActivitys", "Title", para.Title, para.ID) > 0)
            {
                return Content("fail|活动标题已存在");
            }
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok|" + rtn);
            }
            return Content("fail|添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id)
        {
            LotteryActivitys para = LotteryActivitys.GetEntityByID(id);
            List<LotteryPrizes> prizes = LotteryPrizes.GetPrizesByActivityID(id);
            ViewData["prizes"] = prizes;
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(LotteryActivitys para)
        {
            if (string.IsNullOrWhiteSpace(para.Title))
            {
                return Content("活动标题不能为空");
            }
            if (RepeatHelper.NoRepeat("LotteryActivitys", "Title", para.Title, para.ID) > 0)
            {
                return Content("活动标题已存在");
            }
            para.IsValid = true;
            int rtn = para.EditByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (LotteryActivitys.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }
       [B_MenuRightsTag("批量删除", "Index")]
        public ContentResult ToDels(int[] ids)
        {
            bool rtn = LotteryActivitys.ToDels(ids);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }









        //[B_MenuRightsTag("奖项设置", "Index")]
        public ActionResult PrizesConfig(int id)
        {
            LotteryActivitys activity = LotteryActivitys.GetEntityByID(id);
            ViewData["activity"] = activity;

            List<LotteryPrizes> Prizes = LotteryPrizes.GetEntitysByActivityID(id);
            ViewData["Prizes"] = Prizes;
            return View();
        }
        //[B_MenuRightsTag("奖项设置", "Index")]
        public ActionResult AddPrize(LotteryPrizes prize)
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

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        //[B_MenuRightsTag("奖项设置", "Index")]
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
                return Content("ok");
            }
            else
            {
                return Content("保存失败");
            }
        }
        //[B_MenuRightsTag("奖项删除", "Index")]
        public ActionResult delPrize(int id)
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

    }
}
