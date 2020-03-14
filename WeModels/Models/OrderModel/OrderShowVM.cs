using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeModels.Models.OrderModel
{
    public class OrderShowVM:Order
    {
        /// <summary>
        /// 用户的Name字段
        /// </summary>
        public string C_User_Name { get; set; }
        /// <summary>
        /// 代理等级
        /// </summary>
        public string C_UserTypeName { get; set; }
        /// <summary>
        /// 上级用户的Name字段
        /// </summary>
        public string ParentUser_Name { get; set; }
        /// <summary>
        /// 快递名
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string PostNo { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime PostDat { get; set; }

    }
}
