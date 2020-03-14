using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class B_UserMsgController : BaseController
    {
        public ActionResult NewSYSMsg()
        {
            return View();
        }
        private string StrWhere(SYSNotifyMsgSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (MsgType like '%{0}%' or MsgContent like '%{0}%')", condition.keyword);
            }
            if (!string.IsNullOrWhiteSpace(condition.ReadState))
            {
                where += string.Format(" and IsRead={0} ", (condition.ReadState == "0" ? 0 : 1));
            }
            return where;
        }
        public ActionResult GetNewSYSMsg(SYSNotifyMsgSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<SYSNotifyMsg> page = new PageJsonModel<SYSNotifyMsg>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " SYSNotifyMsg ";
            page.strSelect = "*";
            page.strWhere = string.Format(" and UserName='{0}' ", CurrentUser.UserName) + where;
            page.strOrder = "Dat desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        public ContentResult GetNewSYSMsgCnt()
        {
            int userCount = C_User.GetC_UserBySta("未审核");       // 非直属待审核
            int OrderCount = Order.GetC_UserCircles("");            // 待审核订单
            int OrderCounts = Order.GetOrder("");                   // 未发货订单
            int uCount = C_User.GetC_UserCircles(0);                // 直属总部

            int Upcount = C_UserUpGrade.GetC_UserByUpGrade("未审核"); //未审核升级信息
            int acount = userCount + uCount + Upcount; //总和

            int msgcnt = SYSNotifyMsg.GetUnReadMsgCount(CurrentUser.UserName);//未审核信息
            int sggcnt = C_UserAdvice.GetNoAuditCount();

            return Content(msgcnt + "," + acount + "," + OrderCount + "," + OrderCounts + "," + uCount + "," + userCount + "," + Upcount + "," + sggcnt);
        }
        public ContentResult ReadMsg(int id)
        {
            int rtn = SYSNotifyMsg.ReadMsgByID(id);
            return Content(rtn > 0 ? "ok" : "读取异常");
        }

    }
}
