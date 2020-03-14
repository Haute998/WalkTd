using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class SYSIntegralCodeController : BaseController
    {
        //
        // GET: /SYSIntegralCode/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            return View();
        }
        [B_MenuRightsTag("查看", "Index")]
        public ActionResult GetList(SYSIntegralCodeSearch condition)
        {
            string where = SYSIntegralCodeSearch.StrWhere(condition);

            PageJsonModel<SYSIntegralCode> page = new PageJsonModel<SYSIntegralCode>();
            page.pageIndex = condition.pageIndex;
            page.strForm = " SYSIntegralCode ";
            page.strSelect = " * ";
            page.pageSize = condition.pageSize;
            page.strWhere = where;
            page.strOrder = " ID Desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("导出", "Index")]
        public ActionResult ExportExcel(SYSIntegralCodeSearch condition)
        {
            StringBuilder where = new StringBuilder();
            where.Append("select WaterCode,IntegralCode from [SYSIntegralCode] where 1=1 ");
            where.Append(SYSIntegralCodeSearch.StrWhere(condition));
            DataTable dt = ExportWay.ExcelDataTable(where.ToString());
            string[] list = { "流水码", "防伪码" };
            return File(ExportWay.GetExcel(dt, list), "application/vnd.ms-excel", "防伪码" + DateTime.Now.ToShortTimeString() + ".xls");
        }
        [B_MenuRightsTag("导入", "Index")]
        public ActionResult leading_in()
        {
            HttpPostedFileBase upfile = Request.Files["leadtxt"];
            if (upfile == null)
            {

                return Content("fail|您没有选择文件");

            }
            //判断文件大小是否符合要求  
            //if (upfile.ContentLength >= (5242880))
            //{
            //    return Content("fail|请上传5M以内的文件！");
            //}
            string ext = Path.GetExtension(upfile.FileName);//获得文件扩展名
            if (ext != ".txt")
            {
                return Content("fail|请上传有效的txt文件");
            }

            //利用InputStream 属性直接从HttpPostedFile对象读取文本内容  
            System.IO.Stream MyStream;
            int FileLen;
            FileLen = upfile.ContentLength;
            // 读取文件的 byte[]   
            byte[] bytes = new byte[FileLen];
            MyStream = upfile.InputStream;
            MyStream.Read(bytes, 0, FileLen);
            string text = Encoding.UTF8.GetString(bytes);

            string error = "";
            int errorcnt = 0;
            List<SYSIntegralCode> codes = new List<SYSIntegralCode>();
            using (StringReader sr = new StringReader(text))
            {
                try
                {
                    string line;
                    int lineIndex = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineIndex++;
                        SYSIntegralCode codeModel = new SYSIntegralCode();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            error += "第" + lineIndex + "行导入失败：" + "空行。";
                            errorcnt++;
                            continue;
                        }
                        string[] codeArray = line.Split(',');
                        if (codeArray.Length < 3)
                        {
                            error += "第" + lineIndex + "行导入失败：" + "参数数量不正确。";
                            errorcnt++;
                            continue;
                        }
                        codeModel.WaterCode = codeArray[0];
                        codeModel.IntegralCode = codeArray[2];
                        codeModel.Dat = DateTime.Now;
                        codeModel.State = "未使用";
                        if (codeModel.IsHave())
                        {
                            error += "第" + lineIndex + "行导入失败：" + "已经存在相同的。";
                            errorcnt++;
                            continue;
                        }

                        codes.Add(codeModel);
                    }
                }
                catch (Exception ex)
                {
                    sr.Close();
                    DAL.Log.Instance.Write(ex.ToString(), "SYSIntegralCode_leading_in_error");
                    return Content("fail|导入出现异常");
                }
            }


            foreach (var item in codes)
            {
                item.InsertAndReturnIdentity();
            }


            return Content(string.Format("ok|【导入完成,成功{0}条,失败{1}条】{2}。", codes.Count, errorcnt, error));
        }
        [B_MenuRightsTag("删除", "Index")]
        public ActionResult del(int id)
        {
            int rtn = SYSIntegralCode.DeleteByID(id);
            return Content(rtn > 0 ? "ok" : "删除失败");
        }

    }
}
