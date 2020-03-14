using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace Mobile.Controllers
{
    public class AdviceController : Controller
    {
        //
        // GET: /Advice/

        public ActionResult Index()
        {
            return View();
        }
        public ContentResult ToAdd(string Contents, string Phone,string Name)
        {
            try
            {
                C_UserAdvice c = new C_UserAdvice();
                c.Contents = Contents;
                c.Phone = Phone;
                c.Name = Name;
                c.DatCreate = DateTime.Now;
                c.State = "未审核";
                c.InsertAndReturnIdentity();
                return Content("ok");
            }
            catch (Exception)
            {
                return Content("提交失败");
            }
        }
    }
}
