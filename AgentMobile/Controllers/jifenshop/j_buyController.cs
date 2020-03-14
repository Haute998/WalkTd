using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class j_buyController : j_baseController
    {
        //
        // GET: /j_buy/

        /// <summary>
        /// 订单确认页面
        /// </summary>
        /// <param name="mpk">商品编号</param>
        /// <param name="cartids">购物车编号集合 逗号分隔</param>
        /// <param name="type">类型：mail/cart 即：商品/购物车</param>
        /// <returns></returns>
        public ActionResult order(string mpk, string cartids, string type, int cnt = 1)
        {
            List<jf_MGoodsVM> goodsVMs = new List<jf_MGoodsVM>();
            if (type == "mail")
            {
                cartids = string.Empty;
                //单商品直接购买方式
                int mid = 0;
                int.TryParse(mpk, out mid);
                if (mid == 0)
                {
                    return View("Error", new ErrorPage { Title = "", Message = "访问路径异常" });
                }
                jf_MGoodsVM goodsVM = new jf_MGoodsVM();
                goodsVM.LoadGoodsVMByGoodsID(mid);
                if (goodsVM.Goods == null)
                {
                    return View("Error", new ErrorPage { Title = "", Message = "访问路径异常" });
                }
                goodsVM.cnt = cnt;

                goodsVM.Goods.BuyIntegral = goodsVM.Goods.SaleIntegral;//当前时购买的价格


                goodsVMs.Add(goodsVM);
            }
            else
            {
                return View("Error", new ErrorPage { Title = "", Message = "访问路径异常" });
            }

            return View(goodsVMs);
        }
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="mpk">商品编号，直接购买单个商品时必填</param>
        /// <param name="mcnt">商品数量，直接购买单个商品时必填</param>
        /// <param name="mailpk">收货地址编号</param>
        /// <param name="type">类型：mail/cart 即：商品/购物车</param>
        /// <returns></returns>
        public ContentResult orderCreate(FormCollection c)
        {
            int mid = 0;//商品编号
            int.TryParse(c["mpk"], out mid);
            int mailid = 0;//收货地址编号
            int.TryParse(c["mailpk"], out mailid);

            int mcnt_i = 0;//商品购买数量
            int.TryParse(c["mcnt"], out mcnt_i);
            string type = c["type"];
            if (type == "mail" && mid == 0)
            {
                return Content("fail|商品有误，请重新访问");
            }
            if (mailid == 0)
            {
                return Content("fail|收货地址有误，请重新访问");
            }
            jf_OrderCreateHelper orderCreateObj = new jf_OrderCreateHelper();
            string result = orderCreateObj.OrderBulid(mid, mcnt_i, mailid, "", type, CurrentUser.UserName, c["Remark"]);
            if (result == "ok")
            {
                return Content("ok|" + orderCreateObj.order.OrderNo);
            }
            return Content("alert|" + result);
        }


        /// <summary>
        /// 积分支付
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public ContentResult jfpay(string orderno)
        {
            string rtn = jf_lpOrder.jf_lpOrderPay_jf(orderno);
            return Content(rtn);
        }
    }
}
