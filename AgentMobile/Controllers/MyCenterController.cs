using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class MyCenterController : BaseController
    {
        //
        // GET: /MyCenter/
        /// <summary>
        /// 授权码
        /// </summary>
        /// <returns></returns>
        public ActionResult InviteCode()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            InviteCode code = WeModels.InviteCode.GetListByUserName(CurrentUser.UserName, "授权");

            if (code == null || code.ValidDat < DateTime.Now) 
            {
                code = new InviteCode();
                code.UserName = CurrentUser.UserName;
                code.ValidDat = DateTime.Now.AddMinutes(20);//二十分钟有效
                code.Type = "授权";
                code.ID=code.InsertAndReturnIdentity();
            }
            ViewData["code"] = code;
            return View();
        }
        /// <summary>
        /// 推荐码
        /// </summary>
        /// <returns></returns>
        public ActionResult IntroducerCode() 
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            InviteCode code = WeModels.InviteCode.GetListByUserName(CurrentUser.UserName, "推荐");

            if (code == null || code.ValidDat < DateTime.Now)
            {
                code = new InviteCode();
                code.UserName = CurrentUser.UserName;
                code.ValidDat = DateTime.Now.AddMinutes(20);//二十分钟有效
                code.Type = "推荐";
                code.ID = code.InsertAndReturnIdentity();
            }
            ViewData["code"] = code;

            List<C_UserType> userTypes = C_UserType.GetC_UserTypeHeighter(CurrentUser.ID).ToList();
            userTypes = userTypes.OrderByDescending(m => m.Lever).ToList();
            ViewData["userTypes"] = userTypes;

            return View();
        }


        public ActionResult EditPwd()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }

        public ContentResult toEditPwd(FormCollection c) 
        {
            C_User user=C_User.GetUserByUserName(CurrentUser.UserName);
            string oldpwd = c["oldpwd"];
            string newpwd = c["newpwd"];
            string twonewpwd = c["twonewpwd"];
    
            if (oldpwd != user.PassWord) 
            {
                return Content("原密码错误");
            }
            if (newpwd != twonewpwd) 
            {
                return Content("两次密码输入不一致");
            }

            int rtn = C_User.EditPwd(CurrentUser.UserName, newpwd);
            if (rtn > 0) 
            {
                SYSLog.add("修改经销商手机端密码从[" + oldpwd + "]修改为[" + newpwd + "]", "代理" + CurrentUser.UserName + "(" + CurrentUser.Name + ")", CurrentURL, "修改密码", "经销商手机端");
            }


            return Content(rtn>0?"ok":"修改密码失败");
        }
        /// <summary>
        /// 申请升级
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyUpgrade() 
        {
            C_UserVM user = C_UserVM.GetVMByID(CurrentUser.ID);
            ViewData["user"] = user;
            //List<C_UserType> userTypes = C_UserType.GetLowerByUserID(user.Chief,CurrentUser.ID);
            List<C_UserType> userTypes = C_UserType.GetEntitysAll();
            if (userTypes != null) 
            {
                userTypes = userTypes.OrderByDescending(m => m.Lever).ToList();
                userTypes = userTypes.FindAll(m => m.Lever < user.userTypeLever).ToList();
            }
            ViewData["userTypes"] = userTypes;

            List<C_UserUpGrade> doings = C_UserUpGrade.GetDetailListByAuditStat("未审核", CurrentUser.UserName);
            ViewData["doings"] = doings;
            return View();
        }
        /// <summary>
        /// 提交升级申请
        /// </summary>
        /// <returns></returns>
        public ActionResult toApplyUpgrade(int C_UserTypeID) 
        {
            if (C_UserTypeID == 0) 
            {
                return Content("申请的等级有误");
            }
            C_UserType userType = C_UserType.GetEntityByLever(C_UserTypeID);
            if (userType == null) 
            {
                return Content("申请的等级有误");
            }

            int ordercnt = Order.GetUserOrderCnt(CurrentUser.UserName);
            if (ordercnt <= 0) 
            {
                return Content("您从来没下过单，不能升级！快去下单吧！");
            }

            //C_UserVM P_user = C_UserVM.GetVMByID(CurrentUser.Chief);
            //if (userType.Lever <= P_user.userTypeLever) 
            //{
            //    return Content("您暂时不能升级到" + userType.Name);
            //}

            string rtn = C_UserUpGrade.toUpGrade(CurrentUser.UserName, C_UserTypeID);

            return Content(rtn);
        }
    }
}
