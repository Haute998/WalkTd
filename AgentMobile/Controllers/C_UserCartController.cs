using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class C_UserCartController : BaseController
    {
        //
        // GET: /C_UserCart/

        public ActionResult Index()
        {
            List<C_UserCartVM> carts = C_UserCartVM.getCarts(CurrentUser.C_UserTypeID,CurrentUser.UserName);
            ViewData["carts"] = carts;

            C_UserMail nowMail = C_UserMail.GetIsNow(CurrentUser.UserName);
            if (nowMail == null) 
            {
                nowMail = new C_UserMail();
            }
            ViewData["nowMail"] = nowMail;
            return View();
        }
        /// <summary>
        /// 读取购物车
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadCart(BaseSearch condition)
        {
            PageJsonModel<C_UserCartVM> page = new PageJsonModel<C_UserCartVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(@" C_UserCart left join Product on C_UserCart.GoodsID=Product.ProductID
                     left join Product_Lever on Product.ProductID=Product_Lever.ProductID and Product_Lever.UserTypeID={0} ", CurrentUser.C_UserTypeID);
            page.strSelect = @" C_UserCart.*,Product.ProductNumber,Product.ProductName,
                              Product.ProductImg,Product.Price,Product.States,ISNULL(Product_Lever.Price,Product.Price) Price_agent ";
            page.strWhere = string.Format(" and C_UserName='{0}' ",CurrentUser.UserName);
            page.strOrder = "C_UserCart.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加到购物车 返回ok为成功
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="GetCnt"></param>
        /// <returns></returns>
        public ContentResult AddToCart(int ProductID, int GetCnt)
        {
            string rtn = C_UserCart.AddToCart(CurrentUser.UserName, ProductID, GetCnt);
            return Content(rtn);
        }

        /// <summary>
        /// 从购物车减少数量
        /// </summary>
        /// <param name="GoodsID"></param>
        /// <param name="Cnt"></param>
        /// <returns></returns>
        public ContentResult ReduceFromCart(int ProductID, int Cnt)
        {
            string rtn = C_UserCart.ReduceFromCart(CurrentUser.UserName, ProductID, Cnt);
            return Content(rtn);
        }

        /// <summary>
        /// 从购物车修改数量
        /// </summary>
        /// <param name="GoodsID"></param>
        /// <param name="Cnt"></param>
        /// <returns></returns>
        public ContentResult EditFromCart(int ProductID, int Cnt)
        {
            string rtn = C_UserCart.EditFromCart(CurrentUser.UserName, ProductID, Cnt);
            return Content(rtn);
        }
        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ContentResult CheckByID(int id,int type) 
        {
            bool isCheck=false;
            if(type==1)
            {
                isCheck=true;
            }
            int rtn= C_UserCart.CheckByID(id, isCheck);
            return Content(rtn>0?"ok":"网络错误");
        }

        public ContentResult del(int id)
        {
            int rtn = C_UserCart.DeleteByID(id);
            return Content(rtn > 0 ? "ok" : "网络错误");
        }


    }
}
