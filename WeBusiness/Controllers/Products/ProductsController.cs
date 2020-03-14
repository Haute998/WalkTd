using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class ProductsController : BaseController
    {
        //
        // GET: /Products/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(MProductSearch condition)
        {
            string where = string.Empty;
            //产品名称
            if (!string.IsNullOrWhiteSpace(condition.GoodsName))
            {
                where += " and Product.ProductName like '%" + Common.Filter(condition.GoodsName) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.States))
            {
                where += string.Format(" and Product.States='{0}' ", Common.Filter(condition.States));
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetProductPage(MProductSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<Product> page = new PageJsonModel<Product>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " Product ";
            page.strSelect = " * ";
            page.strWhere = where;

            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = " ProductID desc ";
            }

            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("添加产品", "Index")]
        public ActionResult ProductAdd()
        {
            return View();
        }
        [B_MenuRightsTag("添加产品", "Index")]
        public ActionResult ToProductAdd(Product product)
        {
            string msg = "";
            if (Product.Whetherthere("ProductNumber", product.ProductNumber, product.ProductID) > 0)
            {
                msg = "该产品编号已存在！";
                TempData["ToProductAdd_err"] = msg;
                return View("ProductAdd", product);
            }
            if (Product.Whetherthere("ProductName", product.ProductName, product.ProductID) > 0)
            {
                msg = "该产品名称已存在！";
                TempData["ToProductAdd_err"] = msg;
                return View("ProductAdd", product);
            }
            
            #region 产品图片处理
            string base64 = Request["ProductImgData"].ToString();
            if (base64.Length == 0)
            {
                return Content("您没有上传产品图片");
            }

            string filePath = "/images/Product/";
            IdWorker works=new IdWorker(1,0);
            string fileName = works.nextId().ToString();
            string ProductImgUrl = string.Empty;
            bool flag = false;

            try
            {
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(Server.MapPath("~/images/Product/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/Product/"));
                }

                if (!imghelper.Base64SaveImage(Server.MapPath("~") + filePath, base64, ref fileName))
                {
                    return Content("图片保存失败！");
                }
                else
                {
                    ProductImgUrl = filePath + fileName;
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            #endregion

            product.ProductImg = ProductImgUrl;
            if (Product.ProductAdd(product))
            {
                msg = "ok";
            }
            else
            {
                msg = "产品添加失败！";
            }
            TempData["ToProductAdd_err"] = msg;
            return View("ProductAdd", product);
        }
        [B_MenuRightsTag("删除产品", "Index")]
        public ContentResult DelProduct(int id)
        {
            int rtn = Product.DeleteByID(id);
            if (rtn > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败");
            }
        }
        [B_MenuRightsTag("批量删除产品", "Index")]
        public ContentResult DelProducts(int[] ids)
        {
            bool rtn = Product.DelProducts(ids);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }
        [B_MenuRightsTag("上架", "Index")]
        public ContentResult toPublish(string isPublish, int id)
        {
            bool rtn = Product.toPublish(isPublish, id);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }
        [B_MenuRightsTag("批量上架", "Index")]
        public ContentResult toPublishs(string isPublish, int[] ids)
        {
            bool rtn = Product.toPublishs(isPublish, ids);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }
        [B_MenuRightsTag("导出", "Index")]
        public FileResult ExportExcel(MProductSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select ProductNumber,ProductName,Price,States,AddTime from [Product] where 1=1 ");
            where.Append(MProductSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "产品编号", "产品名称", "零售价", "状态", "添加时间" };

            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "产品" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        [B_MenuRightsTag("修改产品", "Index")]
        public ActionResult ProductDetailEdit(int id)
        {
            Product product = Product.GetEntityByID(id);
            return View(product);
        }
        [B_MenuRightsTag("修改产品", "Index")]
        [ValidateInput(false)]
        public ActionResult ToProductDetailEdit(Product product)
        {
            Product oldproduct = Product.GetEntityByID(product.ProductID);
            string msg = "";
            if (Product.Whetherthere("ProductNumber", product.ProductNumber, product.ProductID) > 0)
            {
                msg = "该产品编号已存在！";
                TempData["ToProductDetailEdit_err"] = msg;
                return View("ProductDetailEdit", product);
            }
            if (Product.Whetherthere("ProductName", product.ProductName, product.ProductID) > 0)
            {
                msg = "该产品名称已存在！";
                TempData["ToProductDetailEdit_err"] = msg;
                return View("ProductDetailEdit", product);
            }
            oldproduct.ProductNumber = product.ProductNumber;
            oldproduct.ProductName = product.ProductName;
            oldproduct.bianma = product.bianma;
            oldproduct.kw = product.kw;
            //oldproduct.Price = product.Price;
            oldproduct.States = product.States;
            var file = Request.Files[0];
            string path = Request.MapPath("~/");
            string ext = Path.GetExtension(file.FileName);//获得文件扩展名
            if (!Directory.Exists(Server.MapPath("~/images/Product/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/images/Product/"));
            }
            if (file != null && file.ContentLength > 0)
            {
                if (file.ContentLength > 5242880)
                {
                    return Content("上传图片不能超过5MB！");
                }
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
                {
                    return Content("上传图片格式不对！");
                }
                file.SaveAs(Server.MapPath("~/images/Product/P_" + product.ProductID + ext));
                oldproduct.ProductImg = "/images/Product/P_" + product.ProductID + ext + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            if (oldproduct.UpdateByID() > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "产品修改失败！";
            }
            TempData["ToProductDetailEdit_err"] = msg;
            return View("ProductDetailEdit", product);
        }

        [B_MenuRightsTag("修改产品", "Index")]
        public ContentResult DetailTempSave(int id, string detail)
        {
            try
            {

                Product goods = Product.GetEntityByID(id);
                if (goods == null)
                {
                    return Content("产品不存在");
                }
                int rtn = Product.DetailTempSave(id, detail);
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

        [B_MenuRightsTag("修改产品", "Index")]
        public ActionResult ImgTxtEdit(int id)
        {
            Product goods = Product.GetEntityByID(id);
            return View(goods);
        }

        [B_MenuRightsTag("修改产品", "Index")]
        public ContentResult ProDetailSave(Product goods)
        {
            Product oldGoods = Product.GetEntityByID(goods.ProductID);

            oldGoods.ImgTxtContent = goods.ImgTxtContent;
            oldGoods.ImgTxtTmp = "";
            int rtn = oldGoods.EditImgTxtByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存失败");
        }
        [B_MenuRightsTag("修改产品", "Index")]
        public ContentResult GetDetailTemp(int id)
        {
            Product goods = Product.GetEntityByID(id);
            if (goods != null)
            {
                return Content(goods.ImgTxtTmp);
            }
            return Content("");
        }

        [B_MenuRightsTag("修改产品", "Index")]
        public ActionResult DetailImgsUp(int id)
        {
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/images/Product/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/Product/"));
                }
                if (file.ContentLength == 0 || file == null)
                {
                    return Content("上传的图片没有内容");
                }
                if (file.ContentLength > 5242880)
                {
                    return Content("上传图片不能超过5MB！");
                }
                Product goods = Product.GetEntityByID(id);

                ProDetailImg imgs = new ProDetailImg();
                imgs.ImgUrl = "/images/Product/Detail_" + goods.ProductID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
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
    }
}
