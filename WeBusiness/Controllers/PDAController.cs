using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;
using System.IO;
using System.Text;

namespace WeBusiness.Controllers
{
    public class PDAController : BaseController
    {
        //
        // GET: /PDA/

        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            List<PDA> p = PDA.GetEntitysAllMore();

            PDAParam param = PDAParam.GetEntityByID(1);

            if (CurrentUser.UserName == WeConfig.robot)
            {
                ViewData["ss"] = "显示";
            }
            else
            {
                ViewData["ss"] = "隐藏";
            }

            ViewBag.Param = param;

            return View(p);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult PDAUserMsg()
        {
            List<Supplier> supplierList = Supplier.GetEntitysAll();
            ViewBag.SupplierList = supplierList;
            return View();
        }

        [B_MenuRightsTag("查看")]
        public ActionResult DeviceMsg()
        {
            List<PDA> p = PDA.GetEntitysAll();
            return View(p);
        }

        public ActionResult LoadPDAList(string KeyWord)
        {
            List<PDA> pda=PDA.SeacherPDA(KeyWord);
            return Json(pda, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllPdaAPP()
        {
            List<PDAApp> ListApp = PDAApp.GetEntitysAll();
            return Json(ListApp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAgentAllPdaAPP()
        {
            List<PDAAgentApp> ListApp = PDAAgentApp.GetEntitysAll();
            return Json(ListApp, JsonRequestBehavior.AllowGet);
        }

        [B_MenuRightsTag("查看")]
        public ActionResult ParamSetting()
        {
            PDAParam param = PDAParam.GetEntityByID(1);
            ViewBag.Param = param;
            return View();
        }

        [B_MenuRightsTag("启用/禁用", "DeviceMsg")]
        public ContentResult toPublish(string isPublish, int id)
        {
            bool rtn = Product.toPDA(isPublish, id);
            if (rtn)
            {
                return Content("ok");
            }
            else
            {
                return Content("操作失败,网络异常");
            }
        }

        public ActionResult Add()
        {
            return View();
        }
        [B_MenuRightsTag("修改", "DeviceMsg")]
        public ActionResult Edit(int ID)
        {
            PDA P = PDA.GetEntityByID(ID);
            return View(P);
        }

        public ActionResult toAdd(PDA p)
        {
            if (string.IsNullOrWhiteSpace(p.Keys))
            {
                return Content("序列号不能为空");
            }

            if (PDA.GetIsByKeys(p.Keys) != null)
            {
                return Content("序列号已经存在,请检查后再试！");
            }

            p.CreateTime = CommonFunc.GetNowTimestamp();
            p.State = p.State == "on" ? "启用" : "禁用";

            int rtn = p.InsertAndReturnIdentity();
            return Content(rtn > 0 ? "ok" : "添加出错了！！");
        }

        public ActionResult Del(int ID)
        {
            int rtn = PDA.DeleteByID(ID);
            return Content(rtn > 0 ? "ok" : "删除出错了！！");
        }

        [B_MenuRightsTag("修改", "DeviceMsg")]
        public ActionResult TOEdit(PDA p)
        {
            PDA pp = PDA.GetEntityByID(p.ID);
            pp.SupplierId = p.SupplierId;
            int rtn = pp.UpdateByID();
            return Content(rtn > 0 ? "ok" : "修改出错了！！");
        }

        public ActionResult GetAllPDAUser()
        {
            List<PDAUser> pdauserList = PDAUser.GetEntitysAllMore();

            return Json(pdauserList,JsonRequestBehavior.AllowGet);
        }

        public ContentResult ToAddPDAUser(FormCollection userform)
        {
            PDAUser pdauser = new PDAUser();

            pdauser.PUserName = userform["PUserName"].ToString();
            pdauser.Password = userform["Password"].ToString();
            pdauser.PRealName = userform["PRealName"].ToString();
            pdauser.Remark = userform["Remark"].ToString();
            pdauser.SupplierID = userform["SupplierID"] != null ? int.Parse(userform["SupplierID"]) : 0;

            int ID = pdauser.InsertAndReturnIdentity();

            if (ID > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("添加失败!");
            }
        }

        public ContentResult DeletePDAUser(int UserId)
        {
            int iRet= PDAUser.DeleteByID(UserId);
            if (iRet > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败!");
            }
        }

        public ContentResult ToEditPDAUser(FormCollection userform)
        {
            int ID = int.Parse(userform["UserId"].ToString());
            PDAUser pdauser = PDAUser.GetEntityByID(ID);
            pdauser.Password = userform["Password"].ToString();
            pdauser.PRealName = userform["PRealName"].ToString();
            pdauser.Remark = userform["Remark"].ToString();
            pdauser.SupplierID = userform["SupplierID"] != null ? int.Parse(userform["SupplierID"]) : 0;

            int iRet = pdauser.UpdateByID();

            if (iRet > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("添加失败!");
            }
        }

        public ActionResult AuthorityMsg(int PDAUserId)
        {
            List<PDAFuntion> listFunc = PDAFuntion.GetNotDisabledAll();
            List<PDAUserFunc> listUserFunc = PDAUserFunc.GetEntitysByUserId(PDAUserId);

            for (int i = 0; i < listFunc.Count; i++)
            {
                if (listUserFunc.Where(u => u.FunCode == listFunc[i].FunCode).Count() > 0)
                {
                    listFunc[i].IsCheck = true;
                }
            }

            ViewData["FunctionList"] = listFunc;
            ViewBag.UserId = PDAUserId;
            return View();
        }

        public ContentResult ToSetUserAuth(int PDAUserId, string FunCodeSet)
        {
            bool IsOK = PDAUserFunc.UpdateUserFunc(PDAUserId, FunCodeSet);
            if (IsOK)
            {
                return Content("ok");
            }
            else
            {
                return Content("设置失败！");
            } 
        }

        public ContentResult DelAppVer(int id)
        {
            int iRet = PDAApp.DeleteByID(id);
            if (iRet>0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败！");
            } 
        }

        public ContentResult DelAgentAppVer(int id)
        {
            int iRet = PDAAgentApp.DeleteByID(id);
            if (iRet > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("删除失败！");
            }
        }

        public ActionResult AddAPP()
        {
            return View();
        }
        public ActionResult AddAgentAPP()   // 添加代理APP
        {
            return View();
        }
        public ContentResult ToAddApp(FormCollection userform)
        {
            string msg = string.Empty;
            PDAApp app = new PDAApp();
            app.Ver = userform["Ver"];
            app.AppName = userform["AppName"];

            if(userform["IsOK"]!=null)
            {
                app.IsOK = userform["IsOK"] == "on" ? true : false;
            }
            var file = Request.Files[0];

            string FilePath = "/File/APP/";
            string FileName = "";
            string errMsg = "";

            if (!FileDeal(Request.Files[0], FilePath, out FileName, out errMsg))
            {
                return Content(errMsg);
            }

            SizeUnit sizeObj = StorageUnit.AutoConver(Request.Files[0].ContentLength);
            app.Size = sizeObj.size.ToString() + sizeObj.unit;
            app.AppName = Request.Files[0].FileName;
            app.AppPath = FilePath + Request.Files[0].FileName;
            app.CreatTime = CommonFunc.GetNowTimestamp();

            if (app.IsOK)
            {
                PDAApp.UpdateIsOK();
            }

            if (app.InsertAndReturnIdentity() > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "添加失败!";
            }

            return Content(msg);
        }

        public ContentResult ToAddAgentApp(FormCollection userform)
        {
            string msg = string.Empty;
            PDAAgentApp app = new PDAAgentApp();
            app.Ver = userform["Ver"];
            app.AppName = userform["AppName"];

            if (userform["IsOK"] != null)
            {
                app.IsOK = userform["IsOK"] == "on" ? true : false;
            }
            var file = Request.Files[0];

            string FilePath = "/File/APP/";
            string FileName = "";
            string errMsg = "";

            if (!FileDeal(Request.Files[0], FilePath, out FileName, out errMsg))
            {
                return Content(errMsg);
            }

            SizeUnit sizeObj = StorageUnit.AutoConver(Request.Files[0].ContentLength);
            app.Size = sizeObj.size.ToString() + sizeObj.unit;
            app.AppName = Request.Files[0].FileName;
            app.AppPath = FilePath + Request.Files[0].FileName;
            app.CreatTime = CommonFunc.GetNowTimestamp();

            if (app.IsOK)
            {
                PDAAgentApp.UpdateIsOK();
            }

            if (app.InsertAndReturnIdentity() > 0)
            {
                msg = "ok";
            }
            else
            {
                msg = "添加失败!";
            }

            return Content(msg);
        }

        public ContentResult SetDefult(int ID)
        { 
            PDAApp.UpdateIsOK();
            if (PDAApp.SetIDIsOK(ID) > 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("设置失败！");
            }
        }

        private bool FileDeal(HttpPostedFileBase file, string filePath, out string filename)
        {
            string errorMsg = "";
            return FileDeal(file, filePath, out filename, out errorMsg);
        }
        private bool FileDeal(HttpPostedFileBase file, string filePath, out string filename, out string errorMsg)
        {
            try
            {
                errorMsg = "ok";
                string ext = Path.GetExtension(file.FileName);
                if (file.ContentLength == 0 || file == null) errorMsg = "上传的图片没有内容！";
                //if (file.ContentLength > 5242880) errorMsg = "上传图片不能超过5MB！";
                //if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif") errorMsg = "上传图片格式不对！";
                //string newFileName = getDateTimeStamp().ToString() + ext;
                string newFileName = file.FileName;

                if (!System.IO.Directory.Exists("~/" + filePath))
                {
                    Directory.CreateDirectory(Server.MapPath("~/" + filePath));
                }

                file.SaveAs(Server.MapPath("~/" + filePath + newFileName));

                filename = filePath + newFileName;
                return true;
            }
            catch (Exception ex)
            {
                filename = null;
                errorMsg = ex.Message;
                return false;
            }
        }
        public long getDateTimeStamp()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }

        public ContentResult SaveParam(FormCollection paramFrm)
        {
            PDAParam param = PDAParam.GetEntityByID(1);
            param.ScanProcess = paramFrm["ScanProcess"] != null ? int.Parse(paramFrm["ScanProcess"]) : 0;
            param.IsLink = paramFrm["IsLink"] != null && paramFrm["IsLink"].ToString() == "on" ? true : false;
            param.IsInto = paramFrm["IsInto"] != null && paramFrm["IsInto"].ToString() == "on" ? true : false;
            param.IsOut = paramFrm["IsOut"] != null && paramFrm["IsOut"].ToString() == "on" ? true : false;
            param.IsRtn = paramFrm["IsRtn"] != null && paramFrm["IsRtn"].ToString() == "on" ? true : false;
            param.SmallPacking = paramFrm["SmallPacking"] != null && paramFrm["SmallPacking"].ToString() == "on" ? true : false;
            param.MiddlePacking = paramFrm["MiddlePacking"] != null && paramFrm["MiddlePacking"].ToString() == "on" ? true : false;
            param.LinkWay = paramFrm["LinkWay"] != null ? int.Parse(paramFrm["LinkWay"]) : 0;
            param.IsLinkProduct = paramFrm["IsLinkProduct"] != null && paramFrm["IsLinkProduct"].ToString() == "on" ? true : false;
            param.IsLinkSupplier = paramFrm["IsLinkSupplier"] != null && paramFrm["IsLinkSupplier"].ToString() == "on" ? true : false;
            param.SmallInto = paramFrm["SmallInto"] != null && paramFrm["SmallInto"].ToString() == "on" ? true : false;
            param.MiddleInto = paramFrm["MiddleInto"] != null && paramFrm["MiddleInto"].ToString() == "on" ? true : false;
            param.BigInto = paramFrm["BigInto"] != null && paramFrm["BigInto"].ToString() == "on" ? true : false;
            param.IntoWay = paramFrm["IntoWay"] != null ? int.Parse(paramFrm["IntoWay"]) : 0;
            param.IsIntoProduct = paramFrm["IsIntoProduct"] != null && paramFrm["IsIntoProduct"].ToString() == "on" ? true : false;
            param.IsIntoSupplier = paramFrm["IsIntoSupplier"] != null && paramFrm["IsIntoSupplier"].ToString() == "on" ? true : false;
            param.SmallOut = paramFrm["SmallOut"] != null && paramFrm["SmallOut"].ToString() == "on" ? true : false;
            param.MiddleOut = paramFrm["MiddleOut"] != null && paramFrm["MiddleOut"].ToString() == "on" ? true : false;
            param.BigOut = paramFrm["BigOut"] != null && paramFrm["BigOut"].ToString() == "on" ? true : false;
            param.OutWay = paramFrm["OutWay"] != null ? int.Parse(paramFrm["OutWay"]) : 0;
            param.OrderRtn = paramFrm["OrderRtn"] != null && paramFrm["OrderRtn"].ToString() == "on" ? true : false;
            param.SmallRtn = paramFrm["SmallRtn"] != null && paramFrm["SmallRtn"].ToString() == "on" ? true : false;
            param.MiddleRtn = paramFrm["MiddleRtn"] != null && paramFrm["MiddleRtn"].ToString() == "on" ? true : false;
            param.BigRtn = paramFrm["BigRtn"] != null && paramFrm["BigRtn"].ToString() == "on" ? true : false;
            param.IsOutProduct = paramFrm["IsOutProduct"] != null && paramFrm["IsOutProduct"].ToString() == "on" ? true : false;

            param.UpdateByID();

            WeModels.PDAUserMsg.InitPDASystemParam(); // 更新缓存参数

            string DisabledFuncCode = "";
            string EnableFuncCode = "";

            #region 功能代码禁用启用
            if (param.IsLink) { EnableFuncCode += ",'A001'"; EnableFuncCode += ",'A006'"; }
            else { DisabledFuncCode += ",'A001'"; DisabledFuncCode += ",'A006'"; }

            if (param.IsInto) EnableFuncCode += ",'A002'";
            else DisabledFuncCode += ",'A002'";

            if (param.IsOut) EnableFuncCode += ",'A003'";
            else DisabledFuncCode += ",'A003'";

            if (param.IsRtn) EnableFuncCode += ",'A004'";
            else DisabledFuncCode += ",'A004'";

            if (param.SmallPacking) { EnableFuncCode += ",'B001'"; EnableFuncCode += ",'B013'"; }
            else { DisabledFuncCode += ",'B001'"; DisabledFuncCode += ",'B013'"; }

            if (param.MiddlePacking) { EnableFuncCode += ",'B002'"; EnableFuncCode += ",'B014'"; }
            else { DisabledFuncCode += ",'B002'"; DisabledFuncCode += ",'B014'"; }

            if (param.SmallInto) EnableFuncCode += ",'B003'";
            else DisabledFuncCode += ",'B003'";

            if (param.MiddleInto) EnableFuncCode += ",'B004'";
            else DisabledFuncCode += ",'B004'";

            if (param.BigInto) EnableFuncCode += ",'B005'";
            else DisabledFuncCode += ",'B005'";

            if (param.SmallOut) EnableFuncCode += ",'B006'";
            else DisabledFuncCode += ",'B006'";

            if (param.MiddleOut) EnableFuncCode += ",'B007'";
            else DisabledFuncCode += ",'B007'";

            if (param.BigOut) EnableFuncCode += ",'B008'";
            else DisabledFuncCode += ",'B008'";

            if (param.SmallRtn) EnableFuncCode += ",'B009'";
            else DisabledFuncCode += ",'B009'";

            if (param.MiddleRtn) EnableFuncCode += ",'B010'";
            else DisabledFuncCode += ",'B010'";

            if (param.BigRtn) EnableFuncCode += ",'B011'";
            else DisabledFuncCode += ",'B011'";

            #endregion

            if (DisabledFuncCode != "" && DisabledFuncCode.Length > 4)
            {
                DisabledFuncCode = DisabledFuncCode.Substring(1, DisabledFuncCode.Length - 1);
                PDAFuntion.SetDisabledFunc(DisabledFuncCode);
            }

            if (EnableFuncCode != "" && EnableFuncCode.Length > 4)
            {
                EnableFuncCode = EnableFuncCode.Substring(1, EnableFuncCode.Length - 1);
                PDAFuntion.SetEnableFunc(EnableFuncCode);
            }
            
            return Content("ok");
        }

        public ActionResult PDAAPPMsg()
        {
            return View();
        }

        [B_MenuRightsTag("查看")]
        public ActionResult PDALog()
        {
            return View();
        }

        public ActionResult GetPDALog(BaseDateSearch condition)
        {
            string where = string.Empty;
            if (!string.IsNullOrWhiteSpace(condition.DatCreateB))
            {
                where += string.Format(" and OpearTime >={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateB + " 00:00:00")));
            }
            if (!string.IsNullOrWhiteSpace(condition.DatCreateE))
            {
                where += string.Format(" and OpearTime <={0} ", CommonFunc.GetTimestamp(Convert.ToDateTime(condition.DatCreateE + " 23:59:59")));
            }
            if (!string.IsNullOrWhiteSpace(condition.keyword))
            {
                where += string.Format(" and EventName+TypeName+BarCode like '%{0}%' ", condition.keyword);
            }

            PageJsonModel<PDALog> page = new PageJsonModel<PDALog>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = "PDALog";
            page.strSelect = " * ";
            page.strWhere = where;
            page.strOrder = "OpearTime desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
