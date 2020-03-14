using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using DAL;

namespace WeBusiness.Controllers
{
    public class BaseParametersManagerController : BaseController
    {
        //
        // GET: /BaseParametersManager/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        private string StrWhere(BaseParametersSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and (ParametersKey like '%{0}%' or NickName like '%{0}%' or Remark like '%{0}%' or Type like '%{0}%')", condition.keyword);
            }
            return where;
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetPage(BaseParametersSearch condition)
        {
            string where = StrWhere(condition);
            PageJsonModel<BaseParameters> page = new PageJsonModel<BaseParameters>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " BaseParameters ";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "Type,Sort";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }


        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(BaseParameters para)
        {
            if (string.IsNullOrWhiteSpace(para.ParametersKey))
            {
                return Content("唯一标识不能为空");
            }
            if (string.IsNullOrWhiteSpace(para.NickName))
            {
                return Content("参数名称不能为空");
            }
            if (RepeatHelper.NoRepeat("BaseParameters", "ParametersKey", para.ParametersKey, para.ID) > 0)
            {
                return Content("唯一标识已存在");
            }
            int rtn = para.InsertAndReturnIdentity();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("添加出错");
        }
        [B_MenuRightsTag("修改", "Index")]
        public ActionResult Edit(int id)
        {
            BaseParameters para = BaseParameters.GetEntityByID(id);
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(BaseParameters para)
        {
            if (string.IsNullOrWhiteSpace(para.NickName))
            {
                return Content("参数名称不能为空");
            }
            if (RepeatHelper.NoRepeat("BaseParameters", "ParametersKey", para.ParametersKey, para.ID) > 0)
            {
                return Content("唯一标识已存在");
            }
            int rtn = para.SYSEditByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            BaseParameters oldpara = BaseParameters.GetEntityByID(id);

            if (BaseParameters.DeleteByID(id) > 0)
            {
                if (oldpara.valType == "image")
                {
                    try
                    {
                        string oldImgUrl = oldpara.ParametersVal;//原来的图片
                        if (oldImgUrl.Contains("?"))
                        {
                            oldImgUrl = oldImgUrl.SubStringSafe(0, oldImgUrl.IndexOf("?"));
                        }
                        if (string.IsNullOrWhiteSpace(oldImgUrl) == false)
                        {

                            string delFile = Server.MapPath("~") + oldImgUrl;
                            System.IO.File.Delete(delFile);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Content("ok");
            }
            return Content("删除出错");
        }

    }
}
