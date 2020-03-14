using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;
using WeModels.Models.C_UserModel;

namespace AgentMobile.Controllers
{
    public class MyInfoController : BaseController
    {
        //
        // GET: /MyInfo/
        /// <summary>
        /// 我的信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }

        /// <summary>
        /// 授权证书
        /// </summary>
        /// <returns></returns>
        public ActionResult AuthCert()
        {
            return View();
        }
        public ActionResult MyImg(FormCollection c)
        {
            C_User cuser = C_User.GetEntityByID(CurrentUser.ID);
            //根据前台html的name获取文件  
            HttpPostedFileBase upfile = Request.Files["file_temporaryImage"];
            if (upfile == null)
            {

                return Content("您没有选择文件");

            }
            string oldMediaName = upfile.FileName;
            //判断文件大小是否符合要求  
            if (upfile.ContentLength >= (5242880))
            {
                return Content("请上传5M以内的文件！");
            }

            string imgName = DateTime.Now.ToString("hhmmss") + DateTime.Now.Ticks;
            string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
            bool flag = false;
            cuser.PortraitUrl = "/images/myimg" + imgName + ext;
            cuser.UpdateByID();
            try
            {
                if (!Directory.Exists(Server.MapPath("~/images/myimg")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/myimg"));
                }
                upfile.SaveAs(Server.MapPath(cuser.PortraitUrl));
                flag = true;
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "myimg_error");
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



        public ActionResult MyWxQRCode() 
        {
            ViewData["user"] = C_UserVM.GetVMByID(CurrentUser.ID);
            return View();
        }


        public ActionResult UpMyWxQRCode(FormCollection c)
        {
            C_User cuser = C_User.GetEntityByID(CurrentUser.ID);
            //根据前台html的name获取文件  
            HttpPostedFileBase upfile = Request.Files["file_temporaryImage"];
            if (upfile == null)
            {

                return Content("您没有选择文件");

            }
            string oldMediaName = upfile.FileName;
            //判断文件大小是否符合要求  
            if (upfile.ContentLength >= (5242880))
            {
                return Content("请上传5M以内的文件！");
            }

            string imgName = DateTime.Now.ToString("hhmmss") + DateTime.Now.Ticks;
            string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
            bool flag = false;

            string oldQRCode = cuser.WxQRCode;

            cuser.WxQRCode = "/images/MyWxQRCode/" + imgName + ext;
            cuser.UpdateByID();
            try
            {
                if (!Directory.Exists(Server.MapPath("~/images/MyWxQRCode")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/MyWxQRCode"));
                }
                upfile.SaveAs(Server.MapPath("~" + cuser.WxQRCode));
                flag = true;


                System.IO.File.Delete(Server.MapPath("~") + oldQRCode);
            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "MyWxQRCode_error");
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

    }
}
