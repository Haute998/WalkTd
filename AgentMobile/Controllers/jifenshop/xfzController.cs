using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using DAL;

namespace AgentMobile.Controllers
{
    public class xfzController : xfzbaseController
    {
        //
        // GET: /xfz/

        public ActionResult Index()
        {
            ViewData["user"] = CurrentUser;
            return View();
        }
        public ActionResult Index6()
        {
            ViewData["user"] = CurrentUser;
            return View();
        }
        public ActionResult ren()
        {
            C_ConsumerWxVM wxvm = new C_ConsumerWxVM();
            wxvm.LoadUserVMByUserName(CurrentUser.UserName);



            ViewData["user"] = wxvm;





            return View();
        }



        public ActionResult lingqurecord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Getlingqurecord(int pageIndex)
        {
            PageJsonModel<j_jflog> page = new PageJsonModel<j_jflog>();
            page.pageIndex = pageIndex;
            page.strForm = "j_jflog";
            page.strSelect = "*";
            page.strWhere = string.Format(" and Type='领取' and UserName='{0}'", CurrentUser.UserName);
            page.strOrder = " j_jflog.ID desc";
            page.LoadListNoCnt();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }











        public ActionResult Index1()
        {
            ViewData["user"] = CurrentUser;
            return View();
        }

        public ActionResult rank1(int pageIndex, int pageSize)
        {
            PageJsonModel<C_Consumer> page = new PageJsonModel<C_Consumer>();
            page.pageIndex = 1;
            page.pageSize = 10000;
            page.strForm = " C_Consumer left join C_UserWxInfo on C_Consumer.UserName=C_UserWxInfo.C_ConsumerUserName ";
            page.strSelect = " C_Consumer.UserName,SUM(jf)as jf,isnull(C_UserWxInfo.headimgurl,'') wx_headurl,isnull(C_UserWxInfo.nickname,'') wx_name";
            page.strWhere = " and C_Consumer.Type='消费者' group by C_Consumer.UserName,C_UserWxInfo.headimgurl,C_UserWxInfo.nickname ";
            page.strOrder = " SUM(jf) desc";
            page.LoadListNoCnt();
            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GoodsList(int pageIndex, jf_GoodsSearch condition)
        {
            string where = string.Empty;

            if (condition != null)
            {
                if (!string.IsNullOrWhiteSpace(condition.keyword))
                {
                    string keyword = Common.Filter(condition.keyword);
                    where += string.Format(" and GoodsName like '%{0}%'", keyword);
                }
            }

            PageJsonModel<jf_Goods> page = new PageJsonModel<jf_Goods>();
            page.pageIndex = pageIndex;
            page.strForm = "jf_Goods";
            page.strSelect = "*";
            page.strWhere = " and PublishStat='已上架' " + where;
            page.strOrder = " ID desc";
            page.LoadListNoCnt();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }



        public ActionResult MyOrders(string state)
        {
            ViewData["state"] = state;
            return View();
        }

        [HttpPost]
        public ActionResult GetMyOrders(int pageIndex, string state)
        {
            PageJsonModel<jf_lpOrder> page = new PageJsonModel<jf_lpOrder>();
            page.pageIndex = pageIndex;
            page.strForm = "jf_lpOrder left join j_OrderPost on jf_lpOrder.OrderNo=j_OrderPost.orderNo";
            page.strSelect = "jf_lpOrder.*,j_OrderPost.PostName,j_OrderPost.PostNo";
            page.strWhere = string.Format(" and UserName='{0}'", CurrentUser.UserName);
            page.strOrder = " jf_lpOrder.ID desc";
            if (!string.IsNullOrWhiteSpace(state))
            {
                page.strWhere += string.Format(" and OrderState='{0}'", state);
            }
            page.LoadListNoCnt();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="orderno"></param>
        /// <returns></returns>
        public ActionResult orderDetail(string orderno)
        {
            jf_OrderVM order = new jf_OrderVM();
            order.LoadOrder(orderno);
            if (order.order == null)
            {
                return View("Error", new ErrorPage { Title = "", Message = "访问路径异常" });
            }
            return View(order);
        }


        public ActionResult jifengo()
        {
            ViewData["user"] = CurrentUser;
            bool IsWx = false;
            if (Request.UserAgent.ToLower().Contains("micromessenger"))
            {
                IsWx = true;
            }

            ViewData["IsWx"] = IsWx ? "1" : "0";
            if (IsWx)
            {
                WXVariousApi VariousApi = new WXVariousApi();
                VariousApi.LoadWxConfigIncidentalAccess_token();
                string nonceStr = WXVariousApi.GenerateNonceStr();
                string timestamp = WXVariousApi.GenerateTimeStamp();
                ViewData["signature"] = VariousApi.GetSignature(Request.Url.ToString(), nonceStr, timestamp);
                ViewData["nonceStr"] = nonceStr;
                ViewData["timestamp"] = timestamp;
                ViewData["AppID"] = VariousApi.WxConfig.APPID;
            }

            return View();
        }
        /// <summary>
        /// 订单取消
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public ContentResult orderCancel(string orderNo)
        {
            string rtn = jf_lpOrder.CancelOrder(orderNo);
            if (rtn == "ok")
            {
                jf_lpOrderLog.LogAdd(orderNo, "订单取消", "客户【" + CurrentUser.UserName + "】取消了订单", "Mobile");
            }
            return Content(rtn);
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public ContentResult orderReceipt(string orderNo)
        {
            string rtn = jf_lpOrder.ReceiptOrder(orderNo);
            if (rtn == "ok")
            {
                jf_lpOrderLog.LogAdd(orderNo, "确认收货", "客户【" + CurrentUser.UserName + "】确认收货", "Mobile");
            }
            return Content(rtn);
        }


        public ActionResult datashow() 
        {
            C_ConsumerWxVM wxvm = new C_ConsumerWxVM();
            wxvm.LoadUserVMByUserName(CurrentUser.UserName);
            ViewData["user"] = wxvm;
            return View();
        }

        /// <summary>
        /// 领取积分失败
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult toGet(string code, string type)
        {

            if (string.IsNullOrWhiteSpace(CurrentUser.Mobile)) 
            {
                return Content("ziliao|请补充资料再领取积分");
            }




            if (type == "input")
            {
                type = "手输";
            }
            else if (type == "qrcode")
            {
                type = "扫码";
                code = code.SubStringSafe(code.IndexOf("code=") + 5, code.Length - 1);
                Scale codeModel = Scale.GetEntityBySmallCode(code);
                if (codeModel == null)
                {
                    Log.Instance.Write(code, "toGet_NoQRCode");
                    return Content("fail|积分码不存在呀");
                }
                code = codeModel.SmallCode;
            }
            else
            {
                type = "未知";
            }
            string rtn = Scale.GetIntegral(CurrentUser.UserName, code, type);
            return Content(rtn);
        }


        public ActionResult ziliao(C_Consumer user) 
        {
            if (string.IsNullOrWhiteSpace(user.Mobile)) 
            {
                return Content("请输入手机号");
            }

            if (user.Mobile.Length != 11)
            {
                return Content("手机号不正确");

            }
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return Content("请输入姓名");
            }
            if (string.IsNullOrWhiteSpace(user.IDCard))
            {
                return Content("请输入身份证号码");
            }

            if (string.IsNullOrWhiteSpace(user.Sex))
            {
                return Content("请选择性别");
            }

            string addressStr = Request["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            string Province = "";
            string City = "";
            string Area = "";

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    Province = addre[i];
                }
                else if (i == 1)
                {
                    City = addre[i];
                }
                else if (i == 2)
                {
                    Area = addre[i];
                }
            }



            C_Consumer old = C_Consumer.GetEntityByUserName(CurrentUser.UserName);


            old.Mobile = user.Mobile;
            old.Name = user.Name;
            old.IDCard = user.IDCard;
            old.Sex = user.Sex;
            old.Province = Province;
            old.City = City;
            old.Area = Area;
            old.UpdateByID();

            return Content("ok");
        }

    }
}
