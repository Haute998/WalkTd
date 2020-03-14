using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class MyAddressController : BaseController
    {
        //
        // GET: /MyAddress/

        public ActionResult Index(string type)
        {
            if (type == "set") 
            {

            }
            ViewData["type"] = type;
            return View();
        }
        public ActionResult LoadMyAddress(BaseSearch condition) 
        {
            PageJsonModel<C_UserMail> page = new PageJsonModel<C_UserMail>();
            page.pageIndex = condition.pageIndex;
            page.pageSize = condition.pageSize;
            page.strForm = " C_UserMail";
            page.strSelect = " *  ";
            page.strWhere =string.Format(" and UserName='{0}' ",CurrentUser.UserName);
            page.strOrder = "ID desc";
            page.LoadList();

            return Json(page.pageResponse, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 地址添加
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id=0) 
        {
            C_UserMail address = new C_UserMail();
            if (id != 0) 
            {
                address = C_UserMail.GetEntityByID(id);
                if (address == null) 
                {
                    return View(ErrorPage.ViewName, new ErrorPage { Message = "地址有误" });
                }
            }
            return View(address);
        }

        public ActionResult toSave(C_UserMail address,FormCollection c) 
        {
            address = new C_UserMail();
            int rtn = 0;
            int id = 0;
            int.TryParse(c["ID"],out id);
            address.ID = id;
            address.UserName = CurrentUser.UserName;
            address.Address = c["Address"];
            address.Area = c["Area"];
            int AreaID = 0;
            int.TryParse(c["AreaID"], out AreaID);
            address.AreaID = AreaID;
            address.City = c["City"];
            int CityID = 0;
            int.TryParse(c["CityID"], out CityID);
            address.CityID = CityID;
            address.ContactMobile = c["ContactMobile"];
            address.ContactName = c["ContactName"];
            address.IsDefault = false;
            address.Province = c["Province"];
            int ProvinceID = 0;
            int.TryParse(c["ProvinceID"], out ProvinceID);
            address.ProvinceID = ProvinceID;
            if (address.ID == 0)
            {
                rtn=address.InsertAndReturnIdentity();
            }
            else 
            {
                C_UserMail oldMail = C_UserMail.GetEntityByID(address.ID);
                if (oldMail == null||oldMail.UserName!=CurrentUser.UserName) 
                {
                    return Content("操作异常");
                }
                rtn=address.UpdateByID();
            }
            return Content(rtn>0?"ok":"保存失败");
        }

        public ContentResult toDel(int id) 
        {
            C_UserMail mail = C_UserMail.GetEntityByID(id);
            if (mail == null || mail.UserName != CurrentUser.UserName) 
            {
                return Content("操作异常");
            }
            int rtn= C_UserMail.DeleteByID(id);
            return Content(rtn>0?"ok":"删除失败");
        }
        
        public ContentResult ToIsNow(int id) 
        {
            string rtn= C_UserMail.ToIsNowByID(id,CurrentUser.UserName);
            if (rtn == "ok") 
            {
                return Content("ok");
            }
            return Content("网络异常");
        }
    }
}
