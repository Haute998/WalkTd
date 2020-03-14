using DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSPolicyController : BaseController
    {
        //
        // GET: /SYSPolicy/

        //
        // GET: /SYSAgentKnow/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<SYSPolicy> model = SYSPolicy.GetNavAdvs();
            return View(model);
        }

        [B_MenuRightsTag("查看", "Index")]
        public ActionResult AgentKnowIndex()
        {
            return View();
        }
        [B_MenuRightsTag("保存", "Index")]
        public ActionResult Save(SYSPolicy condition)
        {
            if (string.IsNullOrWhiteSpace(condition.Title))
            {
                return Content("标题不能为空!!");
            }
            if (string.IsNullOrWhiteSpace(condition.Contents))
            {
                return Content("内容不能为空!!");
            }
            string msg = "出错了";
            if (condition.InsertAndReturnIdentity() > 0)
            {
                msg = "ok";
            }
            return Content(msg);

        }
        [B_MenuRightsTag("保存", "Index")]
        public ActionResult EditSave(SYSPolicy condition)
        {
            if (string.IsNullOrWhiteSpace(condition.Title))
            {
                return Content("标题不能为空!!");
            }
            if (string.IsNullOrWhiteSpace(condition.Contents))
            {
                return Content("内容不能为空!!");
            }
            string msg = "出错了";
            if (condition.UpdateByID() > 0)
            {
                msg = "ok";
            }
            return Content(msg);

        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult DetailImgsUp()
        {
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/images/SYSAgentKnow/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/SYSAgentKnow/"));
                }
                if (file.ContentLength == 0 || file == null)
                {
                    return Content("上传的图片没有内容");
                }
                if (file.ContentLength > 5242880)
                {
                    return Content("上传图片不能超过5MB！");
                }
                SYSCompanyImgs imgs = new SYSCompanyImgs();
                imgs.ImgUrl = "/images/SYSAgentKnow/Profile_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                file.SaveAs(path + imgs.ImgUrl);
                imgs.UserName = CurrentUser.UserName;
                imgs.Type = "优惠政策";
                imgs.Dat = DateTime.Now;
                imgs.ImgUrl = WeConfig.b_domain + imgs.ImgUrl;
                imgs.InsertAndReturnIdentity();
                return Content(imgs.ImgUrl);
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "SYSCompanyProfile_DetailImgsUp_error");
                return Content("error|服务器繁忙");
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
       [B_MenuRightsTag("删除", "Index")]
        public ActionResult Del(int ID)
        {
            string msg = "出错了!!";
            if (SYSPolicy.DeleteByID(ID) > 0)
            {
                msg = "ok";
            }
            return Content(msg);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult EditIndex(int ID)
        {
            SYSPolicy agentkow = SYSPolicy.GetEntityByID(ID);
            return View(agentkow);
        }

    }
}
