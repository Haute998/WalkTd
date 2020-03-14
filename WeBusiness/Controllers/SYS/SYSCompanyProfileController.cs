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
    public class SYSCompanyProfileController : BaseController
    {
        //
        // GET: /SYSCompanyProfile/
        [B_MenuRightsTag("修改")]
        public ActionResult Index()
        {
            SYSCompanyProfile Profile = SYSCompanyProfile.GetProfile();
            if (Profile == null) 
            {
                Profile = new SYSCompanyProfile();
            }
            return View(Profile);
        }

        [B_MenuRightsTag("修改", "Index")]
        public ContentResult Save(SYSCompanyProfile Profile)
        {
            int rtn = 0;
            if (Profile.ID == 0)
            {
                rtn=Profile.InsertAndReturnIdentity();
            }
            else 
            {
                rtn=Profile.UpdateByID();
            }
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存失败");
        }

         [B_MenuRightsTag("修改", "Index")]
        public ContentResult GetDetailTemp()
        {
            SYSCompanyProfile Profile = SYSCompanyProfile.GetProfile();
            if (Profile != null)
            {
                return Content(Profile.Tmp);
            }
            return Content("");
        }

          [B_MenuRightsTag("修改", "Index")]
         public ContentResult DetailTempSave(string detail)
         {
             try
             {
                 int rtn = 0; ;
                 SYSCompanyProfile Profile = SYSCompanyProfile.GetProfile();
                 if (Profile == null)
                 {
                     Profile = new SYSCompanyProfile();
                     Profile.Tmp = detail;
                     rtn = Profile.InsertAndReturnIdentity();
                 }
                 else
                 {
                     rtn = SYSCompanyProfile.SaveTmp(detail);

                 }
                 if (rtn > 0)
                 {
                     return Content("ok");

                 }
                 return Content("网络异常");
             }
             catch (Exception ex)
             {
                 Log.Instance.Write(ex.ToString(), "SYSCompanyProfile_DetailTempSave_error");
                 return Content("网络异常");
             }
         }

        [B_MenuRightsTag("修改", "Index")]
        public ActionResult DetailImgsUp()
        {
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/images/SYSCompanyImgs/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/SYSCompanyImgs/"));
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
                imgs.ImgUrl = "/images/SYSCompanyImgs/Profile_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                file.SaveAs(path + imgs.ImgUrl);
                imgs.UserName = CurrentUser.UserName;
                imgs.Type = "公司简介";
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

    }
}
