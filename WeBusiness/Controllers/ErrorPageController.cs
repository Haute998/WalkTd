using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeBusiness.Controllers
{
    public class ErrorPageController : BaseController
    {
        //
        // GET: /ErrorPage/

        public ActionResult to404()
        {
            return View();
        }

    }
}
