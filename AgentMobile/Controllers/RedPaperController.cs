using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.WxModel;
using DAL;

namespace AgentMobile.Controllers
{
    public class RedPaperController : HBBaseController
    {
        /// <summary>
        /// 裂变分享红包
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public ActionResult ShareRedPack(string id)
        {
            try
            {
                LotteryActivitys act = LotteryActivitys.GetEntityByID(1);
                ViewData["act"] = act;
                ViewData["id"] = 1;
                ViewData["IsWx"] = Request.UserAgent.ToLower().Contains("micromessenger");
                ViewData["user"] = CurrentUser;
            }
            catch (Exception e)
            {
                DAL.Log.Instance.Write(e.Message, "红包获取");
            }

            return View();
        }

        public ActionResult getpack(int id, string IntegralCode)
        {
            DAL.Log.Instance.Write("" + id + "||" + IntegralCode + "||" + CurrentUser.UserName + "||" + Request.UserHostAddress, "红包获取---");
            string rtn = RedPackOrder.getRedPack(id, IntegralCode, CurrentUser.UserName, Request.UserHostAddress);

            return Content(rtn);
        }
        public ActionResult getpack1(int id, string IntegralCode, string Phone)
        {
            string ip1 = GetWebClientIp();
            CurrentUser.Mobile = Phone;
            CurrentUser.UpdateByID();
            string rtn = RedPackOrder.getRedPack(id, IntegralCode, CurrentUser.UserName, Request.UserHostAddress);
            return Content(rtn);
        }

        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult fengxiang(string Code)
        {
            ViewData["Code"] = Code;
            return View();
        }
        /// <summary>
        /// 普通红包
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult NormalRedPack(string Code)
        {
            RedPackShare redmodel = RedPackShare.GetEntityByCode(Code);
            string msg = "";
            int sl = 0;
            if (redmodel == null)
            {
                msg = "非法请求！";
            }
            else
            {
                if (redmodel.ReceiveCnt >= redmodel.RedCnt)
                {
                    msg = "您来晚了，红包派完了！";
                }
                sl = redmodel.RedCnt - redmodel.ReceiveCnt;
            }
            if (CurrentUser.Mobile.Length > 0)
            {
                msg = "您已经参与过此活动！";
            }

            RedPackLottery code = RedPackLottery.GetEntityByUserName(CurrentUser.UserName);
            if (code != null)
            {
                msg = "您已经参与过此活动";
            }

            ViewData["msg"] = msg;
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            LotteryActivitys act = LotteryActivitys.GetEntityByID(3);
            ViewData["act"] = act;
            ViewData["user"] = CurrentUser;
            ViewData["Code"] = Code;
            ViewData["sl"] = sl;



            return View();
        }

        /// <summary>
        /// 防伪码查询记录
        /// </summary>
        public ActionResult AuthenticityList()
        {
            ViewData["IsWx"] = Request.UserAgent.ToLower().Contains("micromessenger");
            return View();
        }

        /// <summary>
        /// 我的扫码记录
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ActionResult LoadAuthenticityList(BaseSearch condition)
        {
            string where = " and UserName='" + CurrentUser.UserName + "' and (ActivityID='1'  or ActivityID='3')";

            List<LotteryRecord> mode = LotteryRecord.GetEntitysbywhere(where);
            where = " and l.UserName='" + CurrentUser.UserName + "' and (ActivityID='1'  or ActivityID='3')";
            if (mode.Count > 0)
            {
                //where = " and l.IntegralCode='" + mode[0].IntegralCode + "'";
            }

            PageJsonModel<LotteryRecord> page = new PageJsonModel<LotteryRecord>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = string.Format(" LotteryRecord  l left join C_User  c on c.userName=l.UserName ");
            page.strSelect = " l.* ,PortraitUrl,NickName ";
            //page.strWhere = " " + where;
            page.strOrder = " l.ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询领取红包
        /// </summary>
        public ActionResult AuthenticityQuery()
        {
            string msg = "";

            List<LotteryRecord> Lmodel = LotteryRecord.GetEntitysbytg("and LotteryRecord.UserName='" + CurrentUser.UserName + "' and (LotteryRecord.ActivityID='3' or LotteryRecord.ActivityID='1' )");
            if (Lmodel.Count > 0)
            {
                msg = "您已经参与过此活动！";
            }
            RedPackLottery code = RedPackLottery.GetEntityByUserName2(CurrentUser.UserName);
            if (code != null)
            {
                msg = "您已经参与过此活动";
            }
            ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
            LotteryActivitys act = LotteryActivitys.GetEntityByID(1);
            ViewData["act"] = act;
            ViewData["IsWx"] = Request.UserAgent.ToLower().Contains("micromessenger");
            ViewData["user"] = CurrentUser;

            ViewData["msg"] = msg;
            return View();
        }

        public ActionResult getSharepack(int id, string IntegralCode, string Phone)
        {

            string ip1 = GetWebClientIp();
            if (CurrentUser.Mobile.Length > 0)
            {
                return Content("fail|您已经参与过此活动！");
            }
            CurrentUser.Mobile = Phone;
            CurrentUser.UpdateByID();
            string rtn = RedPackOrder.getRedSharePack(id, IntegralCode, CurrentUser.UserName, Request.UserHostAddress);
            return Content(rtn);
        }

        private static string GetWebClientIp()
        {

            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null || System.Web.HttpContext.Current.Request.ServerVariables == null)
                {
                    return "";
                }

                string CustomerIP = "";

                //CDN加速后取到的IP simone 090805
                CustomerIP = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!String.IsNullOrEmpty(CustomerIP))
                {
                    return CustomerIP;
                }

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (CustomerIP == null)
                    {
                        CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    CustomerIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.Compare(CustomerIP, "unknown", true) == 0 || String.IsNullOrEmpty(CustomerIP))
                {
                    return System.Web.HttpContext.Current.Request.UserHostAddress;
                }
                return CustomerIP;
            }
            catch { }

            return userIP;
        }

        public ActionResult AuthenticityShow(string money, string Code, string type = "0")
        {
            string mo = Convert.ToDecimal(money).ToString("0.00");
            ViewData["money"] = mo;
            ViewData["Code"] = Code;
            ViewData["type"] = type;
            return View();
        }

        /// <summary>
        /// 红包分享
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult ShareRedPack(String Code)
        //{
        //    ViewData["ip"] = System.Web.HttpContext.Current.Request.ServerVariables.Get("Remote_Addr").ToString();
        //    LotteryActivitys act = LotteryActivitys.GetEntityByID(3);
        //    ViewData["act"] = act;
        //    ViewData["IsWx"] = IsWx;
        //    ViewData["user"] = CurrentUser;
        //    ViewData["Code"] = Code;
        //    return View();
        //}
    }
}
