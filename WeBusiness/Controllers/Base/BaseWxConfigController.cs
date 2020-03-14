using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBusiness.Models;
using WeModels;

namespace WeBusiness.Controllers
{
    public class BaseWxConfigController : BaseController
    {
        //
        // GET: /BaseWxConfig/
        [B_MenuRightsTag("查看")]
        public ActionResult Index()
        {
            BaseWxConfig wxconfig = BaseWxConfig.GetWxConfig();
            return View(wxconfig);
        }

        [B_MenuRightsTag("修改", "Index")]
        public ContentResult Edit(BaseWxConfig config)
        {
            int rtn = 0;
            if (config.ID > 0)
            {
                rtn = config.EditByID();
            }
            else
            {
                config.IsActive = true;
                rtn = config.InsertAndReturnIdentity();
            }


            if (rtn > 0)
            {
                return Content("ok");
            }
            return Content("保存出错");
        }

    }
}
