using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models.ProductModel;

namespace WeBusiness.Controllers
{
    public class Product_LeverController : BaseController
    {
        //
        // GET: /Product_Lever/
        [B_MenuRightsTag("查看")]
        public ActionResult Index(int productID)
        {
            if (productID <= 0)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "产品有误" });
            }
            Product product = Product.GetEntityByID(productID);
            return View(product);
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult Index2(int productID)
        {
            if (productID <= 0)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "产品有误" });
            }
            Product product = Product.GetEntityByID(productID);
            return View(product);
        }


        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetLevelPrices(C_UserTypeSearch condition)
        {
            string where = C_UserTypeSearch.StrWhere(condition);

            PageJsonModel<LevelPriceVM> page = new PageJsonModel<LevelPriceVM>();
            page.pageIndex = condition.pageIndex;
            page.strForm = string.Format(@" C_UserType left join Product_Lever on C_UserType.Lever=Product_Lever.UserTypeID and Product_Lever.ProductID={0} 
left join Product on Product.ProductID={0} ", condition.ProductID);
            page.strSelect = " C_UserType.*,ISNULL(Product_Lever.ID,0) ProL_ID,ISNULL(Product_Lever.Price,Product.Price) price ";
            page.pageSize = condition.pageSize;
            page.strWhere = where;
            page.strOrder = " C_UserType.Lever ";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult EditLevelPrice(int productID, int UserTypeID, decimal Price)
        {
            if (productID <= 0 || UserTypeID<=0) 
            {
                return Content("参数有误");
            }
            Product_Lever oldProduct_Lever = Product_Lever.GetByProductIDAndUserTypeID(productID, UserTypeID);
            int rtn = 0;
            if (oldProduct_Lever == null)
            {
                Product_Lever lever = new Product_Lever();
                lever.Price = Price;
                lever.ProductID = productID;
                lever.UserTypeID = UserTypeID;
                rtn = lever.InsertAndReturnIdentity();

            }
            else
            {
                rtn = Product_Lever.EditPriceByProductIDAndUserTypeID(productID, UserTypeID, Price);
            }
            return Content(rtn > 0 ? "ok" : "修改失败");
        }
    }
}
