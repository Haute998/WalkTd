using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class BasePostCodeController : BaseController
    {
        //
        // GET: /BasePostCode/

        //
        // GET: /BasePostCode/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<BasePostCode> PostCodes = BasePostCode.GetAllBySort();
            return View(PostCodes);
        }
        [B_MenuRightsTag("添加", "Index")]
        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("添加", "Index")]
        public ContentResult ToAdd(BasePostCode para)
        {
            if (string.IsNullOrWhiteSpace(para.PostName))
            {
                return Content("快递公司名不能为空");
            }
            if (RepeatHelper.NoRepeat("BasePostCode", "PostName", para.PostName, para.ID) > 0)
            {
                return Content("快递公司名已存在");
            }
            if (!string.IsNullOrWhiteSpace(para.PostCode))
            {
                if (RepeatHelper.NoRepeat("BasePostCode", "PostCode", para.PostCode, para.ID) > 0)
                {
                    return Content("快递公司代号已存在");
                }
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
            BasePostCode para = BasePostCode.GetEntityByID(id);
            return View(para);
        }
        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToEdit(BasePostCode para)
        {
            if (string.IsNullOrWhiteSpace(para.PostName))
            {
                return Content("快递公司名不能为空");
            }
            if (RepeatHelper.NoRepeat("BasePostCode", "PostName", para.PostName, para.ID) > 0)
            {
                return Content("快递公司名已存在");
            }
            if (!string.IsNullOrWhiteSpace(para.PostCode))
            {
                if (RepeatHelper.NoRepeat("BasePostCode", "PostCode", para.PostCode, para.ID) > 0)
                {
                    return Content("快递公司代号已存在");
                }
            }
            int rtn = para.UpdateByID();
            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

        [B_MenuRightsTag("删除", "Index")]
        public ContentResult ToDel(int id)
        {
            if (BasePostCode.DeleteByID(id) > 0)
            {
                return Content("ok");
            }
            return Content("删除出错");
        }

        [B_MenuRightsTag("修改", "Index")]
        public ContentResult ToHave(int id, bool ishave)
        {
            if (BasePostCode.TohaveByID(id, ishave) > 0)
            {
                return Content("ok");
            }
            return Content((ishave == true ? "启用" : "禁用") + "出错");
        }

        [B_MenuRightsTag("更新", "Index")]
        public ContentResult UpdateDown()
        {
            try
            {

                string url = Server.MapPath("~/BaseFile/BasePostCode2016.xls");
                DataTable dt = ExportHelper.LoadExcelToDataTable(url);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BasePostCode code = new BasePostCode();
                    code.IsHave = false;
                    code.PostCode = dt.Rows[i][0].ToString();
                    code.PostName = dt.Rows[i][1].ToString();
                    BasePostCode oldCode = BasePostCode.GetByPostCode(code.PostCode);
                    if (oldCode == null)
                    {
                        code.InsertAndReturnIdentity();
                    }

                }
                return Content("ok");
            }
            catch (Exception ex)
            {
                Log.Instance.Write(ex.ToString(), "BasePostCode_UpdateDown_error");
                return Content("更新异常");
            }
        }
    }
}
