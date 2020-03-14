using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using DAL;

namespace WeBusiness.Controllers
{
    public class jf_GoodsController : BaseController
    {
        //
        // GET: /jf_Goods/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

        private string StrWhere(jf_GoodsSearch condition)
        {
            string where = string.Empty;
            //产品名称
            if (!string.IsNullOrWhiteSpace(condition.GoodsName))
            {
                where += " and jf_Goods.GoodsName like '%" + Common.Filter(condition.GoodsName) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.PublishStat))
            {
                where += string.Format(" and jf_Goods.PublishStat='{0}' ", Common.Filter(condition.PublishStat));
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetGoodsPage(jf_GoodsSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<jf_Goods> page = new PageJsonModel<jf_Goods>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " jf_Goods ";
            page.strSelect = " jf_Goods.* ";
            page.strWhere = where;
            page.strOrder = "jf_Goods.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult AddGoods()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult ToAddGoods(jf_Goods goods)
        {
            string msg = "";
            if (RepeatHelper.NoRepeat("jf_Goods", "GoodsName", goods.GoodsName, goods.ID) > 0)
            {
                msg = "该礼品名称已存在！";
                goods.error = msg;
                return View("AddGoods", goods);
            }
            var file = Request.Files[0];
            string path = Request.MapPath("~/");
            string ext = Path.GetExtension(file.FileName);//获得文件扩展名
            if (!Directory.Exists(Server.MapPath("~/images/jf_Goods/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/images/jf_Goods/"));
            }
            if (file.ContentLength == 0 || file == null)
            {
                msg = "上传的图片没有内容！";
                goods.error = msg;
                return View("AddGoods", goods);
            }
            if (file.ContentLength > 5242880)
            {
                msg = "上传图片不能超过5MB！";
                goods.error = msg;
                return View("AddGoods", goods);
            }

            goods.PublishStat = "未上架";
            int rtn = goods.AddGoods(ext, CurrentUser.UserName);
            if (rtn > 0)
            {
                file.SaveAs(path + goods.Main_img);
                return RedirectToAction("GoodsDetailEdit", "jf_Goods", new { id = goods.ID });
            }
            msg = "礼品添加失败！";
            goods.error = msg;
            return View("AddGoods", goods);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult EditGoods(int id)
        {
            jf_Goods goods = jf_Goods.GetEntityByID(id);
            return View(goods);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult ToEditGoods(jf_Goods goods)
        {
            jf_Goods OldGoods = jf_Goods.GetEntityByID(goods.ID);
            string oldImgUrl = OldGoods.Main_img;//原来的图片
            if (oldImgUrl.Contains("?"))
            {
                oldImgUrl = oldImgUrl.SubStringSafe(0, oldImgUrl.IndexOf("?"));
            }
            string msg = "";
            if (RepeatHelper.NoRepeat("jf_Goods", "GoodsName", goods.GoodsName, goods.ID) > 0)
            {
                msg = "该礼品已存在！";
                goods.error = msg;
                return View("EditGoods", goods);
            }
            OldGoods.GoodsName = goods.GoodsName;
            var file = Request.Files[0];
            if (file.ContentLength > 0 && file != null)
            {
                if (file.ContentLength > 5242880)
                {
                    msg = "上传图片不能超过5MB！";
                    goods.error = msg;
                    return View("EditGoods", goods);
                }
                if (!Directory.Exists(Server.MapPath("~/images/jf_Goods/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/jf_Goods/"));
                }
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                OldGoods.Main_img = "/images/jf_Goods/G_" + OldGoods.ID + ext;
                string saveName = OldGoods.Main_img;//实际保存文件名

                try
                {
                    string delFile = Server.MapPath("~") + oldImgUrl;
                    System.IO.File.Delete(delFile);
                }
                catch (Exception ex) { }


                file.SaveAs(path + saveName);
            }
            OldGoods.Main_img = OldGoods.Main_img + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
            OldGoods.SaleIntegral = goods.SaleIntegral;
            OldGoods.Quantity = goods.Quantity;
            if (OldGoods.EditByID() > 0)
            {
                goods.error = "ok";
            }
            else
            {
                goods.error = "修改礼品失败！";
            }
            return View("EditGoods", goods);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult GoodsDetailEdit(int id)
        {
            jf_Goods goods = jf_Goods.GetEntityByID(id);
            return View(goods);
        }
      
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult DetailImgsUp(int id)
        {
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/images/jf_Goods/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/jf_Goods/"));
                }
                if (file.ContentLength == 0 || file == null)
                {
                    return Content("上传的图片没有内容");
                }
                if (file.ContentLength > 5242880)
                {
                    return Content("上传图片不能超过5MB！");
                }
                jf_Goods goods = jf_Goods.GetEntityByID(id);

                jf_GoodsDetailImg imgs = new jf_GoodsDetailImg();
                imgs.ImgUrl = "/images/jf_Goods/Detail_" + goods.ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                file.SaveAs(path + imgs.ImgUrl);
                imgs.UserName = CurrentUser.UserName;
                imgs.GoodsID = id;
                imgs.Dat = DateTime.Now;
                imgs.ImgUrl = WeConfig.b_domain + imgs.ImgUrl;
                imgs.InsertAndReturnIdentity();
                return Content(imgs.ImgUrl);
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "DetailImgsUp_error");
                return Content("error|服务器繁忙");
            }

        }

        [B_MenuRightsTag("修改", "Index")]
        public ContentResult DetailTempSave(int id, string detail)
        {
            try
            {

                jf_Goods goods = jf_Goods.GetEntityByID(id);
                if (goods == null)
                {
                    return Content("商品不存在");
                }
                int rtn = jf_Goods.DetailTempSave(id, detail);
                if (rtn > 0)
                {
                    return Content("ok");

                }
                return Content("网络异常");
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "goodsDetailEdit_error");
                return Content("网络异常");
            }
        }

        [B_MenuRightsTag("修改", "Index")]
        public ContentResult GoodsDetailSave(jf_Goods goods)
        {
            jf_Goods oldGoods = jf_Goods.GetEntityByID(goods.ID);

            oldGoods.Detail = goods.Detail;
            oldGoods.DetailTemp = "";
            oldGoods.PublishStat = goods.PublishStat;
            int rtn = oldGoods.EditByID();
            if (rtn > 0)
            {
                return Content("保存成功");
            }
            return Content("保存失败");
        }
        [B_MenuRightsTag("修改产品", "Index")]
        public ContentResult GetDetailTemp(int id)
        {
            jf_Goods goods = jf_Goods.GetEntityByID(id);
            if (goods != null)
            {
                return Content(goods.DetailTemp);
            }
            return Content("");
        }

        [B_MenuRightsTag("删除产品", "Index")]
        public ContentResult DelMGoods(int id)
        {
            int rtn = jf_Goods.DeleteByID(id);
            if (rtn > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败");
            }
        }

        [B_MenuRightsTag("批量上架", "Index")]
        public ContentResult toPublishs(int isPublish, int[] ids)
        {
            bool rtn = jf_Goods.toPublishs(isPublish, ids);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }


    }
}
