using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels
{
    public class C_UserInduce:C_User
    {
        /// <summary>
        /// 介绍人
        /// </summary>
        public string IName { get; set; }
       /// <summary>
       /// 介绍电话
       /// </summary>
        public string IPhone { get; set; }
        /// <summary>
        /// 级别
        /// </summary>
        public string LeverName { get; set; }
        /// <summary>
        /// 推荐人级别
        /// </summary>
        public string ILeverName { get; set; }
    }
}
