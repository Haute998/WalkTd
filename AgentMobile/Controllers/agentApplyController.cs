using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using DAL;
using WeModels.Models.C_UserModel;
using System.Text;

namespace AgentMobile.Controllers
{
    public class agentApplyController : BaseController
    {
        //
        // GET: /agentApply/
        public ActionResult Index(string usertype)
        {
            ViewData["puserdata"] = CurrentUser.UserName;
            ViewData["usertype"] = CurrentUser.C_UserTypeID+"";
            usertype = CurrentUser.C_UserTypeID.ToString();
             
            return View();
        }
        public ActionResult shangchuan(string usertype)
        {
            ViewData["puserdata"] = CurrentUser.UserName;
            ViewData["usertype"] = CurrentUser.C_UserTypeID + "";
            usertype = CurrentUser.C_UserTypeID.ToString();

            return View();
        }
        /// <summary>
        /// 被推荐的申请
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key"></param>
        /// <param name="type"> code为具有有效期 </param>
        /// <param name="usertype">申请等级</param>
        /// <returns></returns>
        public ActionResult Index_Recommended(string id, string key, string type, string usertype)
        {
            if (type == null)
            {
                type = "";
            }

            ViewData["type"] = type;
            int lever = 0;
            int.TryParse(usertype, out lever);

            C_UserType applyUserType = C_UserType.GetEntityByLever(lever);
            if (applyUserType == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "您申请的等级已经不存在啦" });
            }

            C_UserVM user = new C_UserVM();
            user = C_UserVM.GetVMByUserNameOrPhone(id);//推荐人信息
            if (user == null)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "推荐人不存在" });
            }

            C_UserVM P_User = user;
            int P_Chief = 0;
            int MaxLevel = C_UserType.GetMaxLevel();//最大等级


            if (applyUserType.Lever == MaxLevel || user.Chief == 0)
            {
                P_User = null;
                P_Chief = 0;
            }
            else
            {
                while (true)
                {
                    P_User = C_UserVM.GetVMByID(P_User.Chief);//推荐人的上级
                    if (applyUserType.Lever > P_User.userTypeLever)
                    {
                        P_Chief = P_User.ID;
                        break;
                    }
                }
            }

            ViewData["Introducer"] = id;//推荐人账号
            ViewData["IntroducerData"] = user;//推荐人信息
            ViewData["applyUserType"] = applyUserType;

            ViewData["P_User"] = P_User;//上级信息
            return View();
        }

        public ActionResult query()
        {
            return View();
        }

        public ActionResult checkuser(string id)
        {
            if (id == BaseParameters.getOfficialAgent())
            {
                return Content("ok");
            }
            C_UserVM user = C_UserVM.GetVMByUserNameOrPhone(id);
            if (user == null)
            {
                return Content("该上级不存在");
            }
            return Content("ok");
        }
        public ContentResult toApply(FormCollection c)
        {
            C_User user = new C_User();
            int C_UserTypeID = 0;
            int.TryParse(c["C_UserTypeID"], out C_UserTypeID);
            user.C_UserTypeID = C_UserTypeID;

            string addressStr = c["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    user.Province = addre[i];
                }
                else if (i == 1)
                {
                    user.City = addre[i];
                }
                else if (i == 2)
                {
                    user.Area = addre[i];
                }
            }

            string ChiefUserName = c["ChiefUserName"];

            C_UserType usertype = C_UserType.GetEntityByLever(C_UserTypeID);

            user.Phone = c["Phone"];
            if (user.Phone.Length != 11)
            {
                return Content("手机号有误");
            }
            if (C_User.GetPhoneCnt(user.Phone) > 0)
            {
                return Content("该手机号已存在");
            }
            user.DatPwdChange = DateTime.Now;
            user.DatCreate = DateTime.Now;
            user.IsValid = true;
            user.Name = c["Name"];
            user.C_UserTypeID = C_UserTypeID;
            user.Introducer = c["Card"];
            user.WxQRCode = c["WxQRCode"];
            user.Chief = CurrentUser.ID;
            user.PassWord = user.Phone.Substring(user.Phone.Length - 6, 6);
            user.state = "已审核";
            user.wxNo = c["wxNo"];

            user.ID = user.InsertAndReturnIdentity();

            int idLen = user.ID.ToString().Length;
            string idDQ = user.ID.ToString();
            if (idLen < 5)
            {
                string zero = new string('0', 5 - idLen);
                idDQ = zero + idDQ;
            }
            user.UserName = usertype.TypeCode + idDQ;
            user.Identifier = usertype.TypeCode + idDQ;

            if (user.UpdateByID()>0)
            {
                return Content("ok");
            }
            else
            {
                return Content("保存出错");
            }
        }

        public ContentResult toApply_Recommended(FormCollection c)
        {
            C_User user = new C_User();
            int C_UserTypeID = 0;
            int.TryParse(c["C_UserTypeID"], out C_UserTypeID);
            user.C_UserTypeID = C_UserTypeID;

            string addressStr = c["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    user.Province = addre[i];
                }
                else if (i == 1)
                {
                    user.City = addre[i];
                }
                else if (i == 2)
                {
                    user.Area = addre[i];
                }
            }

            user.Introducer = c["Introducer"];
            C_User IntroducerUser = C_User.GetC_UserByUserName(user.Introducer);//推荐人信息

            if (IntroducerUser == null)
            {
                return Content("推荐人信息有误");
            }

            string ChiefUserName = c["ChiefUserName"];
            C_UserType userType = C_UserType.GetEntityByLever(C_UserTypeID);
            if (userType == null)
            {
                return Content("代理级别不存在");
            }

            if (string.IsNullOrWhiteSpace(ChiefUserName))
            {
                user.Chief = 0;
            }
            else
            {
                C_User ChiefC_User = C_User.GetUserByUserName(ChiefUserName);

                if (ChiefC_User == null)
                {
                    return Content("上级不存在");
                }
                user.Chief = ChiefC_User.ID;//上级用户
            }

            user.Phone = c["Phone"];
            if (C_User.GetPhoneCnt(user.Phone) > 0)
            {
                return Content("该手机号已存在");
            }


            user.Card = c["Card"];
            if (user.Card.Length != 18)
            {
                return Content("身份证号码有误，请输入有效的二代身份证");
            }

            if (C_User.GetCardCnt(user.Card) > 0)
            {
                return Content("该身份证已存在");
            }

            user.DatCreate = DateTime.Now;
            user.IsValid = true;
            user.Name = c["Name"];
            user.PassWord = user.Card.SubStringSafe(user.Card.Length - 6, 6);
            user.PortraitUrl = WeConfig.wx_domain + "/images/27.png";


            user.state = "未审核";
            user.wxNo = c["wxNo"];
            if (string.IsNullOrWhiteSpace(user.wxNo))
            {
                return Content("微信号不能为空");
            }

            user.UserName = "FZ" + (C_User.GetTopUseID() + 1 + 1000);
            user.Identifier = "FZ" + "00000".Substring(0, 5 - (C_User.GetTopUseID() + 1).ToString().Length) + ((C_User.GetTopUseID() + 1));
            string imgName = DateTime.Now.ToString("yyyy-MM-dd-HH-ss") + DateTime.Now.Ticks;
            bool flag = false;
            user.ID = user.InsertAndReturnIdentity();

            try
            {
                if (!Directory.Exists(Server.MapPath("~/File/User/AuthData/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/File/User/AuthData/"));
                }
                flag = true;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "agentApply_toApply_error");
            }

            if (flag)
            {

                return Content("ok");

            }
            else
            {
                return Content("保存出错");
            }
        }

        /// <summary>
        /// 意向代理
        /// </summary>
        /// <returns></returns>
        public ActionResult liaojie(string username = "")
        {
            C_UserVM user = C_UserVM.GetVMByUserName(username);

            if (user == null)
            {
                username = "";
            }
            ViewData["user"] = user;




            return View();
        }
        public ContentResult toliaojie(FormCollection c)
        {
            AgentIntention user = new AgentIntention();

            string addressStr = c["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    user.Province = addre[i];
                }
                else if (i == 1)
                {
                    user.City = addre[i];
                }
                else if (i == 2)
                {
                    user.Area = addre[i];
                }
            }

            string ChiefUserName = c["ChiefUserName"];

            if (ChiefUserName != "")
            {
                C_User ChiefC_User = C_User.GetUserByUserName(ChiefUserName);

                if (ChiefC_User == null)
                {
                    return Content("上级不存在");
                }
            }
            user.ChiefUser = ChiefUserName;

            user.Phone = c["Phone"];
            if (user.Phone.Length != 11)
            {
                return Content("手机号有误");
            }
            if (C_User.GetPhoneCnt(user.Phone) > 0)
            {
                return Content("该手机号已存在");
            }

            user.DatCreate = DateTime.Now;
            user.Name = c["Name"];


            user.wxNo = c["wxNo"];
            if (string.IsNullOrWhiteSpace(user.wxNo))
            {
                return Content("微信号不能为空");
            }


            user.ID = user.InsertAndReturnIdentity();

            if (user.ID>0)
            {

                return Content("ok");

            }
            else
            {
                return Content("提交失败");
            }
        }
    }
}
