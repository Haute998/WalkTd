using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class j_mymailController : j_baseController
    {
        //
        // GET: /j_mymail/
        /// <summary>
        /// 我的收货地址
        /// </summary>
        /// <returns></returns>
        public ActionResult MyAddress()
        {

            return View();
        }
        public ActionResult jMyAddress()
        {

            return View();
        }
        /// <summary>
        /// 获取我的收获地址
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMyAddresss(int pageIndex)
        {
            PageJsonModel<jf_UserMail> page = new PageJsonModel<jf_UserMail>();
            page.pageIndex = pageIndex;
            page.strForm = " jf_UserMail ";
            page.strSelect = "*";
            page.strWhere = string.Format(" and UserName='{0}'", CurrentUser.UserName);
            page.strOrder = " IsDefault desc,ID desc";
            page.LoadListNoCnt();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

      
        /// <summary>
        /// 新增收货地址页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddAddress()
        {
            return View();
        }
        /// <summary>
        /// 修改收货地址页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditAddress(int id)
        {
            jf_UserMail contact = jf_UserMail.GetEntityByID(id);
            if (contact.UserName != CurrentUser.UserName)
            {
                return View(ErrorPage.ViewName, new ErrorPage { Message = "非法请求" });
            }
            return View(contact);
        }


        /// <summary>
        /// 收货地址添加
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public ContentResult MailAdd(jf_UserMail contact)
        {
            contact.UserName = CurrentUser.UserName;
            string verirtn = contact.veri();



            if (verirtn != string.Empty)
            {
                return Content("fail|" + verirtn);
            }


            string addressStr = Request["PCAids"];
            if (string.IsNullOrWhiteSpace(addressStr))
            {
                return Content("请选择所在地");
            }
            string[] addre = addressStr.Split(',');

            for (int i = 0; i < addre.Length; i++)
            {
                if (i == 0)
                {
                    contact.Province = addre[i];
                }
                else if (i == 1)
                {
                    contact.City = addre[i];
                }
                else if (i == 2)
                {
                    contact.Area = addre[i];
                }
            }






            int contactID = contact.InsertAndReturnIdentity();
            if (contact.IsDefault)
            {
                jf_UserMail.CancelDefaultNotID(contact.ID);
            }
            return Content("ok|" + contactID.ToString());
        }

        /// <summary>
        /// 收货地址修改
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public ContentResult MailEdit(jf_UserMail contact)
        {
            contact.UserName = CurrentUser.UserName;
            string verirtn = contact.veri();
            if (verirtn != string.Empty)
            {
                return Content("fail|" + verirtn);
            }
            int rtn = contact.UpdateByID();
            if (rtn > 0)
            {
                if (contact.IsDefault)
                {
                    jf_UserMail.CancelDefaultNotID(contact.ID);
                }
                return Content("ok|" + rtn);
            }
            return Content("fail|网络异常，请稍后再试");
        }

        public ContentResult Del(int id)
        {
            jf_UserMail mail = jf_UserMail.GetEntityByID(id);
            if (mail.UserName != CurrentUser.UserName)
            {
                return Content("非法请求");
            }
            int rtn = jf_UserMail.DeleteByID(id);
            return Content(rtn > 0 ? "ok" : "删除失败");
        }

        /// <summary>
        /// 获取自己的所有邮寄地址
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMyMail()
        {
            List<jf_UserMail> mails = jf_UserMail.GetMyMail(CurrentUser.UserName);
            if (mails != null && mails.Count > 0)
            {
                return Json(mails, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取默认收货地址
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMyDefaultMail()
        {
            jf_UserMail mail = jf_UserMail.GetDefaultMailByUser(CurrentUser.UserName);
            if (mail != null)
            {
                return Json(mail, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
