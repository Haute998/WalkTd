using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using WeModels.Models.OrderModel;
using DAL;

namespace WeBusiness.Controllers
{
    public class OrderManageController : BaseController
    {
        //
        // GET: /OrderManage/
        [B_MenuRightsTag("查看")]
        public ActionResult Orders()
        {
            return View();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel(OrderSearch condition)
        {
            StringBuilder where = new StringBuilder();
            string[] field = { "OrderNo", "OrderName", "SumPrice", "OrderMan", "OrderMobile", "Remark" };

            string[] list = { "订单编号", "订单名称", "订单金额", "收货人", "电话", "备注" };

            string url = "/OrderManage/Orders";

            exportHelper exhelper = exportHelper.getexport(field, list, url, CurrentUser.UserName);

            where.Append(string.Format("select {0} from [Order] where 1=1 ", string.Join(",", exhelper.field)));
            list = exhelper.list;
            where.Append(OrderSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());








            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "订单" + DateTime.Now.ToShortTimeString() + ".xls");
        }

        [B_MenuRightsTag("查看", "Orders")]
        public ActionResult GetOrders(OrderSearch condition)
        {
            string where = OrderSearch.StrWhere(condition);

            PageJsonModel<OrderShowVM> page = new PageJsonModel<OrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.strForm = @" [Order] left join C_User on [Order].UserName=C_User.UserName ";
            page.strSelect = " [Order].*,C_User.Name C_User_Name,(select C_UserType.Name from C_UserType where  C_UserType.Lever= C_User.C_UserTypeID) C_UserTypeName ";
            page.pageSize = condition.pageSize;
            page.strWhere = string.Format(" and [Order].ParentUser='' ") + where;

            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = " [Order].ID Desc";
            }

            page.LoadList();

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("订单详情", "Orders")]
        //public ActionResult OrderDetail(string orderNo)
        //{
        //    if (string.IsNullOrWhiteSpace(orderNo))
        //    {
        //        return View(ErrorPage.ViewName, new ErrorPage { Message = "订单编号有误" });
        //    }
        //    OrderVM order = new OrderVM();
        //    order.LoadOrder(orderNo);
        //    return View(order);
        //}
    
        

        //[B_MenuRightsTag("查看订单日志", "Orders")]
        public ActionResult GetOrderLogPage(int pageIndex, string orderNo)
        {
            PageJsonModel<OrderLog> page = new PageJsonModel<OrderLog>();
            page.pageIndex = pageIndex;
            page.pageSize = 100;
            page.strForm = " OrderLog ";
            page.strSelect = " * ";
            page.strWhere = " and OrderNo='" + Common.Filter(orderNo) + "'";
            page.strOrder = "Dat desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 发货待审核页面
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult NoAuditOrders()
        {
            return View();
        }
        /// <summary>
        /// 获取待审核订单
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "NoAuditOrders")]
        public ActionResult GetNoAuditOrders(OrderSearch condition)
        {
            string where = string.Empty;
            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and [Order].OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and ([Order].OrderNo like '%{0}%' or [Order].OrderState like '%{0}%' or [Order].OrderName like '%{0}%') ", condition.keyword);
            }
            where += string.Format(" and [Order].OrderState='待审核' and [Order].AuditState='未审核'");

            PageJsonModel<OrderShowVM> page = new PageJsonModel<OrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order] left join C_User on [Order].UserName=C_User.UserName ";
            page.strSelect = " [Order].*,C_User.Name C_User_Name,(select Name from C_UserType where  C_UserType.Lever= C_User.C_UserTypeID) C_UserTypeName ";
            page.strWhere = string.Format(" and [Order].ParentUser='' ") + where;
           


            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = " [Order].DatCreate Asc ";
            }

            page.LoadList();

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("审核", "NoAuditOrders")]
        //public ActionResult toAudit(string orderno)
        //{
        //    string rtn = Order.AuditByOrderNo(orderno, CurrentUser.UserName);
        //    return Content(rtn);
        //}

        /// <summary>
        /// 待发货订单页面
        /// </summary>
        /// <returns></returns>
        //[B_MenuRightsTag("查看")]
        public ActionResult NoSendOrders()
        {
            return View();
        }
        /// <summary>
        /// 获取待发货订单
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "NoSendOrders")]
        public ActionResult GetNoSendOrders(OrderSearch condition)
        {
            string where = string.Empty;
            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and [Order].OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and ([Order].OrderNo like '%{0}%' or [Order].OrderName like '%{0}%' or [Order].OrderMan like '%{0}%' or [Order].OrderMobile like '%{0}%' or [Order].[Address] like '%{0}%') ", condition.keyword);
            }
            where += string.Format(" and [Order].OrderState='待发货' and [Order].AuditState='已审核'");

            PageJsonModel<OrderShowVM> page = new PageJsonModel<OrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order] left join C_User on [Order].UserName=C_User.UserName ";
            page.strSelect = @" [Order].*,C_User.Name C_User_Name,(select Name from C_UserType where  C_UserType.Lever= C_User.C_UserTypeID) C_UserTypeName ";
            page.strWhere = string.Format(" and [Order].ParentUser='' ") + where;
            page.strOrder = " DatAudit Asc";
            page.LoadList();

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 已发货订单页面
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
        public ActionResult SentOrders()
        {
            return View();
        }
        /// <summary>
        /// 获取已发货订单订单
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [B_MenuRightsTag("查看", "NoSendOrders")]
        public ActionResult GetSentOrders(OrderSearch condition)
        {
            string where = string.Empty;
            //订单编号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(@" and ([Order].OrderNo like '%{0}%' or [Order].OrderName like '%{0}%' 
                                           or [Order].OrderMan like '%{0}%' or [Order].OrderMobile like '%{0}%' 
                                           or [Order].[Address] like '%{0}%' or OrderPost.[PostName] like '%{0}%' or OrderPost.[PostNo] like '%{0}%') ", condition.keyword);
            }

            PageJsonModel<OrderShowVM> page = new PageJsonModel<OrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " [Order] left join C_User on [Order].UserName=C_User.UserName left join OrderPost on OrderPost.OrderNo=[Order].OrderNo ";
            page.strSelect = @" [Order].*,C_User.Name C_User_Name,
                ISNULL(OrderPost.PostName,'') PostName,ISNULL(OrderPost.PostNo,'') PostNo,ISNULL(OrderPost.Dat,0) PostDat,(select Name from C_UserType where  C_UserType.Lever= C_User.C_UserTypeID) C_UserTypeName  ";
            page.strWhere = " and [Order].ParentUser='' and OrderPost.PostNo is not null " + where;
            page.strOrder = " OrderPost.Dat Desc";
            page.LoadList();

                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        //[B_MenuRightsTag("发货", "NoSendOrders")]
        //public ActionResult SendOrder(string orderNo)
        //{
        //    OrderVM order = new OrderVM();
        //    order.LoadOrder(orderNo);
        //    if (order.order.AuditState != "已审核")
        //    {
        //        return View("Error", new ErrorPage { Title = "", Message = "该订单未审核，不能发货" });
        //    }
        //    if (order.order == null)
        //    {
        //        return View("Error", new ErrorPage { Title = "", Message = "找不到路啦" });
        //    }
        //    return View(order);
        //}
        //[B_MenuRightsTag("发货", "NoSendOrders")]
        //public ContentResult ToSendOrder(OrderPost orderPost)
        //{
        //    if (string.IsNullOrWhiteSpace(orderPost.OrderNo) || string.IsNullOrWhiteSpace(orderPost.PostNo) || orderPost.CodeID == 0)
        //    {
        //        return Content("您还没有填完哦");
        //    }
        //    List<ScaleOutStoke> OutStokeModel = ScaleOutStoke.GetListByOrderNo(orderPost.OrderNo);
        //   // 方便测试   暂时注释
        //    //if (OutStokeModel.Count <= 0) 
        //    //{
        //    //    return Content("该订单还没扫码出库，不能发货！");
        //    //}


        //    string msg = string.Empty;
        //    BasePostCode postCode = BasePostCode.GetEntityByID(orderPost.CodeID);
        //    if (postCode == null)
        //    {
        //        return Content("该快递没有收录");
        //    }

        //    orderPost.PostName = postCode.PostName;//快递名
        //    OrderPost old = OrderPost.GetInfoByOrderNo(orderPost.OrderNo);
        //    orderPost.Dat = DateTime.Now;
        //    if (old != null && !string.IsNullOrWhiteSpace(old.PostNo))
        //    {
        //        orderPost.ID = old.ID;
        //        int rtn = 0;
        //        rtn = orderPost.UpdateByID();
        //        msg = rtn > 0 ? "ok" : "保存失败";
        //    }
        //    else
        //    {
        //        msg = OrderPost.SendOrder(orderPost, CurrentUser.UserName);
        //    }


        //    if (msg == "ok")
        //    {
        //        return Content("ok");
        //    }
        //    else
        //    {
        //        return Content(msg);

        //    }
        //}

    }
}
