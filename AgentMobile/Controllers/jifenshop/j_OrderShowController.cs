using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels;

namespace AgentMobile.Controllers
{
    public class j_OrderShowController : j_baseController
    {
        //
        // GET: /j_OrderShow/

        public ActionResult logistics(string orderNo)
        {
            j_OrderPost orderPost = j_OrderPost.GetInfoByOrderNo(orderNo);
            if (orderPost == null)
            {
                return View("Error", new ErrorPage { Title = "", Message = "无物流信息" });
            }
            BasePostCode PostCode = BasePostCode.GetEntityByID(orderPost.CodeID);


            return Redirect("https://m.kuaidi100.com/index_all.html?type=" + orderPost.PostName + "&postid=" + orderPost.PostNo + "&callbackurl=" + Request.Url);
        }

    }
}
