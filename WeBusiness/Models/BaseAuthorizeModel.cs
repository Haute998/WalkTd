using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeModels;

namespace WeBusiness.Models
{
    public class BaseAuthorizeModel
    {
        /// <summary>
        /// 是否验证
        /// </summary>
        public bool IsAuthorize { get; internal set; }
        /// <summary>
        /// 唯一标识码
        /// </summary>
        public string GuidCode { get; internal set; }
        /// <summary>
        /// 提示消息
        /// </summary>
        public string TempDataMsg { get; internal set; }

        /// <summary>
        /// 当前用户名(包含cookie中的用户名)
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public B_User CurrentSYSUser { get; internal set; }
    }
}