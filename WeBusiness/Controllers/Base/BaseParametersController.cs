using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using DAL;
using System.IO;

namespace WeBusiness.Controllers
{
    public class BaseParametersController : BaseController
    {
        //
        // GET: /BaseParameters/
        //
        // GET: /BaseParameters/
        [B_MenuRightsTag("查看")]
        public ActionResult para()
        {
            List<BaseParameters> parameters = BaseParameters.GetEntitysIsShow();
            return View(parameters);
        }
        [B_MenuRightsTag("修改", "para")]
        public ContentResult ToSave(FormCollection c)
        {
            try
            {
                string[] parakeys = c["parakey"].Split(',');
                string[] paravals = c["paraval"].Split(',');

                if (parakeys.Length != paravals.Length)
                {
                    return Content("保存设置出现异常");
                }
                for (int i = 0; i < parakeys.Length; i++)
                {
                    BaseParameters oldPara = BaseParameters.GetEntityByParametersKey(parakeys[i]);


                    BaseParameters.EditByID(parakeys[i], paravals[i]);

                    SYSLog.add("系统参数["+oldPara.NickName+"]从["+oldPara.ParametersVal + "]修改为[" + paravals[i]+"]", "后台用户" + CurrentUser.UserName + "(" + CurrentUser.Name + ")", CurrentURL,"系统参数","电脑端后台");

                }
                return Content("ok");

            }
            catch (Exception ex)
            {
                DAL.Log.Instance.Write(ex.ToString(), "BaseParameters_ToSave_error");
                return Content("保存出错");
            }

        }

        [B_MenuRightsTag("修改", "para")]
        public ContentResult saveImg(int paraID)
        {
            BaseParameters oldpara = BaseParameters.GetEntityByID(paraID);
            string oldImgUrl = oldpara.ParametersVal;//原来的图片
            if (oldImgUrl.Contains("?"))
            {
                oldImgUrl = oldImgUrl.SubStringSafe(0, oldImgUrl.IndexOf("?"));
            }
            string newParametersVal = oldImgUrl;
            string savefileurl1 = "";
            var file = Request.Files["paraimg"];
            if (file != null && file.ContentLength > 0)
            {
                if (file.ContentLength > 5242880)
                {
                    return Content("fail|图片不能超过5MB！");
                }
                if (!Directory.Exists(Server.MapPath("~/images/BaseParameters/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/BaseParameters/"));
                }
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                newParametersVal = "/images/BaseParameters/" + paraID + ext;
                string saveName = newParametersVal;//实际保存文件名

                if (string.IsNullOrWhiteSpace(oldImgUrl) == false)
                {

                    string delFile = Server.MapPath("~") + oldImgUrl;
                    System.IO.File.Delete(delFile);
                }
                savefileurl1 = path + saveName;
            }
            if (string.IsNullOrWhiteSpace(savefileurl1) == false)
            {
                file.SaveAs(savefileurl1);
                newParametersVal = newParametersVal + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");

            }
            return Content("ok|" + newParametersVal);
        }

    }
}
