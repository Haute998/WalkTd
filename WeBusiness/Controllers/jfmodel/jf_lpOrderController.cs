using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class jf_lpOrderController : BaseController
    {
        //
        // GET: /jf_lpOrder/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel(jf_lpOrderSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select OrderNo,OrderName,SumIntegral,OrderMan,OrderMobile,Remark from [jf_lpOrder] where 1=1 ");
            where.Append(jf_lpOrderSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "订单单号", "订单名称", "总积分", "收货人", "电话", "备注" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "订单" + DateTime.Now.ToShortTimeString() + ".xls");
        }



        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetOrders(jf_lpOrderSearch condition)
        {
            string where = jf_lpOrderSearch.StrWhere(condition);

            PageJsonModel<jf_lpOrder> page = new PageJsonModel<jf_lpOrder>();
            page.pageIndex = condition.pageIndex;
            page.strForm = " jf_lpOrder ";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = where;
            page.strOrder = " ID Desc";
            page.LoadList();
            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("订单详情", "Index")]
        public ActionResult OrderDetail(string orderNo)
        {
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "订单号有误" });
            }
            jf_OrderVM order = new jf_OrderVM();
            order.LoadOrder(orderNo);
            return View(order);
        }

        [B_MenuRightsTag("查看订单日志", "Index")]
        public ActionResult GetOrderLogPage(int pageIndex, string orderNo)
        {
            PageJsonModel<jf_lpOrderLog> page = new PageJsonModel<jf_lpOrderLog>();
            page.pageIndex = pageIndex;
            page.pageSize = 100;
            page.strForm = " jf_lpOrderLog ";
            page.strSelect = " * ";
            page.strWhere = " and OrderNo='" + Common.Filter(orderNo) + "'";
            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }







        /// <summary>
        /// 待发货订单页面
        /// </summary>
        /// <returns></returns>
        [B_MenuRightsTag("查看")]
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
        public ActionResult GetNoSendOrders(jf_lpOrderSearch condition)
        {
            string where = string.Empty;
            //订单号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (OrderNo like '%{0}%' or OrderName like '%{0}%' or OrderMan like '%{0}%' or OrderMobile like '%{0}%' or [Address] like '%{0}%') ", condition.keyword);
            }
            where += string.Format(" and OrderState='待发货' and PayState='已支付' and AuditState='已审核'");

            PageJsonModel<jf_lpOrderShowVM> page = new PageJsonModel<jf_lpOrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " jf_lpOrder ";
            page.strSelect = @" * ";
            page.strWhere = where;
            page.strOrder = " DatPay Asc";
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
        public ActionResult GetSentOrders(jf_lpOrderSearch condition)
        {
            string where = string.Empty;
            //订单号
            if (!string.IsNullOrWhiteSpace(condition.OrderNo))
            {
                where += " and OrderNo like '%" + Common.Filter(condition.OrderNo) + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(@" and (jf_lpOrder.OrderNo like '%{0}%' or jf_lpOrder.OrderName like '%{0}%' 
                                           or OrderMan like '%{0}%' or OrderMobile like '%{0}%' 
                                           or [Address] like '%{0}%' or [PostName] like '%{0}%' or [PostNo] like '%{0}%') ", condition.keyword);
            }

            PageJsonModel<jf_lpOrderShowVM> page = new PageJsonModel<jf_lpOrderShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " jf_lpOrder left join j_OrderPost on j_OrderPost.OrderNo=jf_lpOrder.OrderNo ";
            page.strSelect = @" jf_lpOrder.*,
                ISNULL(j_OrderPost.PostName,'') PostName,ISNULL(j_OrderPost.PostNo,'') PostNo,ISNULL(j_OrderPost.Dat,0) PostDat  ";
            page.strWhere = " and j_OrderPost.PostNo is not null " + where;
            page.strOrder = " j_OrderPost.Dat Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }












        [B_MenuRightsTag("发货", "NoSendOrders")]
        public ActionResult SendOrder(string orderNo)
        {
            jf_OrderVM order = new jf_OrderVM();
            order.LoadOrder(orderNo);
            if (order.order.OrderState != "待发货")
            {
                return View("Error", new ErrorPage { Title = "", Message = "该订单不能发货" });
            }
            if (order.order == null)
            {
                return View("Error", new ErrorPage { Title = "", Message = "找不到路啦" });
            }
            return View(order);
        }
        [B_MenuRightsTag("发货", "NoSendOrders")]
        public ContentResult ToSendOrder(j_OrderPost orderPost)
        {
            if (string.IsNullOrWhiteSpace(orderPost.OrderNo) || string.IsNullOrWhiteSpace(orderPost.PostNo) || orderPost.CodeID == 0)
            {
                return Content("您还没有填完哦");
            }
            string msg = string.Empty;
            BasePostCode postCode = BasePostCode.GetEntityByID(orderPost.CodeID);
            if (postCode == null)
            {
                return Content("该快递没有收录");
            }
            orderPost.PostName = postCode.PostName;//快递名
            j_OrderPost old = j_OrderPost.GetInfoByOrderNo(orderPost.OrderNo);
            orderPost.Dat = DateTime.Now;
            if (old != null && !string.IsNullOrWhiteSpace(old.PostNo))
            {
                orderPost.ID = old.ID;
                int rtn = 0;
                rtn = orderPost.UpdateByID();
                msg = rtn > 0 ? "ok" : "保存失败";
            }
            else
            {
                msg = j_OrderPost.SendOrder(orderPost, CurrentUser.UserName);
            }


            if (msg == "ok")
            {
                return Content("ok");
            }
            else
            {
                return Content(msg);

            }
        }

    }
}
