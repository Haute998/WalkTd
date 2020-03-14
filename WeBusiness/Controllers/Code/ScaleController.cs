using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class ScaleController : BaseController
    {
        //
        // GET: /Scale/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }

       [B_MenuRightsTag("条码导入","Index")]
        public ActionResult ScaleAdd()
        {
            string msg = "";
            try
            {
                var file = Request.Files[0];
                string path = Request.MapPath("~/");
                string ext = Path.GetExtension(file.FileName);//获得文件扩展名
                if (!Directory.Exists(Server.MapPath("~/Codetxt/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Codetxt/"));
                }

                if (file.ContentLength == 0 || file == null)
                {
                    msg = "上传的文件没有内容！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }

                if (file.ContentLength > 3242880)
                {
                    msg = "上传的文件不能超过3MB！！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }

                if (ext != ".txt")
                {
                    msg = "上传文件格式不对！";
                    TempData["ToIndex_err"] = msg;
                    return View();
                }

                string DatNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                file.SaveAs(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
                StreamReader sr = new StreamReader(Server.MapPath("~/Codetxt/" + DatNow + ".txt"), Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] list = line.ToString().Split(',');
                    //if (Scale.GetBoolCodeRepeat(list))
                    //{
                    Scale code = new Scale();
                    code.BigCode = list[0];
                    code.SmallCode = list[2];
                    code.AntiCode = list[4];
                    code.CreateTime = CommonFunc.GetNowTimestamp();
                    code.InsertAndReturnIdentity();
                    //}
                }
                sr.Close();
                System.IO.File.Delete(Server.MapPath("~/Codetxt/" + DatNow + ".txt"));
                msg = "导入成功！";
                TempData["ToIndex_err"] = msg;
                return View("Index");
            }
            catch
            {
                msg = "导入失败,文件太大！！";
                TempData["ToIndex_err"] = msg;
                return View("Index");
            }

        }
        public ActionResult GetCodePage(Scale condition)
        {
            string where = string.Empty;
            return GetPages(condition, where);
        }
        [B_MenuRightsTag("查看")]
        public ActionResult BigCodeIndex()
        {
            return View();
        }
        [B_MenuRightsTag("查看小标", "BigCodeIndex")]
        public ActionResult SmallCodeIndex(string ID)
        {
            ViewData["BigCode"] = ID;
            return View();
        }
        public ActionResult GetSmallCodePage(Scale condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += " and (SmallCode like '%" + condition.keyword + "%' or AntiCode like '%" + condition.keyword + "%' )";
            }
            where += " and BigCode='" + condition.BigCode + "'";
            return GetPages(condition, where);
        }
        public ActionResult GetBigCodePage(BigScaleSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.BigCode))
            {
                where += " and BigCode='" + condition.BigCode + "'";
            }
            PageJsonModel<BigScale> page = new PageJsonModel<BigScale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "(select COUNT(*) SmallCount,BigCode from Scale group by BigCode) as BigScale";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = " SmallCount asc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        private ActionResult GetPages(Scale condition, string where)
        {
            PageJsonModel<Scale> page = new PageJsonModel<Scale>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "Scale";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "DatCreate desc";
            page.LoadList();

            if (page.pageResponse != null && page.pageResponse.RtnList != null && page.pageResponse.RtnList.Count > 0)
            {
                return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetThisCodePage(Scale condition)
        {
            string where = string.Empty;
            //List<Scale> BigScale = Scale.GetBigScaleList(condition.Code);
            //List<Scale> SmallScale = Scale.GetSmallScaleList(condition.Code);

            //List<Scale> CodeScale = new List<Scale>();
            //string msg = string.Empty;
            //if (BigScale.Count > 0)
            //{
            //    where += " and BigCode='" + condition.Code + "'";
            //}
            //else if (SmallScale.Count > 0)
            //{
            //    where += " and SmallCode='" + condition.Code + "'";
            //}
            //else
            //{
            //    where += " and BigCode=''";
            //}

            //return GetPages(condition, where);

            List<Scale> BarCodeList = Scale.GetBarCodeList(condition.Code);
            return Json(BarCodeList, JsonRequestBehavior.AllowGet);
        }
    }
}
