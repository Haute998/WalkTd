using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeModels.ModelService.Common;

namespace AgentMobile.Controllers
{
    public class sendmsgController : Controller
    {
        //
        // GET: /sendmsg/
        public ContentResult sendUserVeriCode(string mobile)
        {

            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(6);
            Session["userdatacode"] = code;
            Session.Timeout = 2;
            string contents = string.Format("您的验证码为：{0}", code);
            string sendRtn = YstSmsDemoBASE64.YstSmsDemoBASE64.BASE64SendSmsMult(contents, mobile);
            if (sendRtn.Split(',')[0] == "成功")
            {
                return Content(code);
            }
            else
            {
                return Content("ok");
            }


        }

    }
}
