using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;
using WeModels.Models.OrderModel;

namespace AgentMobile.Controllers
{
    public class OrderController : BaseController
    {
        //
        // GET: /Order/

        public ActionResult Manage()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            ViewData["OrderNoVerity"] = Order.GetC_UserCircles(CurrentUser.UserName);
            return View();
        }
        /// <summary>
        /// 我要订货
        /// </summary>
        /// <returns></returns>
        public ActionResult ToShop()
        {
            C_UserCartCalModel cartCal = C_UserCartCalModel.GetList(CurrentUser.UserName, CurrentUser.C_UserTypeID);
            if (cartCal == null)
            {
                cartCal = new C_UserCartCalModel();
            }
            ViewData["cartCal"] = cartCal;
            return View();
        }
        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadProducts(BaseSearch condition)
        {
            PageJsonModel<ProductPriceVM> page = new PageJsonModel<ProductPriceVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" Product left join Product_Lever on Product.ProductID=Product_Lever.ProductID and Product_Lever.UserTypeID={0} ", CurrentUser.C_UserTypeID);
            page.strSelect = " Product.*,ISNULL(Product_Lever.Price,Product.Price) Price_agent ";
            page.strWhere = "";
            page.strOrder = "Product.ProductID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 产品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProductsDetail(int id)
        {
            Product goods = Product.GetEntityByID(id);
            return View(goods);
        }


        public ActionResult MyOrders(string AuditState = "", string OrderState = "")
        {
            ViewData["AuditState"] = AuditState;
            ViewData["OrderState"] = OrderState;
            return View();
        }
        public ActionResult LoadMyOrders(OrderSearch condition)
        {
            PageJsonModel<Order> page = new PageJsonModel<Order>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order] ";
            page.strSelect = " *  ";
            page.strWhere = string.Format(" and UserName='{0}' ", CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.OrderState))
            {
                condition.OrderState = Common.Filter(condition.OrderState);
                page.strWhere += string.Format(" and OrderState='{0}' ", condition.OrderState);
            }
            if (!string.IsNullOrWhiteSpace(condition.AuditState))
            {
                condition.AuditState = Common.Filter(condition.AuditState);
                page.strWhere += string.Format(" and AuditState='{0}' ", condition.AuditState);
            }

            //姓名
            if (string.IsNullOrWhiteSpace(condition.Name) == false)
            {
                page.strWhere += " and C_User.Name = '" + Common.Filter(condition.Name) + "' ";
            }

            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                page.strWhere += " and [Order].OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }

            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                page.strWhere += string.Format(" and [Order].DatCreate >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                page.strWhere += string.Format(" and [Order].DatCreate <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }



            page.strWhere += " and OrderState!='已删除' ";
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// 订单删除
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public ContentResult orderdel(string orderNo)
        {
            int rtn = Order.DelOrder(orderNo);
            if (rtn > 0)
            {
                OrderLog.LogAdd(orderNo, "删除", "客户【" + CurrentUser.UserName + "】删除了订单", "Mobile");
            }
            return Content("ok");
        }

        /// <summary>

        /// <summary>
        /// 我的下级订单
        /// </summary>
        /// <param name="AuditState"></param>
        /// <param name="OrderState"></param>
        /// <returns></returns>
        public ActionResult MyJuniorOrders(string AuditState = "", string OrderState = "")
        {
            ViewData["AuditState"] = AuditState;
            ViewData["OrderState"] = OrderState;
            return View();
        }
        /// <summary>
        /// 我的下级订单详情
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
     
        /// <summary>
        /// 读取我的下级订单
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadMyJuniorOrders(OrderSearch condition)
        {
            PageJsonModel<OrderShowVM> page = new PageJsonModel<OrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order] left join C_User on [Order].UserName=C_User.UserName ";
            page.strSelect = " [Order].*,C_User.Name C_User_Name  ";
            page.strWhere = string.Format(" and [Order].ParentUser='{0}' ", CurrentUser.UserName);

            if (!string.IsNullOrWhiteSpace(condition.OrderState))
            {
                condition.OrderState = Common.Filter(condition.OrderState);
                page.strWhere += string.Format(" and [Order].OrderState='{0}' ", condition.OrderState);
            }
            if (!string.IsNullOrWhiteSpace(condition.AuditState))
            {
                condition.AuditState = Common.Filter(condition.AuditState);
                page.strWhere += string.Format(" and [Order].AuditState='{0}' ", condition.AuditState);
            }
            //姓名
            if (string.IsNullOrWhiteSpace(condition.Name) == false)
            {
                page.strWhere += " and C_User.Name = '" + Common.Filter(condition.Name) + "' ";
            }

            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                page.strWhere += " and [Order].OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }

            //订单创建时间
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                page.strWhere += string.Format(" and [Order].DatCreate >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                page.strWhere += string.Format(" and [Order].DatCreate <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }

            page.strWhere += " and [Order].OrderState!='已删除' ";
            page.strOrder = "[Order].ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 生成订单
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>


        /// <summary>
        /// 订单取消
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>

    }
}
