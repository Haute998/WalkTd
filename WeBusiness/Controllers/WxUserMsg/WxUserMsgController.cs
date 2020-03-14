using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using System.Data;
using System.Text;

namespace WeBusiness.Controllers
{
    public class WxUserMsgController : BaseController
    {
        //
        // GET: /C_User/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看")]
        public ActionResult Indextg()
        {
            return View();
        }
        public ActionResult ii()
        {
            return View();
        }

        public ActionResult DrawPrizeDetail()
        {
            return View();
        }

        /// <summary>
        /// 派送普通红包明细
        /// </summary>
        /// <returns></returns>
        public ActionResult NormalRedPackDetail()
        {
            System.Data.SqlClient.SqlParameter[] paramters = null;
            object PMoney0obj = DAL.SqlHelper.ExecuteScalar(string.Format("SELECT sum(redMoney)as PMoney0 FROM  LotteryRecord "), paramters);
            decimal PMoney0 = 0;
            try
            {
                PMoney0 = PMoney0obj == null ? 0 : Convert.ToDecimal(PMoney0obj);
            }
            catch
            {
            }
            ViewData["PMoney0"] = PMoney0;
            return View();
        }
        /// <summary>
        /// 拉黑客户
        /// </summary>
        /// <param name="cklst"></param>
        /// <returns></returns>
        [B_MenuRightsTag("拉黑", "Index")]
        public ContentResult DeFriend(int[] cklst)
        {
            if (cklst == null || cklst.Length <= 0)
            {
                return Content("请勾选你要拉黑的人");
            }
            string rtn = C_WxUser.DeFriend(cklst, true);
            return Content(rtn);
        }
        [B_MenuRightsTag("取消拉黑", "Index")]
        public ContentResult NoDeFriend(int[] cklst)
        {
            if (cklst == null || cklst.Length <= 0)
            {
                return Content("请勾选你要取消拉黑的人");
            }
            string rtn = C_WxUser.DeFriend(cklst, false);
            return Content(rtn);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public FileResult ExportExcel(C_WxUserShowVM.Condition condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select UserName,NickName,TrueName,Mobile from [C_WxUser] where 1=1 ");
            where.Append(StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "用户账号", "昵称", "姓名", "电话" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "客户" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        private string StrWhere(C_WxUserShowVM.Condition condition)
        {
            string where = string.Empty;
            //客户ID
            if (!string.IsNullOrWhiteSpace(condition.UserName))
            {
                where += " and UserName like '%" + Common.Filter(condition.UserName) + "%' ";
            }
            //微信关注状态
            if (!string.IsNullOrWhiteSpace(condition.wx_subscribeStat))
            {
                if (condition.wx_subscribeStat == "1")
                {
                    where += " and wx_subscribe=1 ";
                }
                else if (condition.wx_subscribeStat == "0")
                {
                    where += " and (wx_subscribe is null or wx_subscribe=0) ";
                }
            }
            //客户微信昵称
            if (!string.IsNullOrWhiteSpace(condition.wx_nickname))
            {
                condition.wx_nickname = Common.Filter(condition.wx_nickname);
                where += " and (wx_nickname like '%" + condition.wx_nickname + "%' or NickName like '%" + condition.wx_nickname + "%') ";
            }
            //关键字搜索
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                condition.keyword = Common.Filter(condition.keyword);
                where += string.Format(@" and (wx_nickname like '%{0}%' or NickName like '%{0}%' or UserName like '%{0}%') ", condition.keyword);
            }

            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and wx_subscribe_time >='{0} 00:00:00' ", Common.Filter(condition.DatCreateB));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and wx_subscribe_time <'{0} 23:59:59' ", Common.Filter(condition.DatCreateE));
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetUserPages(C_WxUserShowVM.Condition condition)
        {
            string where = StrWhere(condition);

            PageJsonModel<C_WxUserShowVM> page = new PageJsonModel<C_WxUserShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"(select a.ID,a.UserName,a.IsValid,a.NickName,a.Mobile,a.Grade,a.TrueName,
                                a.WxNo,a.DatRegister,a.Email,a.PortraitUrl,wx.openid wx_openid,
                                wx.nickname wx_nickname,wx.sex wx_sex,wx.headimgurl wx_headimgurl,wx.remark wx_remark,
                                wx.groupid wx_groupid,wx.subscribe wx_subscribe,wx.subscribe_time wx_subscribe_time,
                                wx.country wx_country,wx.language wx_language 
                                from C_WxUser as a left join C_UserWxInfo as wx on a.UserName=wx.C_UserName) as userwxvm";
            page.strSelect = "*";
            page.strWhere = where + " and Email<>'T '";
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ID desc";
            }

            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetUserPages2(C_WxUserShowVM.Condition condition)
        {
            string where = StrWhere(condition);

            PageJsonModel<C_WxUserShowVM> page = new PageJsonModel<C_WxUserShowVM>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = @"(select a.ID,a.UserName,a.IsValid,a.NickName,a.Mobile,a.Grade,a.TrueName,
                                a.WxNo,a.DatRegister,a.Email,a.PortraitUrl,wx.openid wx_openid,
                                wx.nickname wx_nickname,wx.sex wx_sex,wx.headimgurl wx_headimgurl,wx.remark wx_remark,
                                wx.groupid wx_groupid,wx.subscribe wx_subscribe,wx.subscribe_time wx_subscribe_time,
                                wx.country wx_country,wx.language wx_language 
                                from C_WxUser as a left join C_UserWxInfo as wx on a.UserName=wx.C_UserName) as userwxvm";
            page.strSelect = "*";
            page.strWhere = where + " and Email<>''";
            if (string.IsNullOrWhiteSpace(condition.orderby) == false)
            {
                page.strOrder = Common.FilteSQLStr(condition.orderby);
            }
            else
            {
                page.strOrder = "ID desc";
            }

            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        [B_MenuRightsTag("更新微信信息", "Index")]
        public ActionResult UpdateWx(string UserName)
        {
            C_UserWxInfo info = C_UserWxInfo.GetInfoByC_UserName(UserName);
            WXVariousApi VariousApi = new WXVariousApi();
            VariousApi.LoadWxConfigIncidentalAccess_token();
            WXUserInfo wx_userinfo = VariousApi.GetUserInfo(info.openid);
            info.nickname = wx_userinfo.nickname;
            info.headimgurl = wx_userinfo.headimgurl;
            info.groupid = wx_userinfo.groupid;
            info.C_UserName = UserName;
            info.subscribe = (wx_userinfo.subscribe == "0" ? false : true);
            info.country = wx_userinfo.country;
            info.subscribe_time = Common.ConvertToDateTen(wx_userinfo.subscribe_time);
            info.language = wx_userinfo.language;
            bool rtn = true;
            if (!string.IsNullOrWhiteSpace(info.nickname))
            {
                rtn = info.UpdateUserWxInfo();
            }
            return Content(rtn ? "ok" : "更新失败");
        }
        public ActionResult UpdateEmail(string UserName)
        {
            C_WxUser info = C_WxUser.GetUserByUserName(UserName);
            bool rtn = true;
            if (!string.IsNullOrWhiteSpace(info.UserName))
            {
                rtn = info.UpdateUserEmail(UserName);
            }
            return Content(rtn ? "ok" : "设置失败");
        }
        public ActionResult Updateqx(string UserName)
        {
            C_WxUser info = C_WxUser.GetUserByUserName(UserName);
            bool rtn = true;
            if (!string.IsNullOrWhiteSpace(info.UserName))
            {
                rtn = info.UpdateUserqx(UserName);
            }
            return Content(rtn ? "ok" : "设置失败");
        }
    }
}
