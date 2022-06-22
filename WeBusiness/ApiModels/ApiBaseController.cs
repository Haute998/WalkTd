﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeModels;
using System.Net.Http.Headers;

namespace WeBusiness
{
    public class ApiBaseController : ApiController
    {
        protected PDAUser PdaUser { get; private set; }
        protected string UserToken { get; private set; }
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            HttpContextBase context = (HttpContextBase)controllerContext.Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;

            if (request["UserToken"] != null)
            {
                UserToken = request["UserToken"].ToString();

                if (!string.IsNullOrWhiteSpace(UserToken))
                {
                    CachePDAUser User = PDAUserMsg.TokenGetUser(UserToken);
                    if (User != null)
                    {
                        PdaUser = PDAUser.GetEntityByID(User.UserID);
                    }
                }
            }

            base.Initialize(controllerContext);
        }
    }
}